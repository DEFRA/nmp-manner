using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;

[Repository(ServiceLifetime.Scoped)]
public class NutrientRepository(ILogger<NutrientRepository> logger, ApplicationDbContext applicationDbContext) : INutrientRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<NutrientRepository> _logger = logger;
    public async Task<IEnumerable<Nutrient>?> FetchAllAsync()
    {
        _logger.LogTrace("NutrientRepository : FetchAllAsync() callled");
        return await _context.Nutrients.ToListAsync();
    }

    public async Task<Nutrient?> FetchByIdAsync(int id)
    {
        _logger.LogTrace("NutrientRepository : FetchByIdAsync({Id}) callled", id);
        return await _context.Nutrients.FirstOrDefaultAsync(a => a.ID == id);
    }
}
