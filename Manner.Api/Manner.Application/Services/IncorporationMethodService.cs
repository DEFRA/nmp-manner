using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class IncorporationMethodService : IIncorporationMethodService
{
    private readonly IIncorporationMethodRepository _incorporationMethodRepository;
    private readonly IMapper _mapper;
    public IncorporationMethodService(IIncorporationMethodRepository incorporationMethodRepository, IMapper mapper)
    {
        _incorporationMethodRepository = incorporationMethodRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IncorporationMethodDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<IncorporationMethodDto>>(await _incorporationMethodRepository.FetchAllAsync());
    }

    public async Task<IncorporationMethodDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<IncorporationMethodDto>(await _incorporationMethodRepository.FetchByIdAsync(id));
    }
}