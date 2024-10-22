using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class SubSoilRepository(ILogger<SubSoilRepository> logger, ApplicationDbContext applicationDbContext) : ISubSoilRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<SubSoilRepository> _logger = logger;
    public async Task<IEnumerable<SubSoil>?> FetchAllAsync()
    {
        _logger.LogTrace($"SubSoilRepository : FetchAllAsync() callled");
        return await _context.SubSoils.ToListAsync();
    }

    public async Task<SubSoil?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"SubSoilRepository : FetchByIdAsync({id}) callled");
        return await _context.SubSoils.FirstOrDefaultAsync(a => a.ID == id);
    }
}