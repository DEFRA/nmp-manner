using Manner.Core.Interfaces;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Manner.Application.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ManureTypeCategoryService(ILogger<ManureTypeCategoryService> logger, IManureTypeCategoryRepository manureTypeCategoryRepository, IMapper mapper) : IManureTypeCategoryService
{
    private readonly IManureTypeCategoryRepository _manureTypeCategoryRepository = manureTypeCategoryRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ManureTypeCategoryService> _logger = logger;

    public async Task<IEnumerable<ManureTypeCategoryDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"ManureTypeCategoryService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<ManureTypeCategoryDto>>(await _manureTypeCategoryRepository.FetchAllAsync());
    }

    public async Task<ManureTypeCategoryDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ManureTypeCategoryService : FetchByIdAsync({id}) callled");
        return _mapper.Map<ManureTypeCategoryDto>(await _manureTypeCategoryRepository.FetchByIdAsync(id));
    }
}