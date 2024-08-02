using Manner.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationMethod> ApplicationMethods { get; set; }
        public DbSet<Climate> Climates { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CropType> CropTypes { get; set; }
        public DbSet<IncorporationDelay> IncorporationDelays { get; set; }
        public DbSet<IncorporationMethod> IncorporationMethods { get; set; }
        public DbSet<ManureGroup> ManureGroups { get; set; }
        public DbSet<ManureType> ManureTypes { get; set; }
        public DbSet<ManureTypeCategory> ManureTypeCategories { get; set; }
        public DbSet<MoistureType> MoistureTypes { get; set; }
        public DbSet<RainType> RainTypes { get; set; }
        public DbSet<SubSoil> SubSoils { get; set; }
        public DbSet<TopSoil> TopSoils { get; set; }
        public DbSet<Windspeed> Windspeeds { get; set; }
    }
}
