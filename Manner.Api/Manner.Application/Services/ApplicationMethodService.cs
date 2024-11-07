using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ApplicationMethodService(ILogger<ApplicationMethodService> logger, IApplicationMethodRepository applicationMethodRepository, IMapper mapper) : IApplicationMethodService
{
    private readonly IApplicationMethodRepository _applicationMethodRepository = applicationMethodRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ApplicationMethodService> _logger = logger;

    public async Task<IEnumerable<ApplicationMethodDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"ApplicationMethodService : FetchAllAsync callled");
        return _mapper.Map<IEnumerable<ApplicationMethodDto>>(await _applicationMethodRepository.FetchAllAsync());
    }

    public async Task<ApplicationMethodDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"ApplicationMethodService : FetchByIdAsync({id}) callled");
        return _mapper.Map<ApplicationMethodDto>(await _applicationMethodRepository.FetchByIdAsync(id));
    }

    public async Task<IEnumerable<ApplicationMethodDto>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null)
    {
        _logger.LogTrace($"ApplicationMethodService : FetchByCriteriaAsync({isLiquid}, {fieldType}) callled");
        var methods = await _applicationMethodRepository.FetchByCriteriaAsync(isLiquid, fieldType);
        return _mapper.Map<IEnumerable<ApplicationMethodDto>>(methods);
    }
}
