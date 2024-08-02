using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Manner.Application.DTOs;
using AutoMapper;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureTypeService : IManureTypeService
{
    private readonly IManureTypeRepository _manureTypeRepository;
    private readonly IMapper _mapper;
    public ManureTypeService(IManureTypeRepository manureTypeRepository, IMapper mapper)
    {
        _manureTypeRepository = manureTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ManureTypeDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<ManureTypeDto>>(await _manureTypeRepository.FetchAllAsync());
    }

    public async Task<ManureTypeDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(id));
    }
}
