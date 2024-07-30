using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class RainTypeRepository : IRainTypeRepository
{
    private readonly ApplicationDbContext _context;
    public RainTypeRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<RainType>?> FetchAllAsync()
    {
        return await _context.RainTypes.ToListAsync();
    }

    public async Task<RainType?> FetchByIdAsync(int id)
    {
        return await _context.RainTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}