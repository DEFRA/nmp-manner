using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class WindspeedService(ILogger<WindspeedService> logger, IWindspeedRepository windspeedRepository, IMapper mapper) : IWindspeedService
{
    private readonly IWindspeedRepository _windspeedRepository = windspeedRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<WindspeedService> _logger = logger;
    public async Task<IEnumerable<WindspeedDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"WindspeedService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<WindspeedDto>>(await _windspeedRepository.FetchAllAsync());
    }

    public async Task<WindspeedDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"WindspeedService : FetchByIdAsync({id}) callled");
        return _mapper.Map<WindspeedDto>(await _windspeedRepository.FetchByIdAsync(id));
    }
}