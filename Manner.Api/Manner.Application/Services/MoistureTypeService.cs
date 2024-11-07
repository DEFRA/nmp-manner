using Manner.Core.Interfaces;
using Manner.Core.Entities;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Manner.Application.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class MoistureTypeService(ILogger<MoistureTypeService> logger, IMoistureTypeRepository moistureTypeRepository, IMapper mapper) : IMoistureTypeService
{
    private readonly IMoistureTypeRepository _moistureTypeRepository = moistureTypeRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<MoistureTypeService> _logger = logger;
    public async Task<IEnumerable<MoistureTypeDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"MoistureTypeService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<MoistureTypeDto>>(await _moistureTypeRepository.FetchAllAsync());
    }

    public async Task<MoistureTypeDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"MoistureTypeService : FetchByIdAsync({id}) callled");
        return _mapper.Map<MoistureTypeDto>(await _moistureTypeRepository.FetchByIdAsync(id));
    }
}