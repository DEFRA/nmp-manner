
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;

[Repository(ServiceLifetime.Scoped)]
public class ClimateTypeRepository(ILogger<ClimateTypeRepository> logger, ApplicationDbContext applicationDbContext) : IClimateTypeRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<ClimateTypeRepository> _logger = logger;
    public async Task<IEnumerable<ClimateType>?> FetchAllAsync()
    {
        _logger.LogTrace($"ClimateTypeRepository : FetchAllAsync() callled");
        return await _context.ClimateTypes.ToListAsync();
    }

    public async Task<ClimateType?> FetchByIdAsync(int monthNumber)
    {
        _logger.LogTrace($"ClimateTypeRepository : FetchByIdAsync({monthNumber}) callled");
        return await _context.ClimateTypes.FirstOrDefaultAsync(a => a.MonthNumber == monthNumber);
    }

}
