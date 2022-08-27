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
				UriParam uriParam = new();

				uriParam.paramFrom = context.Request.Query["dateFrom"];
				if (! DateTime.TryParseExact(uriParam.paramFrom, "dd:MM:yyyy", cultureInfo, 
					DateTimeStyles.None,out uriParam.dateFrom))
				{
					uriParam.dateFrom = DateTime.Now.AddDays(-1);
				};

				uriParam.paramTo = context.Request.Query["dateTo"];
				if (!DateTime.TryParseExact(uriParam.paramTo, "dd:MM:yyyy", cultureInfo,
					DateTimeStyles.None, out uriParam.dateTo))
				{
					uriParam.dateTo = DateTime.Now.AddDays(-1);
				};

				uriParam.paramStationId = context.Request.Query["stantionId"];
				if (!int.TryParse(uriParam.paramStationId,out uriParam.StationId))
				{
					uriParam.StationId = 0;
				};

				uriParam.paramStation = context.Request.Query["stantionName"];

				uriParam.paramPage = context.Request.Query["page"];
				uriParam.page = 1;
				if (!int.TryParse(uriParam.paramPage, out uriParam.page))
				{
					uriParam.page = 1;
				};

				WeatherRepository weather = new(uriParam); 
				try 
				{
					var res = await weather.GetWeather(dbContext);
					return Results.Ok(weather);
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
						//db.Database.Migrate();

						db.Database.EnsureDeleted();
						db.Database.EnsureCreated();
						await DataInitializer.InitUser(UM, RM, UserConstants.UserLists);
						await DataInitializer.InitMS(db);
						IParser _parser = new Parser(db);
						await _parser.GetData(new DateTime(2022, 7, 1), new DateTime(2022, 7, 31), "5125", true);
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