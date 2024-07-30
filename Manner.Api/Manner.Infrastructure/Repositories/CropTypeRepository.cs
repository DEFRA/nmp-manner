using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class CropTypeRepository : ICropTypeRepository
{
    private readonly ApplicationDbContext _context;
    public CropTypeRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<CropType>?> FetchAllAsync()
    {
        return await _context.CropTypes.ToListAsync();
    }

    public async Task<CropType?> FetchByIdAsync(int id)
    {
        return await _context.CropTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}