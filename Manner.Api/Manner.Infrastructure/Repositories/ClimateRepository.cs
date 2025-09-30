
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
        //var priorities = Enumerable.Range(2, postcode.Length - 1)   // from length 2 up to full length
        //                   .Select(len => postcode.Substring(0, len))
        //                   .ToList();
        //var climate = (await _context.Climates
        //                .Where(c => priorities.Contains(c.PostCode))
        //                .ToListAsync())                          // fetch matching rows
        //                .OrderByDescending(c => priorities.IndexOf(c.PostCode)) // order in memory
        //                .FirstOrDefault();
        //return climate;
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
