using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ApplicationMethodRepository : IApplicationMethodRepository
{
    private readonly ApplicationDbContext _context;
    public ApplicationMethodRepository(ApplicationDbContext applicationDbContext) 
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<ApplicationMethod>?> FetchAllAsync()
    {
        return await _context.ApplicationMethods.ToListAsync();
    }

    public async Task<ApplicationMethod?> FetchByIdAsync(int id)
    {
        return await _context.ApplicationMethods.FirstOrDefaultAsync(a => a.ID == id);
    }

    public async Task<IEnumerable<ApplicationMethod>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null)
    {
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
            string liquidCondition = isLiquid.Value ? "L" : "B";

            if (applicableField != null)
            {
                // Exclude null values in the applicable field when filtering by fieldType
                query = query.Where(a => EF.Property<string>(a, applicableField) != null &&
                                         (EF.Property<string>(a, applicableField) == "B" ||
                                          EF.Property<string>(a, applicableField) == liquidCondition));
            }
            else
            {
                if (isLiquid.Value)
                {
                    query = query.Where(a => (a.ApplicableForArableAndHorticulture == "B" || a.ApplicableForArableAndHorticulture == "L") ||
                                             (a.ApplicableForGrass == "B" || a.ApplicableForGrass == "L"));
                }
                else
                {
                    query = query.Where(a => a.ApplicableForArableAndHorticulture == "B" || a.ApplicableForGrass == "B");
                }
            }
        }
        else if (applicableField != null)
        {
            // Apply fieldType-specific filtering and exclude null values
            query = query.Where(a => EF.Property<string>(a, applicableField) != null &&
                                     (EF.Property<string>(a, applicableField) == "B" ||
                                      EF.Property<string>(a, applicableField) == "L"));
        }

        return await query.ToListAsync();
    }



}
