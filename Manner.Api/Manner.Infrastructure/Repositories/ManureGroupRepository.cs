using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ManureGroupRepository(ILogger<ManureGroupRepository> logger, ApplicationDbContext applicationDbContext) : IManureGroupRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<ManureGroupRepository> _logger = logger;
    public async Task<IEnumerable<ManureGroup>?> FetchAllAsync()
    {
        _logger.LogTrace($"ManureGroupRepository : FetchAllAsync() callled");
        return await _context.ManureGroups.ToListAsync();
    }

    public async Task<ManureGroup?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ManureGroupRepository : FetchByIdAsync({id}) callled");
        return await _context.ManureGroups.FirstOrDefaultAsync(a => a.ID == id);
    }
}