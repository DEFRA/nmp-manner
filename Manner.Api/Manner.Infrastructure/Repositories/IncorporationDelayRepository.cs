using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class IncorporationDelayRepository : IIncorporationDelayRepository
{
    private readonly ApplicationDbContext _context;
    public IncorporationDelayRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchAllAsync()
    {
        return await _context.IncorporationDelays.ToListAsync();
    }

    public async Task<IncorporationDelay?> FetchByIdAsync(int id)
    {
        return await _context.IncorporationDelays.FirstOrDefaultAsync(a => a.ID == id);
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId)
    {
        return await _context.IncorporationDelays
            .Where(d => _context.Set<IncorpMethodsIncorpDelays>()
                .Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID))
            .ToListAsync();
    }

}
