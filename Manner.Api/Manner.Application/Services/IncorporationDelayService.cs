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
public class IncorporationDelayService(ILogger<IncorporationDelayService> logger, IIncorporationDelayRepository incorporationDelayRepository, IMapper mapper) : IIncorporationDelayService
{
    private readonly IIncorporationDelayRepository _incorporationDelayRepository = incorporationDelayRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<IncorporationDelayService> _logger = logger;

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"IncorporationDelayService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(await _incorporationDelayRepository.FetchAllAsync());
    }

    public async Task<IncorporationDelayDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"IncorporationDelayService : FetchByIdAsync({id}) callled");
        return _mapper.Map<IncorporationDelayDto>(await _incorporationDelayRepository.FetchByIdAsync(id));
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAsync(int methodId)
    {
        _logger.LogTrace($"IncorporationDelayService : FetchByIncorpMethodIdAsync({methodId}) callled");
        var delays = await _incorporationDelayRepository.FetchByIncorpMethodIdAsync(methodId);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchByApplicableForAsync(string applicableFor)
    {
        _logger.LogTrace($"IncorporationDelayService : FetchByApplicableForAsync({applicableFor}) callled");
        var delays = await _incorporationDelayRepository.FetchByApplicableForAsync(applicableFor);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAndApplicableForAsync(int methodId, string applicableFor)
    {
        _logger.LogTrace($"IncorporationDelayService : FetchByIncorpMethodIdAndApplicableForAsync({methodId},{applicableFor}) callled");
        var delays = await _incorporationDelayRepository.FetchByIncorpMethodIdAndApplicableForAsync(methodId,applicableFor);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }
}