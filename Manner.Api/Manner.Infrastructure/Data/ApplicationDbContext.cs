using Manner.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manner.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationMethod> ApplicationMethods { get; set; }
        public DbSet<Climate> Climates { get; set; }
        public DbSet<ClimateType>ClimateTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CropType> CropTypes { get; set; }
        public DbSet<IncorporationDelay> IncorporationDelays { get; set; }
        public DbSet<IncorporationMethod> IncorporationMethods { get; set; }

        // Define the IncorpMethodsIncorpDelays as keyless
        public DbSet<IncorpMethodsIncorpDelays> IncorpMethodsIncorpDelays { get; set; }

        public DbSet<ApplicationMethodsIncorpMethods> ApplicationMethodsIncorpMethods { get; set; }

        public DbSet<ManureGroup> ManureGroups { get; set; }
        public DbSet<ManureType> ManureTypes { get; set; }
        public DbSet<ManureTypeCategory> ManureTypeCategories { get; set; }
        public DbSet<MoistureType> MoistureTypes { get; set; }
        public DbSet<RainType> RainTypes { get; set; }
        public DbSet<SubSoil> SubSoils { get; set; }
        public DbSet<TopSoil> TopSoils { get; set; }
        public DbSet<Windspeed> Windspeeds { get; set; }

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
}
