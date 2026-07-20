using AutoMapper;
using Manner.Application.Calculators;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.Intrinsics.Arm;

namespace Manner.Application.Services;
[Service(ServiceLifetime.Transient)]
#pragma warning disable S107
public class CalculateResultService(
    ILogger<CalculateResultService> logger,
    IClimateRepository climateRepository,
    ICropTypeRepository cropTypeRepository,
    IManureTypeRepository manureTypeRepository,
    IMapper mapper,
    IIncorporationDelayRepository incorporationDelayRepository,
    ITopSoilRepository topSoilRepository,
    ISubSoilRepository subSoilRepository,
    IClimateTypeRepository climateTypeRepository) : ICalculateResultService
#pragma warning restore S107
{
    private readonly IClimateRepository _climateRepository = climateRepository;
    private readonly ICropTypeRepository _cropTypeRepository = cropTypeRepository;
    private readonly IManureTypeRepository _manureTypeRepository = manureTypeRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IIncorporationDelayRepository _incorporationDelayRepository = incorporationDelayRepository;
    private readonly ITopSoilRepository _topSoilRepository = topSoilRepository;
    private readonly ISubSoilRepository _subSoilRepository = subSoilRepository;
    private readonly IClimateTypeRepository _climateTypeRepository = climateTypeRepository;
    private readonly ILogger<CalculateResultService> _logger = logger;



    public async Task<NutrientsResponse> CalculateNutrientsAsync(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        _logger.LogTrace("CalculateResultService : CalculateNutrientsAsync() called");

        var context = await LoadCalculationContextAsync(calculateNutrientsRequest);
        var totalOutputs = new Outputs();

        foreach (var application in calculateNutrientsRequest.ManureApplications)
        {
            var calculator = await ExecuteCalculationAsync(calculateNutrientsRequest, application, context);
            var applicationOutputs = CreateOutputs(calculator);

            AccumulateOutputs(totalOutputs, applicationOutputs);
        }

        return CreateResponse(totalOutputs, calculateNutrientsRequest);
    }

    private static void AccumulateOutputs(Outputs total, Outputs current)
    {
        total.TotalNitrogenApplied += current.TotalNitrogenApplied;
        total.PotentialCropAvailableN += current.PotentialCropAvailableN;
        total.NH3NLoss += current.NH3NLoss;
        total.N2ONLoss += current.N2ONLoss;
        total.N2NLoss += current.N2NLoss;
        total.NO3NLoss += current.NO3NLoss;
        total.DenitrifiedNLoss += current.DenitrifiedNLoss;
        total.MineralisedN += current.MineralisedN;
        total.PotentialEconomicValue += current.PotentialEconomicValue;
        total.P2O5CropAvailable += current.P2O5CropAvailable;
        total.P2O5Total += current.P2O5Total;
        total.K2OCropAvailable += current.K2OCropAvailable;
        total.K2OTotal += current.K2OTotal;

        if (current.SO3Total.HasValue)
        {
            total.SO3Total = (total.SO3Total ?? 0) + current.SO3Total.Value;
        }

        if (current.SO3CropAvailable.HasValue)
        {
            total.SO3CropAvailable = (total.SO3CropAvailable ?? 0) + current.SO3CropAvailable.Value;
        }

        if (current.MgOTotal.HasValue)
        {
            total.MgOTotal = (total.MgOTotal ?? 0) + current.MgOTotal.Value;
        }

        total.ResultantNAvailable += current.ResultantNAvailable;
        total.ResultantNAvailableSecondCut += current.ResultantNAvailableSecondCut;
        total.ResultantNAvailableYear2 += current.ResultantNAvailableYear2;
        total.CropUptake += current.CropUptake;
    }
    
    private async Task<CalculationContext> LoadCalculationContextAsync(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        return new CalculationContext
        {
            Climate = _mapper.Map<ClimateDto>(await _climateRepository.FetchByPostcodeAsync(calculateNutrientsRequest.Postcode)),
            CropType = _mapper.Map<CropTypeDto>(await _cropTypeRepository.FetchByIdAsync(calculateNutrientsRequest.Field.CropTypeID)),
            TopSoil = _mapper.Map<TopSoilDto>(await _topSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.TopsoilID)),
            SubSoil = _mapper.Map<SubSoilDto>(await _subSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.SubsoilID)),
            ClimateTypes = _mapper.Map<List<ClimateTypeDto>>(await _climateTypeRepository.FetchAllAsync()),
            RunType = calculateNutrientsRequest.RunType
        };
    }

    private async Task<ManureTypeDto> BuildManureTypeAsync(ManureApplication application)
    {
        var manure = _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(application.ManureDetails.ManureID));

        manure.TotalN = application.ManureDetails.TotalN ?? manure.TotalN;
        manure.NH4N = application.ManureDetails.NH4N ?? manure.NH4N;
        manure.DryMatter = application.ManureDetails.DryMatter ?? manure.DryMatter;
        manure.Uric = application.ManureDetails.Uric ?? manure.Uric;
        manure.NO3N = application.ManureDetails.NO3N ?? manure.NO3N;
        manure.P2O5 = application.ManureDetails.P2O5 ?? manure.P2O5;
        manure.K2O = application.ManureDetails.K2O ?? manure.K2O;
        manure.SO3 = application.ManureDetails.SO3 ?? manure.SO3;
        manure.MgO = application.ManureDetails.MgO ?? manure.MgO;

        return manure;
    }

    private async Task<MannerCalculator> ExecuteCalculationAsync( CalculateNutrientsRequest calculateNutrientsRequest, ManureApplication application, CalculationContext context)
    {
        var incorporationDelay = _mapper.Map<IncorporationDelayDto>(await _incorporationDelayRepository.FetchByIdAsync(application.IncorporationDelayID));

        var manureType = await BuildManureTypeAsync(application);

        var calculator = new MannerCalculator(new MannerCalculatorInput
        {
            Field = calculateNutrientsRequest.Field,
            Climate = context.Climate,
            CropType = context.CropType,
            ManureApplication = application,
            ManureType = manureType,
            IncorporationDelay = incorporationDelay,
            TopSoil = context.TopSoil,
            SubSoil = context.SubSoil,
            ClimateTypes = context.ClimateTypes,
            RunType = context.RunType
        });

        calculator.Calculate();

        return calculator;
    }

    private Outputs CreateOutputs(MannerCalculator calculator)
    {
        return new Outputs
        {
            TotalNitrogenApplied = calculator.MannerEngine.TotalNitrogenApplied,
            PotentialCropAvailableN = calculator.MannerEngine.PotentialCropAvailableN,
            NH3NLoss = calculator.MannerEngine.NH3NLoss,
            N2ONLoss = calculator.MannerEngine.N2ONLoss,
            N2NLoss = calculator.MannerEngine.N2NLoss,
            NO3NLoss = calculator.MannerEngine.NO3NLoss,
            DenitrifiedNLoss = calculator.MannerEngine.N2ONLoss + calculator.MannerEngine.N2NLoss,
            MineralisedN = calculator.MannerEngine.MineralisedN,
            PotentialEconomicValue = calculator.MannerEngine.PotentialEconomicValue,
            P2O5CropAvailable = calculator.MannerEngine.P2O5CropAvailable,
            P2O5Total = calculator.MannerEngine.P2O5Total,
            K2OCropAvailable = calculator.MannerEngine.K2OCropAvailable,
            K2OTotal = calculator.MannerEngine.K2OTotal,
            SO3Total = calculator.MannerEngine.SO3Total is null
                ? null
                : calculator.MannerEngine.SO3Total.Value,
            SO3CropAvailable = calculator.MannerEngine.SO3CropAvailable is null
                ? null
                : calculator.MannerEngine.SO3CropAvailable.Value,
            MgOTotal = calculator.MannerEngine.MgOTotal is null
                ? null
                : calculator.MannerEngine.MgOTotal.Value,
            ResultantNAvailable = calculator.MannerEngine.ResultantNAvailable,// Math.Round(calculator.MannerEngine.ResultantNAvailable),
            ResultantNAvailableSecondCut = calculator.MannerEngine.ResultantNAvailableSecondCut,// Math.Round(calculator.MannerEngine.ResultantNAvailableSecondCut),
            ResultantNAvailableYear2 = calculator.MannerEngine.ResultantNAvailableYear2,// Math.Round(calculator.MannerEngine.ResultantNAvailableYear2),
            CropUptake = calculator.MannerEngine.CropUptake
        };
    }

    private NutrientsResponse CreateResponse(Outputs output, CalculateNutrientsRequest request)
    {
        return new NutrientsResponse
        {
            FieldID = request.Field.FieldID,
            FieldName = request.Field.FieldName,
            TotalN = (int)Math.Round(output.TotalNitrogenApplied),
            MineralisedN = (int)Math.Round(output.MineralisedN),
            NitrateNLoss = (int)Math.Round(output.NO3NLoss),
            AmmoniaNLoss = (int)Math.Round(output.NH3NLoss),
            DenitrifiedNLoss = (int)Math.Round(output.DenitrifiedNLoss),
            CurrentCropAvailableN = (int)Math.Round(output.ResultantNAvailable),
            NextGrassNCropCurrentYear = (int)Math.Round(output.ResultantNAvailableSecondCut),
            FollowingCropYear2AvailableN = (int)Math.Round(output.ResultantNAvailableYear2),
            NitrogenEfficiencePercentage = CalculateNitrogenEfficiency(output, request.Field.CropTypeID == 1),
            TotalP2O5 = (int)Math.Round(output.P2O5Total),
            CropAvailableP2O5 = (int)Math.Round(output.P2O5CropAvailable),
            TotalK2O = (int)Math.Round(output.K2OTotal),
            CropAvailableK2O = (int)Math.Round(output.K2OCropAvailable),
            TotalSO3 = output.SO3Total.HasValue ? (int)Math.Round(output.SO3Total.Value) : 0,
            CropAvailableSO3 = output.SO3CropAvailable.HasValue ? (int)Math.Round(output.SO3CropAvailable.Value) : null,
            TotalMgO = output.MgOTotal.HasValue ? (int)Math.Round(output.MgOTotal.Value) : 0
        };
    }

    private static int CalculateNitrogenEfficiency(Outputs output, bool isGrass)
    {
        if (Math.Abs(output.TotalNitrogenApplied) < 0.0001)
            return 0;

        var available = isGrass
            ? output.ResultantNAvailable + output.ResultantNAvailableSecondCut
            : output.ResultantNAvailable;

        return (int)Math.Round(available * 100 / output.TotalNitrogenApplied);
    }

    public async Task<List<NutrientsResponse>> CalculateNutrientsIndivisualApplicationsAsync(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        var context = await LoadCalculationContextAsync(calculateNutrientsRequest);
        var responses = new List<NutrientsResponse>();

        foreach (var application in calculateNutrientsRequest.ManureApplications)
        {
            var calculator = await ExecuteCalculationAsync(calculateNutrientsRequest, application, context);

            var outputs = CreateOutputs(calculator);

            responses.Add(CreateResponse(outputs, calculateNutrientsRequest));
        }

        return responses;
    }    
}
