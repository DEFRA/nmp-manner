using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class WindspeedRepository : IWindspeedRepository
{
    private readonly ApplicationDbContext _context;
    public WindspeedRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<Windspeed>?> FetchAllAsync()
    {
        return await _context.Windspeeds.ToListAsync();
    }

    public async Task<Windspeed?> FetchByIdAsync(int id)
    {
        return await _context.Windspeeds.FirstOrDefaultAsync(a => a.ID == id);
    }
}