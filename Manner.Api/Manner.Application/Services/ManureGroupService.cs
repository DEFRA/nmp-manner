using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureGroupService : IManureGroupService
{
    private readonly IManureGroupRepository _manureGroupRepository;
    private readonly IMapper _mapper;
    public ManureGroupService(IManureGroupRepository manureGroupRepository, IMapper mapper)
    {
        _manureGroupRepository = manureGroupRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ManureGroupDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<ManureGroupDto>>(await _manureGroupRepository.FetchAllAsync());
    }

    public async Task<ManureGroupDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ManureGroupDto>(await _manureGroupRepository.FetchByIdAsync(id));
    }
}

