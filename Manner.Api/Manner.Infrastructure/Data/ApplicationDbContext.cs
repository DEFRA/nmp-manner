using Manner.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace Manner.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationMethod> ApplicationMethods { get; set; } = null!;
    public DbSet<Climate> Climates { get; set; } = null!;
    public DbSet<ClimateType> ClimateTypes { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<CropType> CropTypes { get; set; } = null!;
    public DbSet<IncorporationDelay> IncorporationDelays { get; set; } = null!;
    public DbSet<IncorporationMethod> IncorporationMethods { get; set; } = null!;

    // Define the IncorpMethodsIncorpDelays as keyless
    public DbSet<IncorpMethodsIncorpDelays> IncorpMethodsIncorpDelays { get; set; } = null!;
    public DbSet<ApplicationMethodsIncorpMethods> ApplicationMethodsIncorpMethods { get; set; } = null!;
    public DbSet<ManureGroup> ManureGroups { get; set; } = null!;
    public DbSet<ManureType> ManureTypes { get; set; } = null!;
    public DbSet<ManureTypeCategory> ManureTypeCategories { get; set; } = null!;
    public DbSet<MoistureType> MoistureTypes { get; set; } = null!;
    public DbSet<RainType> RainTypes { get; set; } = null!;
    public DbSet<SubSoil> SubSoils { get; set; } = null!;
    public DbSet<TopSoil> TopSoils { get; set; } = null!;
    public DbSet<Windspeed> Windspeeds { get; set; } = null!;
    public DbSet<Nutrient> Nutrients { get; set; } = null!;
    public DbSet<NutrientProduct> NutrientProducts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mark IncorpMethodsIncorpDelays as keyless
        modelBuilder.Entity<IncorpMethodsIncorpDelays>()
            .HasNoKey();

        // Mark ApplicationMethodsIncorpMethods as keyless

        modelBuilder.Entity<ApplicationMethodsIncorpMethods>()
            .HasNoKey();
        modelBuilder.Entity<ClimateType>().HasKey("MonthNumber");
        base.OnModelCreating(modelBuilder);
    }
}
