using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class WindSpeedRepository : IWindSpeedRepository
{
    private readonly ApplicationDbContext _context;
    public WindSpeedRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<WindSpeed>?> FetchAllAsync()
    {
        return await _context.WindSpeeds.ToListAsync();
    }

    public async Task<WindSpeed?> FetchByIdAsync(int id)
    {
        return await _context.WindSpeeds.FirstOrDefaultAsync(a => a.ID == id);
    }
}