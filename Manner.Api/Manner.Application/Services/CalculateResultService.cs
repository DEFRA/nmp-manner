using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;
[Service(ServiceLifetime.Transient)]
public class CalculateResultService : ICalculateResultService
{
    private readonly INutrientsCalculator _nutrientsCalculator;
    private readonly IClimateRepository _climateRepository;
    private readonly ICropTypeRepository _cropTypeRepository;
    private readonly IManureTypeRepository _manureTypeRepository;
    public CalculateResultService(INutrientsCalculator nutrientsCalculator, IClimateRepository climateRepository, ICropTypeRepository cropTypeRepository, IManureTypeRepository manureTypeRepository)
    {
        _nutrientsCalculator = nutrientsCalculator;
        _climateRepository = climateRepository;
        _cropTypeRepository = cropTypeRepository;
        _manureTypeRepository = manureTypeRepository;
    }

    public async Task<NutrientsResponse> CalculateNutrientsAsync(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        NutrientsResponse ret = new NutrientsResponse();
        ret.Field.FieldID = calculateNutrientsRequest.Field.FieldID;
        ret.Field.FieldName = calculateNutrientsRequest.Field.FieldName;

        Climate? climate = await _climateRepository.FetchByPostcodeAsync(calculateNutrientsRequest.Postcode);

        CropType? crop = await _cropTypeRepository.FetchByIdAsync(calculateNutrientsRequest.Field.CropTypeID);
        string cropUse = crop?.Use ?? string.Empty;
        foreach (var application in calculateNutrientsRequest.Applications)
        {
            ManureType? manureType = await _manureTypeRepository.FetchByIdAsync(application.ManureDetails.ManureID);
            if (manureType == null)
            {
                ret.Outputs.Add(_nutrientsCalculator.CalculateNutrientsOutputsValues(application, manureType));

                // Available N
                // --------------------------------------------------------------
                decimal calculatedTotalN = application.ApplicationRate.Value * manureType.TotalN;
                // Readily Available N applied (NH4-N and uric acid N)
                // --------------------------------------------------------------
                // 18 Jan 2013 - Lizzie says "CalcPot = AppRate * (TotalAmmN + TotalUricN + TotalNitrateN)"
               // decimal calculatedPotentialN = application.ApplicationRate.Value * (application.ManureDetails.NH4N + application.ManureDetails.Uric + application.ManureDetails.NO3N);

                // Volatilised N
                // --------------------------------------------------------------
               // decimal calculatedVolatilisedN = this.CalculateAmmoniaVolatilisation(calculatedTotalN, application.ApplicationRate.Value * (manureType.NH4N + manureType.Uric));


            }
            else
            {
                throw new Exception("Manure not found");
            }

        }

        return ret;
    }

    private decimal AvailableNitrogen ()
    {
        return 0;
    }
}
