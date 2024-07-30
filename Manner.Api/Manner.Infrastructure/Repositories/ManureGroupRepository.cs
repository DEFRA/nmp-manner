using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ManureGroupRepository : IManureGroupRepository
{
    private readonly ApplicationDbContext _context;
    public ManureGroupRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<ManureGroup>?> FetchAllAsync()
    {
        return await _context.ManureGroups.ToListAsync();
    }

    public async Task<ManureGroup?> FetchByIdAsync(int id)
    {
        return await _context.ManureGroups.FirstOrDefaultAsync(a => a.ID == id);
    }
}