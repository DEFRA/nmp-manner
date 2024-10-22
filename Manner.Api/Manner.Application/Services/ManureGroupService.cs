using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureGroupService(ILogger<ManureGroupService> logger, IManureGroupRepository manureGroupRepository, IMapper mapper) : IManureGroupService
{
    private readonly IManureGroupRepository _manureGroupRepository = manureGroupRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ManureGroupService> _logger = logger;

    public async Task<IEnumerable<ManureGroupDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"ManureGroupService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<ManureGroupDto>>(await _manureGroupRepository.FetchAllAsync());
    }

    public async Task<ManureGroupDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ManureGroupService : FetchByIdAsync({id}) callled");
        return _mapper.Map<ManureGroupDto>(await _manureGroupRepository.FetchByIdAsync(id));
    }
}

