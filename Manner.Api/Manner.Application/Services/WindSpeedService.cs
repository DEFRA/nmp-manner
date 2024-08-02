using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class WindspeedService : IWindspeedService
{
    private readonly IWindspeedRepository _windspeedRepository;
    private readonly IMapper _mapper;
    public WindspeedService(IWindspeedRepository windspeedRepository, IMapper mapper)
    {
        _windspeedRepository = windspeedRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WindspeedDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<WindspeedDto>>(await _windspeedRepository.FetchAllAsync());
    }

    public async Task<WindspeedDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<WindspeedDto>(await _windspeedRepository.FetchByIdAsync(id));
    }
}