using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class MoistureTypeRepository(ILogger<MoistureTypeRepository> logger, ApplicationDbContext applicationDbContext) : IMoistureTypeRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<MoistureTypeRepository> _logger = logger;
    public async Task<IEnumerable<MoistureType>?> FetchAllAsync()
    {
        _logger.LogTrace($"MoistureTypeRepository : FetchAllAsync() callled");
        return await _context.MoistureTypes.ToListAsync();
    }

    public async Task<MoistureType?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"MoistureTypeRepository : FetchByIdAsync({id}) callled");
        return await _context.MoistureTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}