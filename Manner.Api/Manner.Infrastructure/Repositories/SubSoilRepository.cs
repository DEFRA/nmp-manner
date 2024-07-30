using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class SubSoilRepository : ISubSoilRepository
{
    private readonly ApplicationDbContext _context;
    public SubSoilRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<SubSoil>?> FetchAllAsync()
    {
        return await _context.SubSoils.ToListAsync();
    }

    public async Task<SubSoil?> FetchByIdAsync(int id)
    {
        return await _context.SubSoils.FirstOrDefaultAsync(a => a.ID == id);
    }
}