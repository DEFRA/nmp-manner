using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class CountryRepository(ILogger<CountryRepository> logger, ApplicationDbContext applicationDbContext) : ICountryRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<CountryRepository> _logger = logger;
    public async Task<IEnumerable<Country>?> FetchAllAsync()
    {
        _logger.LogTrace($"CountryRepository : FetchAllAsync() callled");
        return await _context.Countries.ToListAsync();
    }

    public async Task<Country?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"CountryRepository : FetchByIdAsync({id}) callled");
        return await _context.Countries.FirstOrDefaultAsync(a => a.ID == id);
    }
}
