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
    public IncorporationMethodService(IIncorporationMethodRepository incorporationMethodRepository)
    {
        _incorporationMethodRepository = incorporationMethodRepository;
    }

    public async Task<IEnumerable<IncorporationMethod>?> FetchAllAsync()
    {
        return await _incorporationMethodRepository.FetchAllAsync();
    }

    public async Task<IncorporationMethod?> FetchByIdAsync(int id)
    {
        return await _incorporationMethodRepository.FetchByIdAsync(id);
    }
}