
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;

[Repository(ServiceLifetime.Scoped)]
public class ClimateTypeRepository : IClimateTypeRepository
{
    private readonly ApplicationDbContext _context;
    public ClimateTypeRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }      

    public async Task<IEnumerable<ClimateType>?> FetchAllAsync()
    {
        return await _context.ClimateTypes.ToListAsync();
    }

    public async Task<ClimateType?> FetchByIdAsync(int monthNumber)
    {
        return await _context.ClimateTypes.FirstOrDefaultAsync(a => a.MonthNumber == monthNumber);
    }

}
