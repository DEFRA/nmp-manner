using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manner.Infrastructure.Repositories
{
    [Repository(ServiceLifetime.Scoped)]
    public class IncorporationMethodRepository(ILogger<IncorporationMethodRepository> logger, ApplicationDbContext applicationDbContext) : IIncorporationMethodRepository
    {
        private readonly ApplicationDbContext _context = applicationDbContext;
        private readonly ILogger<IncorporationMethodRepository> _logger = logger;
        public async Task<IEnumerable<IncorporationMethod>?> FetchAllAsync()
        {
            _logger.LogTrace($"IncorporationMethodRepository : FetchAllAsync() callled");
            return await _context.IncorporationMethods.ToListAsync();
        }

        public async Task<IncorporationMethod?> FetchByIdAsync(int id)
        {
            _logger.LogTrace($"IncorporationMethodRepository : FetchByIdAsync({id}) callled");
            return await _context.IncorporationMethods.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAsync(int methodId)
        {
            _logger.LogTrace($"IncorporationMethodRepository : FetchByAppMethodIdAsync({methodId}) callled");
            return await _context.IncorporationMethods
                .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                    .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID))
                .ToListAsync();
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAndApploicableForAsync(int methodId, string applicableFor)
        {
            _logger.LogTrace($"IncorporationMethodRepository : FetchByAppMethodIdAndApploicableForAsync({methodId},{applicableFor}) callled");
            if (string.IsNullOrWhiteSpace(applicableFor))
            {
                return await _context.IncorporationMethods
               .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID))
               .ToListAsync();
            }
            else if (applicableFor.ToLower() == "null")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.Set<ApplicationMethodsIncorpMethods>().Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) 
                            && (im.ApplicableForGrass == null || im.ApplicableForArableAndHorticulture == null))
               .ToListAsync();
            }
            else if(applicableFor == "G")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) && (im.ApplicableForGrass == applicableFor || im.ApplicableForGrass == "B"))
               .ToListAsync();
            }
            else if (applicableFor =="A")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) && (im.ApplicableForArableAndHorticulture == applicableFor || im.ApplicableForArableAndHorticulture == "B"))
               .ToListAsync();
            }
            else if (applicableFor == "B")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) && (im.ApplicableForGrass == applicableFor || im.ApplicableForArableAndHorticulture == applicableFor))
               .ToListAsync();
            }
            else
            {
                return null;
            }
        }
    }
}
