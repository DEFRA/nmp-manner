using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class IncorporationDelayRepository(ILogger<IncorporationDelayRepository> logger, ApplicationDbContext applicationDbContext) : IIncorporationDelayRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<IncorporationDelayRepository> _logger = logger;
    public async Task<IEnumerable<IncorporationDelay>?> FetchAllAsync()
    {
        _logger.LogTrace($"IncorporationDelayRepository : FetchAllAsync() callled");
        return await _context.IncorporationDelays.ToListAsync();
    }

    public async Task<IncorporationDelay?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"IncorporationDelayRepository : FetchByIdAsync({id}) callled");
        return await _context.IncorporationDelays.FirstOrDefaultAsync(i => i.ID == id);
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId)
    {
        _logger.LogTrace($"IncorporationDelayRepository : FetchByIncorpMethodIdAsync({methodId}) callled");
        return await _context.IncorporationDelays
            .Where(d => _context.Set<IncorpMethodsIncorpDelays>()
                .Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID))
            .ToListAsync();
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchByApplicableForAsync(string applicableFor)
    {
        _logger.LogTrace($"IncorporationDelayRepository : FetchByApplicableForAsync({applicableFor}) callled");
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
        _logger.LogTrace($"IncorporationDelayRepository : FetchByIncorpMethodIdAndApplicableForAsync({methodId},{applicableFor}) callled");
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
                //.Where(d=>d.ApplicableFor == "A" || d.ApplicableFor == applicableFor)
                
                .Where(d => _context.Set<IncorpMethodsIncorpDelays>().Any(link => link.IncorporationMethodID == methodId && link.IncorporationDelayID == d.ID)
                && (d.ApplicableFor == "A" || d.ApplicableFor == applicableFor))
                .ToListAsync();
        }
    }
}
