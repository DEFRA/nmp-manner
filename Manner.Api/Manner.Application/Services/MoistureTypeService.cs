using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Manner.Application.DTOs;
using AutoMapper;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class MoistureTypeService : IMoistureTypeService
{
    private readonly IMoistureTypeRepository _moistureTypeRepository;
    private readonly IMapper _mapper;
    public MoistureTypeService(IMoistureTypeRepository moistureTypeRepository, IMapper mapper)
    {
        _moistureTypeRepository = moistureTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MoistureTypeDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<MoistureTypeDto>>(await _moistureTypeRepository.FetchAllAsync());
    }

    public async Task<MoistureTypeDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<MoistureTypeDto>(await _moistureTypeRepository.FetchByIdAsync(id));
    }
}