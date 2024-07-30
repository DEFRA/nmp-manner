using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class IncorporationMethodRepository : IIncorporationMethodRepository
{
    private readonly ApplicationDbContext _context;
    public IncorporationMethodRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<IncorporationMethod>?> FetchAllAsync()
    {
        return await _context.IncorporationMethods.ToListAsync();
    }

    public async Task<IncorporationMethod?> FetchByIdAsync(int id)
    {
        return await _context.IncorporationMethods.FirstOrDefaultAsync(a => a.ID == id);
    }
}