using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ApplicationMethodRepository(ILogger<ApplicationMethodRepository> logger, ApplicationDbContext applicationDbContext) : IApplicationMethodRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<ApplicationMethodRepository> _logger = logger;
    public async Task<IEnumerable<ApplicationMethod>?> FetchAllAsync()
    {
        _logger.LogTrace("ApplicationMethodRepository : FetchAllAsync() callled");
        return await _context.ApplicationMethods.ToListAsync();
    }

    public async Task<ApplicationMethod?> FetchByIdAsync(int id)
    {
        _logger.LogTrace("ApplicationMethodRepository : FetchByIdAsync({Id}) callled", id);
        return await _context.ApplicationMethods.FirstOrDefaultAsync(a => a.ID == id);
    }

    public async Task<IEnumerable<ApplicationMethod>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null)
    {
        _logger.LogTrace("ApplicationMethodRepository : FetchByCriteriaAsync({IsLiquid},{FieldType}) callled", isLiquid, fieldType);
        IQueryable<ApplicationMethod> query = _context.ApplicationMethods;

        // Determine field based on fieldType: 1 = arable, 2 = grass
        string? applicableField = fieldType switch
        {
            1 => nameof(ApplicationMethod.ApplicableForArableAndHorticulture),
            2 => nameof(ApplicationMethod.ApplicableForGrass),
            _ => null
        };

        if (isLiquid.HasValue)
        {
            query = ApplyLiquidCriteria(query, isLiquid.Value, applicableField);
        }
        else if (applicableField != null)
        {
            query = ApplyFieldOnlyCriteria(query, applicableField);
        }

        return await query.ToListAsync();
    }

    private static IQueryable<ApplicationMethod> ApplyFieldOnlyCriteria(IQueryable<ApplicationMethod> query, string applicableField)
    {
        return query.Where(a => EF.Property<string>(a, applicableField) != null &&
                                (EF.Property<string>(a, applicableField) == "B" ||
                                 EF.Property<string>(a, applicableField) == "L"));
    }

    private static IQueryable<ApplicationMethod> ApplyLiquidCriteria(IQueryable<ApplicationMethod> query, bool isLiquid, string? applicableField)
    {
        if (applicableField != null)
        {
            string liquidCondition = isLiquid ? "L" : "B";
            return query.Where(a => EF.Property<string>(a, applicableField) != null &&
                                    (EF.Property<string>(a, applicableField) == "B" ||
                                     EF.Property<string>(a, applicableField) == liquidCondition));
        }

        if (isLiquid)
        {
            return query.Where(a => (a.ApplicableForArableAndHorticulture == "B" || a.ApplicableForArableAndHorticulture == "L") ||
                                    (a.ApplicableForGrass == "B" || a.ApplicableForGrass == "L"));
        }

        return query.Where(a => a.ApplicableForArableAndHorticulture == "B" || a.ApplicableForGrass == "B");
    }
}
