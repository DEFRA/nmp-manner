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

    public async Task<ClimateDto?> FetchByPostcodeAsync(string postcode)
    {       
        var climate = await _climateRepository.FetchByPostcodeAsync(postcode);
        return _mapper.Map<ClimateDto?>(climate);
    }


    public async Task<ClimateDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ClimateDto>( await _climateRepository.FetchByIdAsync(id));
    }

    public async Task<RainfallPostApplicationResponse> FetchRainfallPostApplication(RainfallPostApplicationRequest rainfallPostApplicationRequest)
    {
        var climate = await _climateRepository.FetchByPostcodeAsync(rainfallPostApplicationRequest.ClimateDataPostcode);

        // Default to 0 mm rainfall if no climate data is found
        RainfallPostApplicationResponse response = new()
        {
            RainfallPostApplication = new Rainfall
            {
                Value = 0,
                Unit = "mm"
            }
        };

        if (climate != null)
        {
            var climateDto = _mapper.Map<ClimateDto>(climate);

            // Calculate rainfall only if climate data is found
            decimal rainfall = _rainfallCalculator.CalculateRainfallPostApplication(
                climateDto,
                rainfallPostApplicationRequest.ApplicationDate,
                rainfallPostApplicationRequest.EndOfSoilDrainageDate
            );

            // Set the calculated rainfall value in the response
            response.RainfallPostApplication.Value = (int)Math.Round(rainfall);
        }

        return response;
    }

    public async Task<Rainfall?> FetchAverageAnualRainfall(string postcode)
    {
        Rainfall? rainfall = null;
        var climate = await _climateRepository.FetchByPostcodeAsync(postcode);
        if(climate != null)
        {
            var meanTotalRainfall = climate.MeanTotalRainFallJan
                + climate.MeanTotalRainFallFeb
                + climate.MeanTotalRainFallMar
                + climate.MeanTotalRainFallApr
                + climate.MeanTotalRainFallMay
                + climate.MeanTotalRainFallJun
                + climate.MeanTotalRainFallJul
                + climate.MeanTotalRainFallAug
                + climate.MeanTotalRainFallSep
                + climate.MeanTotalRainFallOct
                + climate.MeanTotalRainFallNov
                + climate.MeanTotalRainFallDec;
            rainfall = new Rainfall();
            rainfall.Value = Convert.ToInt32(meanTotalRainfall);

        }

        return rainfall;
    }
}
