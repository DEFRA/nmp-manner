using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureTypeService : IManureTypeService
{
    private readonly IManureTypeRepository _manureTypeRepository;
    public ManureTypeService(IManureTypeRepository manureTypeRepository)
    {
        _manureTypeRepository = manureTypeRepository;
    }

    public async Task<IEnumerable<ManureType>?> FetchAllAsync()
    {
        return await _manureTypeRepository.FetchAllAsync();
    }

    public async Task<ManureType?> FetchByIdAsync(int id)
    {
        return await _manureTypeRepository.FetchByIdAsync(id);
    }
}
