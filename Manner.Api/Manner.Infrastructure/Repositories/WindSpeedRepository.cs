using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class WindspeedRepository(ILogger<WindspeedRepository> logger, ApplicationDbContext applicationDbContext) : IWindspeedRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<WindspeedRepository> _logger = logger;
    public async Task<IEnumerable<Windspeed>?> FetchAllAsync()
    {
        _logger.LogTrace($"WindspeedRepository : FetchAllAsync() callled");
        return await _context.Windspeeds.ToListAsync();
    }

    public async Task<Windspeed?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"WindspeedRepository : FetchByIdAsync({id}) callled");
        return await _context.Windspeeds.FirstOrDefaultAsync(a => a.ID == id);
    }
}