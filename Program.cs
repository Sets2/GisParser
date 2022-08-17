using GisParser.Constants;

namespace GisParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            RegisterServices(builder);
            var app = builder.Build();
            Configure(app);

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
        }
        public static void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                var UM = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var RM = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var InitDB = new DataInitializer(db, UM, RM);
                InitDB.InitUser(UserConstants.UserLists);

            }
        }
    }
}