using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Repositories;
[Repository(ServiceLifetime.Scoped)]
public class ApplicationMethodRepository : IApplicationMethodRepository
{
    private readonly ApplicationDbContext _context;
    public ApplicationMethodRepository(ApplicationDbContext applicationDbContext) 
    {
        _context = applicationDbContext;
    }

    public async Task<IEnumerable<ApplicationMethod>?> FetchAllAsync()
    {
        return await _context.ApplicationMethods.ToListAsync();
    }

    public async Task<ApplicationMethod?> FetchByIdAsync(int id)
    {
        return await _context.ApplicationMethods.FirstOrDefaultAsync(a => a.ID == id);
    }
}
