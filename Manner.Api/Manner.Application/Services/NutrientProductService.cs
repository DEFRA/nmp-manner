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
public class NutrientProductService(ILogger<NutrientProductService> logger, INutrientProductRepository nutrientProductRepository, IMapper mapper) : INutrientProductService
{
    private readonly INutrientProductRepository _nutrientProductRepository = nutrientProductRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<NutrientProductService> _logger = logger;
    public async Task<IEnumerable<NutrientProductDto>?> FetchAllAsync()
    {
        _logger.LogTrace("NutrientProductService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<NutrientProductDto>>(await _nutrientProductRepository.FetchAllAsync());
    }

    public async Task<NutrientProductDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace("NutrientProductService : FetchByIdAsync({Id}) callled", id);
        return _mapper.Map<NutrientProductDto>(await _nutrientProductRepository.FetchByIdAsync(id));
    }

   
    public async Task<IEnumerable<NutrientProductDto>?> FetchByNutrientIdAsync(int nutrientId)
    {
        _logger.LogTrace("NutrientProductService : FetchByNutrientIdAsync({NutrientId}) callled", nutrientId);
        return _mapper.Map<IEnumerable<NutrientProductDto>>(await _nutrientProductRepository.FetchByNutrientIdAsync(nutrientId));
    }
}
