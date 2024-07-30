using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class WindSpeedService : IWindSpeedService
{
    private readonly IWindSpeedRepository _windSpeedRepository;
    public WindSpeedService(IWindSpeedRepository windSpeedRepository)
    {
        _windSpeedRepository = windSpeedRepository;
    }

    public async Task<IEnumerable<WindSpeed>?> FetchAllAsync()
    {
        return await _windSpeedRepository.FetchAllAsync();
    }

    public async Task<WindSpeed?> FetchByIdAsync(int id)
    {
        return await _windSpeedRepository.FetchByIdAsync(id);
    }
}