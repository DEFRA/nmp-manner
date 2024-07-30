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
    public TopSoilService(ITopSoilRepository topSoilRepository)
    {
        _topSoilRepository = topSoilRepository;
    }

    public async Task<IEnumerable<TopSoil>?> FetchAllAsync()
    {
        return await _topSoilRepository.FetchAllAsync();
    }

    public async Task<TopSoil?> FetchByIdAsync(int id)
    {
        return await _topSoilRepository.FetchByIdAsync(id);
    }
}
