using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ClimateService : IClimateService
{
    private readonly IClimateRepository _climateRepository;
    public ClimateService(IClimateRepository climateRepository)
    {
        _climateRepository = climateRepository;
    }

    public async Task<IEnumerable<Climate>?> FetchAllAsync()
    {
        return await _climateRepository.FetchAllAsync();
    }

    public async Task<Climate?> FetchByIdAsync(int id)
    {
        return await _climateRepository.FetchByIdAsync(id);
    }
}
