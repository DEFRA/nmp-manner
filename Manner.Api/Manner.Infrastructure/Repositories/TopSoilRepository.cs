using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class TopSoilRepository : ITopSoilRepository
{
    private readonly ApplicationDbContext _context;
    public TopSoilRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<TopSoil>?> FetchAllAsync()
    {
        return await _context.TopSoils.ToListAsync();
    }

    public async Task<TopSoil?> FetchByIdAsync(int id)
    {
        return await _context.TopSoils.FirstOrDefaultAsync(a => a.ID == id);
    }
}
