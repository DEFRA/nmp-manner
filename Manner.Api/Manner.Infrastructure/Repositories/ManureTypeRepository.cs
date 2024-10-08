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
    public class ManureTypeRepository : IManureTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ManureTypeRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IEnumerable<ManureType>?> FetchAllAsync()
        {
            return await _context.ManureTypes.ToListAsync();
        }

        public async Task<ManureType?> FetchByIdAsync(int id)
        {
            return await _context.ManureTypes.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<IEnumerable<ManureType>?> FetchByCriteriaAsync(
            int? manureGroupId = null,
            int? manureTypeCategoryId = null,
            int? countryId = null,
            bool? highReadilyAvailableNitrogen = null,
            bool? isLiquid = null)
        {
            IQueryable<ManureType> query = _context.ManureTypes;

            if (manureGroupId.HasValue)
            {
                query = query.Where(mt => mt.ManureGroupID == manureGroupId.Value);
            }

            if (manureTypeCategoryId.HasValue)
            {
                query = query.Where(mt => mt.ManureTypeCategoryID == manureTypeCategoryId.Value);
            }

            if (countryId.HasValue)
            {
                query = query.Where(mt => mt.CountryID == countryId.Value || mt.CountryID==3);
            }

            if (highReadilyAvailableNitrogen.HasValue)
            {
                query = query.Where(mt => mt.HighReadilyAvailableNitrogen == highReadilyAvailableNitrogen.Value);
            }

            if (isLiquid.HasValue)
            {
                query = query.Where(mt => mt.IsLiquid == isLiquid.Value);
            }

            return await query.ToListAsync();
        }
    }
}
