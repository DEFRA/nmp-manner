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
public class TopSoilService(ILogger<TopSoilService> logger, ITopSoilRepository topSoilRepository, IMapper mapper) : ITopSoilService
{
    private readonly ITopSoilRepository _topSoilRepository = topSoilRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<TopSoilService> _logger = logger;
    public async Task<IEnumerable<TopSoilDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"TopSoilService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<TopSoilDto>>(await _topSoilRepository.FetchAllAsync());
    }

    public async Task<TopSoilDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"TopSoilService : FetchByIdAsync({id}) callled");
        return _mapper.Map<TopSoilDto>(await _topSoilRepository.FetchByIdAsync(id));
    }
}
