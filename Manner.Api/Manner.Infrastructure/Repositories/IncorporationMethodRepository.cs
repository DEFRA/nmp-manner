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
            _logger.LogTrace("IncorporationMethodRepository : FetchAllAsync() callled");
            return await _context.IncorporationMethods.ToListAsync();
        }

        public async Task<IncorporationMethod?> FetchByIdAsync(int id)
        {
            _logger.LogTrace("IncorporationMethodRepository : FetchByIdAsync({Id}) callled", id);
            return await _context.IncorporationMethods.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAsync(int methodId)
        {
            _logger.LogTrace("IncorporationMethodRepository : FetchByAppMethodIdAsync({MethodId}) callled", methodId);
            return await _context.IncorporationMethods
                .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                    .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID))
                .ToListAsync();
        }

        public async Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAndApplicableForAsync(int methodId, string applicableFor)
        {
            _logger.LogTrace("IncorporationMethodRepository : FetchByAppMethodIdAndApplicableForAsync({MethodId},{ApplicableFor}) callled", methodId, applicableFor);
            IQueryable<IncorporationMethod> baseQuery = GetByApplicationMethodQuery(methodId);

            if (string.IsNullOrWhiteSpace(applicableFor))
            {
                return await baseQuery.ToListAsync();
            }

            string normalizedApplicableFor = applicableFor.Trim();
            if (normalizedApplicableFor.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return await ApplyNullApplicableForFilter(baseQuery).ToListAsync();
            }

            return normalizedApplicableFor switch
            {
                "G" => await ApplyGrassFilter(baseQuery).ToListAsync(),
                "A" => await ApplyArableFilter(baseQuery).ToListAsync(),
                "B" => await ApplyBothFilter(baseQuery).ToListAsync(),
                _ => null
            };
        }

        private IQueryable<IncorporationMethod> GetByApplicationMethodQuery(int methodId)
        {
            return _context.IncorporationMethods
                .Where(im => _context.Set<ApplicationMethodsIncorpMethods>()
                    .Any(link => link.ApplicationMethodID == methodId && link.IncorporationMethodID == im.ID));
        }

        private static IQueryable<IncorporationMethod> ApplyNullApplicableForFilter(IQueryable<IncorporationMethod> query)
        {
            return query.Where(im => im.ApplicableForGrass == null || im.ApplicableForArableAndHorticulture == null);
        }

        private static IQueryable<IncorporationMethod> ApplyGrassFilter(IQueryable<IncorporationMethod> query)
        {
            return query.Where(im => im.ApplicableForGrass == "G" || im.ApplicableForGrass == "B");
        }

        private static IQueryable<IncorporationMethod> ApplyArableFilter(IQueryable<IncorporationMethod> query)
        {
            return query.Where(im => im.ApplicableForArableAndHorticulture == "A" || im.ApplicableForArableAndHorticulture == "B");
        }

        private static IQueryable<IncorporationMethod> ApplyBothFilter(IQueryable<IncorporationMethod> query)
        {
            return query.Where(im => im.ApplicableForGrass == "B" || im.ApplicableForArableAndHorticulture == "B");
        }
    }
}
