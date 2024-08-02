using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ClimateService : IClimateService
{
    private readonly IClimateRepository _climateRepository;
    private readonly IMapper _mapper;
    public ClimateService(IClimateRepository climateRepository, IMapper mapper)
    {
        _climateRepository = climateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClimateDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<ClimateDto>>( await _climateRepository.FetchAllAsync());
    }

    public async Task<ClimateDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map< ClimateDto>( await _climateRepository.FetchByIdAsync(id));
    }
}
