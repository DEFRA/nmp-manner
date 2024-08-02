using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class TopSoilService: ITopSoilService
{
    private readonly ITopSoilRepository _topSoilRepository;
    private readonly IMapper _mapper;
    public TopSoilService(ITopSoilRepository topSoilRepository, IMapper mapper)
    {
        _topSoilRepository = topSoilRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TopSoilDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<TopSoilDto>>(await _topSoilRepository.FetchAllAsync());
    }

    public async Task<TopSoilDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<TopSoilDto>(await _topSoilRepository.FetchByIdAsync(id));
    }
}
