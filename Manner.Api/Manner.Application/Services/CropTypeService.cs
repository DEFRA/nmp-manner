using AutoMapper;
using Manner.Application.DTOs;
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
    private readonly IMapper _mapper;
    public CropTypeService(ICropTypeRepository cropTypeRepository, IMapper mapper)
    {
        _cropTypeRepository = cropTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CropTypeDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<CropTypeDto>>(await _cropTypeRepository.FetchAllAsync());
    }

    public async Task<CropTypeDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<CropTypeDto>(await _cropTypeRepository.FetchByIdAsync(id));
    }
}
