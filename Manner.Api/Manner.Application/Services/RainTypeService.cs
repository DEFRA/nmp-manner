using AutoMapper;
using Manner.Application.DTOs;
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
    private readonly IMapper _mapper;
    public RainTypeService(IRainTypeRepository rainTypeRepository, IMapper mapper)
    {
        _rainTypeRepository = rainTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RainTypeDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<RainTypeDto>>(await _rainTypeRepository.FetchAllAsync());
    }

    public async Task<RainTypeDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<RainTypeDto>(await _rainTypeRepository.FetchByIdAsync(id));
    }
}