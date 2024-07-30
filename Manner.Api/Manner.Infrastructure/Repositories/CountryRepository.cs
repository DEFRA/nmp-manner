using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;
    public CountryRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<Country>?> FetchAllAsync()
    {
        return await _context.Countries.ToListAsync();
    }

    public async Task<Country?> FetchByIdAsync(int id)
    {
        return await _context.Countries.FirstOrDefaultAsync(a => a.ID == id);
    }
}
