using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class TopSoilRepository(ILogger<TopSoilRepository> logger, ApplicationDbContext applicationDbContext) : ITopSoilRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<TopSoilRepository> _logger = logger;
    public async Task<IEnumerable<TopSoil>?> FetchAllAsync()
    {
        _logger.LogTrace($"TopSoilRepository : FetchAllAsync() callled");
        return await _context.TopSoils.ToListAsync();
    }

    public async Task<TopSoil?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"TopSoilRepository : FetchByIdAsync({id}) callled");
        return await _context.TopSoils.FirstOrDefaultAsync(a => a.ID == id);
    }
}
