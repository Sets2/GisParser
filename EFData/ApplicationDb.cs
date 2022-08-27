using GisParser.Data;

namespace GisParser.EFData;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<MeteorologicalStation> MeteorologicalStation { get; set; }
    public DbSet<Weather> Weather { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        //  Database.EnsureCreated();
        //  var InitTable = new MeteorologicalStationInitializer(this);
        //InitTable.Init();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Weather>().ToTable("Weather", "Weather");
        modelBuilder.Entity<MeteorologicalStation>().ToTable("MeteorologicalStation", "Weather");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //    optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GisParser;Username=postgres");

    }
}