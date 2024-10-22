using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ManureTypeCategoryRepository(ILogger<ManureTypeCategoryRepository> logger, ApplicationDbContext applicationDbContext) : IManureTypeCategoryRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<ManureTypeCategoryRepository> _logger = logger;
    public async Task<IEnumerable<ManureTypeCategory>?> FetchAllAsync()
    {
        _logger.LogTrace($"ManureTypeCategoryRepository : FetchAllAsync() callled");
        return await _context.ManureTypeCategories.ToListAsync();
    }

    public async Task<ManureTypeCategory?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ManureTypeCategoryRepository : FetchByIdAsync({id}) callled");
        return await _context.ManureTypeCategories.FirstOrDefaultAsync(a => a.ID == id);
    }
}