using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class CropTypeService : ICropTypeService
{
    private readonly ICropTypeRepository _cropTypeRepository;
    public CropTypeService(ICropTypeRepository cropTypeRepository)
    {
        _cropTypeRepository = cropTypeRepository;
    }

    public async Task<IEnumerable<CropType>?> FetchAllAsync()
    {
        return await _cropTypeRepository.FetchAllAsync();
    }

    public async Task<CropType?> FetchByIdAsync(int id)
    {
        return await _cropTypeRepository.FetchByIdAsync(id);
    }
}
