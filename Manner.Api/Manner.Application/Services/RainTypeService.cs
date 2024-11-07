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
public class RainTypeService(ILogger<RainTypeService> logger, IRainTypeRepository rainTypeRepository, IMapper mapper) : IRainTypeService
{
    private readonly IRainTypeRepository _rainTypeRepository = rainTypeRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RainTypeService> _logger = logger;
    public async Task<IEnumerable<RainTypeDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"RainTypeService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<RainTypeDto>>(await _rainTypeRepository.FetchAllAsync());
    }

    public async Task<RainTypeDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"RainTypeService : FetchByIdAsync({id}) callled");
        return _mapper.Map<RainTypeDto>(await _rainTypeRepository.FetchByIdAsync(id));
    }
}