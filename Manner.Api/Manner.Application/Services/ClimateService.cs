using AutoMapper;
using Manner.Application.Calculators;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ClimateService : IClimateService
{
    private readonly IClimateRepository _climateRepository;
    private readonly IMapper _mapper;
    private readonly IRainfallCalculator _rainfallCalculator;
    public ClimateService(IClimateRepository climateRepository, IMapper mapper, IRainfallCalculator rainfallCalculator)
    {
        _climateRepository = climateRepository;
        _mapper = mapper;
        _rainfallCalculator = rainfallCalculator;
    }

    public Task<IEnumerable<ClimateDto>?> FetchAllAsync()
    {
        //return _mapper.Map<IEnumerable<ClimateDto>>( await _climateRepository.FetchAllAsync());
         throw new NotImplementedException();
    }

    public async Task<(ClimateDto?, List<string>)> FetchByPostcodeAsync(string postcode)
    {
       List<string> errors = new List<string>();


        if(postcode == null)
        {
            errors.Add("Postcode should not be empty");

        }
        if (postcode != null)
        {
            if (postcode.Length < 3)
            {
                errors.Add("Invalid post code. Post code should be 3 or 4 length");
            }
        }

        if(errors.Any())
        {
            return (null, errors);
        }

        var climate = await _climateRepository.FetchByPostcodeAsync(postcode);
        return (_mapper.Map<ClimateDto?>(climate), errors);
    }


    public async Task<ClimateDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ClimateDto>( await _climateRepository.FetchByIdAsync(id));
    }

    public async Task<(EffectiveRainfallResponse?, List<string>)> FetchEffectiveRainFall(EffectiveRainfallRequest effectiveRainfallRequest)
    {
        List<string> errors = new();
        // Validate EndSoilDrainageDate is between 01/01 and 30/04
        if (effectiveRainfallRequest.EndSoilDrainageDate.DayOfYear < 1 || effectiveRainfallRequest.EndSoilDrainageDate.DayOfYear > 120)
        {
            errors.Add($"EndSoilDrainageDate must be between 01/01 and 30/04, but was {effectiveRainfallRequest.EndSoilDrainageDate:dd/MM/yyyy}");
        }

        var climate = await _climateRepository.FetchByPostcodeAsync(effectiveRainfallRequest.Postcode);

        if (climate == null)
        {
            errors.Add("Climate data not found for the given postcode");
        }

        if (errors.Any())
        {
            return (null, errors);
        }


        var climateDto = _mapper.Map<ClimateDto>(climate);

        decimal rainfall = _rainfallCalculator.CalculateRainfallPostApplication(
            climateDto,
            effectiveRainfallRequest.ApplicationDate,
            effectiveRainfallRequest.EndSoilDrainageDate
        );

        var response = new EffectiveRainfallResponse
        {
            EffectiveRainfall = new EffectiveRainfall
            {
                Value = (int)Math.Round(rainfall),
                Unit = "mm"
            }
        };

        return (response, errors);
    }



}
