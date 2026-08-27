using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class NutrientService(ILogger<NutrientService> logger, INutrientRepository nutrientRepository, IMapper mapper) : INutrientService
{
    private readonly INutrientRepository _nutrientRepository = nutrientRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<NutrientService> _logger = logger;
    public async Task<IEnumerable<NutrientDto>?> FetchAllAsync()
    {
        _logger.LogTrace("NutrientService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<NutrientDto>>(await _nutrientRepository.FetchAllAsync());
    }

    public async Task<NutrientDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace("NutrientService : FetchByIdAsync({Id}) callled", id);
        return _mapper.Map<NutrientDto>(await _nutrientRepository.FetchByIdAsync(id));
    }
}
