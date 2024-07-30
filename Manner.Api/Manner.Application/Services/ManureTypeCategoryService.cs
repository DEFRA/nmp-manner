using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureTypeCategoryService : IManureTypeCategoryService
{
    private readonly IManureTypeCategoryRepository _manureTypeCategoryRepository;
    public ManureTypeCategoryService(IManureTypeCategoryRepository manureTypeCategoryRepository)
    {
        _manureTypeCategoryRepository = manureTypeCategoryRepository;
    }

    public async Task<IEnumerable<ManureTypeCategory>?> FetchAllAsync()
    {
        return await _manureTypeCategoryRepository.FetchAllAsync();
    }

    public async Task<ManureTypeCategory?> FetchByIdAsync(int id)
    {
        return await _manureTypeCategoryRepository.FetchByIdAsync(id);
    }
}