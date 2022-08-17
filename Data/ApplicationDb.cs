
public class ApplicationDbContext : IdentityDbContext<User>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
    //  Database.EnsureCreated();
    //  var InitTable = new MeteorologicalStationInitializer(this);
    //InitTable.Init();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    //modelBuilder.Entity<Weather>().ToTable("Weather", "Weather");
    //modelBuilder.Entity<MeteorologicalStation>().ToTable("MeteorologicalStation", "Weather");
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
//    optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GisParser;Username=postgres");

  }
}