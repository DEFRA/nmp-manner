using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ManureTypeCategoryRepository : IManureTypeCategoryRepository
{
    private readonly ApplicationDbContext _context;
    public ManureTypeCategoryRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<ManureTypeCategory>?> FetchAllAsync()
    {
        return await _context.ManureTypeCategories.ToListAsync();
    }

    public async Task<ManureTypeCategory?> FetchByIdAsync(int id)
    {
        return await _context.ManureTypeCategories.FirstOrDefaultAsync(a => a.ID == id);
    }
}