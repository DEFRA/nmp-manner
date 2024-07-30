using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class RainTypeService : IRainTypeService
{
    private readonly IRainTypeRepository _rainTypeRepository;
    public RainTypeService(IRainTypeRepository rainTypeRepository)
    {
        _rainTypeRepository = rainTypeRepository;
    }

    public async Task<IEnumerable<RainType>?> FetchAllAsync()
    {
        return await _rainTypeRepository.FetchAllAsync();
    }

    public async Task<RainType?> FetchByIdAsync(int id)
    {
        return await _rainTypeRepository.FetchByIdAsync(id);
    }
}