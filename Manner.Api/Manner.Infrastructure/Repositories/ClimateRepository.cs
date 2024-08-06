
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ClimateRepository : IClimateRepository
{
    private readonly ApplicationDbContext _context;
    public ClimateRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<Climate?> FetchByPostcodeAsync(string postcode)
    {
        return await _context.Climates.FirstOrDefaultAsync(c=>c.PostCode == postcode);
    }

    public async Task<IEnumerable<Climate>?> FetchAllAsync()
    {
        return await _context.Climates.ToListAsync();
    }

    public async Task<Climate?> FetchByIdAsync(int id)
    {
        return await _context.Climates.FirstOrDefaultAsync(a => a.ID == id);
    }
}
