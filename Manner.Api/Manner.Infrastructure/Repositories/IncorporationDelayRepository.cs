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
        return await _context.IncorporationDelays.FirstOrDefaultAsync(i => i.ID == id);
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId)
    {
        return await _context.IncorporationDelays
            .Where(d => _context.Set<IncorpMethodsIncorpDelays>()
                .Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID))
            .ToListAsync();
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByApplicableForAsync(string applicableFor)
    {
        // Fetch delays where ApplicableFor is 'A' (All) or matches the specified filter
        if (applicableFor.ToLower() == "null")
        {
            // Handle special case for NULL values in the ApplicableFor column
            return await _context.IncorporationDelays
                .Where(d => d.ApplicableFor == null)
                .ToListAsync();
        }

        return await _context.IncorporationDelays
            .Where(d => d.ApplicableFor == "A" || d.ApplicableFor == applicableFor)
            .ToListAsync();
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAndApplicableForAsync(int methodId, string applicableFor)
    {
        if (string.IsNullOrWhiteSpace(applicableFor))
        {
            return await _context.IncorporationDelays
                .Where(d => _context.Set<IncorpMethodsIncorpDelays>().Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID))
                .ToListAsync();
        }
        else if (applicableFor.ToLower() == "null")
        {
            return await _context.IncorporationDelays
                .Where(d => _context.Set<IncorpMethodsIncorpDelays>().Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID)
                && d.ApplicableFor == null)
                .ToListAsync();
        }
        else
        {

            return await _context.IncorporationDelays
                .Where(d => _context.Set<IncorpMethodsIncorpDelays>().Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID)
                && d.ApplicableFor == "A" || d.ApplicableFor == applicableFor)
                .ToListAsync();
        }
    }
}
