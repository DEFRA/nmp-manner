using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;

[Repository(ServiceLifetime.Scoped)]
public class NutrientProductRepository(ILogger<NutrientProductRepository> logger, ApplicationDbContext applicationDbContext) : INutrientProductRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<NutrientProductRepository> _logger = logger;
    
    public async Task<IEnumerable<NutrientProduct>?> FetchAllAsync()
    {
        _logger.LogTrace("NutrientProductRepository : FetchAllAsync() callled");
        return await _context.NutrientProducts.ToListAsync();
    }

    public async Task<NutrientProduct?>FetchByIdAsync(int id)
    {
        _logger.LogTrace("NutrientProductRepository : FetchByIdAsync({Id}) callled", id);
        return await _context.NutrientProducts.FirstOrDefaultAsync(a => a.ID == id);
    }

    public async Task<IEnumerable<NutrientProduct>?> FetchByNutrientIdAsync(int nutrientId)
    {
        _logger.LogTrace("NutrientProductRepository : FetchByNutrientIdAsync({NutrientId}) callled", nutrientId);
        return await _context.NutrientProducts.Where(a => a.NutrientID == nutrientId).ToListAsync();
    }
}