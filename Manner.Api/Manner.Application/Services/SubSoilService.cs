using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class SubSoilService :ISubSoilService
{
    private readonly ISubSoilRepository _subSoilRepository;
    private readonly IMapper _mapper;
    public SubSoilService(ISubSoilRepository subSoilRepository, IMapper mapper)
    {
        _subSoilRepository = subSoilRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubSoilDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<SubSoilDto>>(await _subSoilRepository.FetchAllAsync());
    }

    public async Task<SubSoilDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<SubSoilDto>(await _subSoilRepository.FetchByIdAsync(id));
    }
}
