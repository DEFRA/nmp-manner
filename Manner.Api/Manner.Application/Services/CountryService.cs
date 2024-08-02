using AutoMapper;
using Manner.Application.DTOs;
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
    private readonly IMapper _mapper;
    public CountryService(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CountryDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<CountryDto>>(await _countryRepository.FetchAllAsync());
    }

    public async Task<CountryDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<CountryDto>(await _countryRepository.FetchByIdAsync(id));
    }
}
