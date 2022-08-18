using GisParser.Constants;
using Microsoft.Extensions.Hosting;
using System.Data;

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

            app.MapGet("/", () => "Hello World!");
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
            .AddEntityFrameworkStores<ApplicationDbContext>();
//            services.AddDatabaseDeveloperPageExceptionFilter();
        }
        public static async Task Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var db = services.GetRequiredService<ApplicationDbContext>();
                        var UM = services.GetRequiredService<UserManager<User>>();
                        var RM = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        //db.Database.EnsureCreated();
                        db.Database.Migrate();
                        await DataInitializer.InitUser(db, UM, RM, UserConstants.UserLists);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occured during migration");
                    }
                }
            }
        }
    }
}