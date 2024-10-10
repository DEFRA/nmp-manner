using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class IncorporationDelayService : IIncorporationDelayService
{
    private readonly IIncorporationDelayRepository _incorporationDelayRepository;
    private readonly IMapper _mapper;
    public IncorporationDelayService(IIncorporationDelayRepository incorporationDelayRepository, IMapper mapper)
    {
        _incorporationDelayRepository = incorporationDelayRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(await _incorporationDelayRepository.FetchAllAsync());
    }

    public async Task<IncorporationDelayDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<IncorporationDelayDto>(await _incorporationDelayRepository.FetchByIdAsync(id));
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAsync(int methodId)
    {
        var delays = await _incorporationDelayRepository.FetchByIncorpMethodIdAsync(methodId);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }

    public async Task<IEnumerable<IncorporationDelayDto>?> FetchByApplicableForAsync(string applicableFor)
    {
        var delays = await _incorporationDelayRepository.FetchByApplicableForAsync(applicableFor);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }

    public Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAndApplicableForAsync(int methodId, string applicableFor)
    {
        var delays = await _incorporationDelayRepository.FetchByIncorpMethodIdAndApplicableForAsync(methodId,applicableFor);
        return _mapper.Map<IEnumerable<IncorporationDelayDto>>(delays);
    }
}