using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manner.Infrastructure.Repositories
{
    [Repository(ServiceLifetime.Scoped)]
    public class IncorporationMethodRepository : IIncorporationMethodRepository
    {
        private readonly ApplicationDbContext _context;

        public IncorporationMethodRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchAllAsync()
        {
            return await _context.IncorporationMethods.ToListAsync();
        }

        public async Task<IncorporationMethod?> FetchByIdAsync(int id)
        {
            return await _context.IncorporationMethods.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAsync(int methodId)
        {
            return await _context.IncorporationMethods
                .Where(im => _context.ApplicationMethodsIncorpMethods
                    .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID))
                .ToListAsync();
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAndApploicableForAsync(int methodId, string applicableFor)
        {
            if (string.IsNullOrWhiteSpace(applicableFor))
            {
                return await _context.IncorporationMethods
               .Where(im => _context.ApplicationMethodsIncorpMethods
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID))
               .ToListAsync();
            }
            else if (applicableFor.ToLower() == "null")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.ApplicationMethodsIncorpMethods.Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) 
                            && (im.ApplicableForGrass == null || im.ApplicableForArableAndHorticulture == null))
               .ToListAsync();
            }
            else if(applicableFor == "G")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.ApplicationMethodsIncorpMethods
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) && (im.ApplicableForGrass == applicableFor || im.ApplicableForGrass == "B"))
               .ToListAsync();
            }
            else if (applicableFor =="A")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.ApplicationMethodsIncorpMethods
                   .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID) && (im.ApplicableForArableAndHorticulture == applicableFor || im.ApplicableForArableAndHorticulture == "B"))
               .ToListAsync();
            }
            else if (applicableFor == "B")
            {
                return await _context.IncorporationMethods
               .Where(im => _context.ApplicationMethodsIncorpMethods
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
