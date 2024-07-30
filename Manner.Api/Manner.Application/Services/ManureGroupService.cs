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
    public ManureGroupService(IManureGroupRepository manureGroupRepository)
    {
        _manureGroupRepository = manureGroupRepository;
    }

    public async Task<IEnumerable<ManureGroup>?> FetchAllAsync()
    {
        return await _manureGroupRepository.FetchAllAsync();
    }

    public async Task<ManureGroup?> FetchByIdAsync(int id)
    {
        return await _manureGroupRepository.FetchByIdAsync(id);
    }
}

