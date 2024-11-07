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
public class CountryService(ILogger<CountryService> logger, ICountryRepository countryRepository, IMapper mapper) : ICountryService
{
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CountryService> _logger = logger;

    public async Task<IEnumerable<CountryDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"CountryService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<CountryDto>>(await _countryRepository.FetchAllAsync());
    }

    public async Task<CountryDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"CountryService : FetchByIdAsync({id}) callled");
        return _mapper.Map<CountryDto>(await _countryRepository.FetchByIdAsync(id));
    }
}
