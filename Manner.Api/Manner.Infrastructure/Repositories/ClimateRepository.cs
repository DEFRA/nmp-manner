
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ClimateRepository(ILogger<ClimateRepository> logger, ApplicationDbContext applicationDbContext) : IClimateRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<ClimateRepository> _logger = logger;
    public async Task<Climate?> FetchByPostcodeAsync(string postcode)
    {
        _logger.LogTrace($"ClimateRepository : FetchByPostcodeAsync({postcode}) callled");
        return await _context.Climates.FirstOrDefaultAsync(c=>c.PostCode == postcode);
    }

    public async Task<IEnumerable<Climate>?> FetchAllAsync()
    {
        _logger.LogTrace($"ClimateRepository : FetchAllAsync() callled");
        return await _context.Climates.ToListAsync();
    }

    public async Task<Climate?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ClimateRepository : FetchByIdAsync({id}) callled");
        return await _context.Climates.FirstOrDefaultAsync(a => a.ID == id);
    }
}
