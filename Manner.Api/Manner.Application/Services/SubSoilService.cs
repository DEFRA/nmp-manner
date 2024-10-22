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
public class SubSoilService(ILogger<SubSoilService> logger, ISubSoilRepository subSoilRepository, IMapper mapper) : ISubSoilService
{
    private readonly ISubSoilRepository _subSoilRepository = subSoilRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<SubSoilService> _logger = logger;
    public async Task<IEnumerable<SubSoilDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"SubSoilService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<SubSoilDto>>(await _subSoilRepository.FetchAllAsync());
    }

    public async Task<SubSoilDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"SubSoilService : FetchByIdAsync({id}) callled");
        return _mapper.Map<SubSoilDto>(await _subSoilRepository.FetchByIdAsync(id));
    }
}
