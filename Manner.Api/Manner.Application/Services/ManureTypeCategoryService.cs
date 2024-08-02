using Manner.Core.Interfaces;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Manner.Application.DTOs;
using AutoMapper;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureTypeCategoryService : IManureTypeCategoryService
{
    private readonly IManureTypeCategoryRepository _manureTypeCategoryRepository;
    private readonly IMapper _mapper;
    public ManureTypeCategoryService(IManureTypeCategoryRepository manureTypeCategoryRepository, IMapper mapper)
    {
        _manureTypeCategoryRepository = manureTypeCategoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ManureTypeCategoryDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<ManureTypeCategoryDto>>(await _manureTypeCategoryRepository.FetchAllAsync());
    }

    public async Task<ManureTypeCategoryDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ManureTypeCategoryDto>(await _manureTypeCategoryRepository.FetchByIdAsync(id));
    }
}