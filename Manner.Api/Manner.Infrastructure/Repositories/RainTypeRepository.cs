using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class RainTypeRepository(ILogger<RainTypeRepository> logger, ApplicationDbContext applicationDbContext) : IRainTypeRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<RainTypeRepository> _logger = logger;
    public async Task<IEnumerable<RainType>?> FetchAllAsync()
    {
        _logger.LogTrace($"RainTypeRepository : FetchAllAsync() callled");
        return await _context.RainTypes.ToListAsync();
    }

    public async Task<RainType?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"RainTypeRepository : FetchByIdAsync({id}) callled");
        return await _context.RainTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}