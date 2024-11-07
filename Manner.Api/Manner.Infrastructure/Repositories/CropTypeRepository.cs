using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class CropTypeRepository(ILogger<CropTypeRepository> logger, ApplicationDbContext applicationDbContext) : ICropTypeRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<CropTypeRepository> _logger = logger;
    public async Task<IEnumerable<CropType>?> FetchAllAsync()
    {
        _logger.LogTrace($"CropTypeRepository : FetchAllAsync() callled");
        return await _context.CropTypes.ToListAsync();
    }

    public async Task<CropType?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"CropTypeRepository : FetchByIdAsync({id}) callled");
        return await _context.CropTypes.FirstOrDefaultAsync(a => a.ID == id);
    }
}