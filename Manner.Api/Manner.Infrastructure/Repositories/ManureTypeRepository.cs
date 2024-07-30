using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ManureTypeRepository : IManureTypeRepository
{
    private readonly ApplicationDbContext _context;
    public ManureTypeRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<ManureType>?> FetchAllAsync()
    {
        return await _context.ManureTypes.ToListAsync();
    }

    public async Task<ManureType?> FetchByIdAsync(int id)
    {
        return await _context.ManureTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}