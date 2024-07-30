using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class MoistureTypeService : IMoistureTypeService
{
    private readonly IMoistureTypeRepository _moistureTypeRepository;
    public MoistureTypeService(IMoistureTypeRepository moistureTypeRepository)
    {
        _moistureTypeRepository = moistureTypeRepository;
    }

    public async Task<IEnumerable<MoistureType>?> FetchAllAsync()
    {
        return await _moistureTypeRepository.FetchAllAsync();
    }

    public async Task<MoistureType?> FetchByIdAsync(int id)
    {
        return await _moistureTypeRepository.FetchByIdAsync(id);
    }
}