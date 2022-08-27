using GisParser.Apis;
using GisParser.Auth;
using GisParser.Constants;
using GisParser.Data;
using GisParser.EFData;
using Microsoft.Extensions.Options;

namespace GisParser
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            RegisterServices(builder);
            var app = builder.Build();
            await Configure(app);
            var apis = app.Services.GetServices<IApi>();
            foreach(var api in apis)
            {
                if (api == null) throw new InvalidProgramException("api not found");
                api.Register(app);
            }

            app.MapGet("/range/",[Authorize (Roles="user, admin")] async (HttpContext context, ApplicationDbContext dbContext) =>
            {
                var cultureInfo = new CultureInfo("ru-ru");
                DateTime dateFrom;
                string? paramFrom = context.Request.Query["dateFrom"];
                if (! DateTime.TryParseExact(paramFrom, "dd:MM:yyyy", cultureInfo, 
                    DateTimeStyles.None,out dateFrom))
                {
                    dateFrom = DateTime.Now.AddDays(-1);
                };
                DateTime dateTo;
                string? paramTo = context.Request.Query["dateTo"];
                if (!DateTime.TryParseExact(paramTo, "dd:MM:yyyy", cultureInfo,
                    DateTimeStyles.None, out dateTo))
                {
                    dateTo = DateTime.Now.AddDays(-1);
                };
                string? paramStationId = context.Request.Query["stantionId"];
                int StationId=0;
                if (!int.TryParse(paramStationId,out StationId))
                {
                    StationId = 0;
                };
                string? paramStation = context.Request.Query["stantionName"];

                var qry = (IQueryable<GisParser.Data.Weather>)dbContext.Weather.Include(x=>x.MeteorologicalStation);
                if (paramFrom != null) qry = qry.Where(o => o.Date>=dateFrom);
                if (paramTo != null) qry = qry.Where(o => o.Date <= dateTo);
                if (StationId != 0) qry = qry.Where(o => o.MeteorologicalStationId == StationId);
                else
                    if (paramStation != null) qry = qry.Where(o => o.MeteorologicalStation.Name == paramStation);

                bool isSuccess = false;
                try 
                { 
                    var res = await qry.ToListAsync();
                    isSuccess = true;
                    return Results.Ok(res);
                }
                catch (Exception)
                {
                    return Results.StatusCode(503);
                }

            });
            app.Run();
        }


        public static void RegisterServices(WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseNpgsql(builder.Configuration.GetConnectionString("SqlDb"));
 
            });
            services.AddIdentity<User, IdentityRole>()
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
//            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddSingleton<ITokenService>(new TokenService());
            services.AddSingleton<IUserRepositoryAuth>(new UserRepositoryAuth());
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.SaveToken = true;
                    option.RequireHttpsMetadata = false;
                    option.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            services.AddTransient<IApi,AuthApi>();
            services.AddTransient<IParser, Parser>();
        }

        public static async Task Configure(WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                using var scope = app.Services.CreateScope();
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var db = services.GetRequiredService<ApplicationDbContext>();
                        var UM = services.GetRequiredService<UserManager<User>>();
                        var RM = services.GetRequiredService<RoleManager<IdentityRole>>();
  
                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                        //db.Database.Migrate();
                        await DataInitializer.InitUser(UM, RM, UserConstants.UserLists);
                        await DataInitializer.InitMS(db);
                        IParser _parser = new Parser(db);
                        await _parser.GetData(new DateTime(2022,7,1), new DateTime(2022, 7, 31), "5125", true);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occured during open and initialization");
                    }
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

        }
    }
}