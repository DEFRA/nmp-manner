using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class SubSoilService :ISubSoilService
{
    private readonly ISubSoilRepository _subSoilRepository;
    public SubSoilService(ISubSoilRepository subSoilRepository)
    {
        _subSoilRepository = subSoilRepository;
    }

    public async Task<IEnumerable<SubSoil>?> FetchAllAsync()
    {
        return await _subSoilRepository.FetchAllAsync();
    }

    public async Task<SubSoil?> FetchByIdAsync(int id)
    {
        return await _subSoilRepository.FetchByIdAsync(id);
    }
}
