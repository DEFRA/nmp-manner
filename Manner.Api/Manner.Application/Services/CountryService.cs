using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<IEnumerable<Country>?> FetchAllAsync()
    {
        return await _countryRepository.FetchAllAsync();
    }

    public async Task<Country?> FetchByIdAsync(int id)
    {
        return await _countryRepository.FetchByIdAsync(id);
    }
}
