using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class IncorporationDelayService : IIncorporationDelayService
{
    private readonly IIncorporationDelayRepository _incorporationDelayRepository;
    public IncorporationDelayService(IIncorporationDelayRepository incorporationDelayRepository)
    {
        _incorporationDelayRepository = incorporationDelayRepository;
    }

    public async Task<IEnumerable<IncorporationDelay>?> FetchAllAsync()
    {
        return await _incorporationDelayRepository.FetchAllAsync();
    }

    public async Task<IncorporationDelay?> FetchByIdAsync(int id)
    {
        return await _incorporationDelayRepository.FetchByIdAsync(id);
    }
}