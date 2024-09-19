using AutoMapper;
using Manner.Application.Calculators;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Intrinsics.Arm;

namespace Manner.Application.Services;
[Service(ServiceLifetime.Transient)]
public class CalculateResultService : ICalculateResultService
{    
    private readonly IClimateRepository _climateRepository;
    private readonly ICropTypeRepository _cropTypeRepository;
    private readonly IManureTypeRepository _manureTypeRepository;
    private readonly IMapper _mapper;
    private readonly IIncorporationMethodRepository _incorporationMethodRepository;
    private readonly IIncorporationDelayRepository _incorporationDelayRepository;
    private readonly ITopSoilRepository _topSoilRepository;
    private readonly ISubSoilRepository _subSoilRepository;
    private readonly IClimateTypeRepository _climateTypeRepository;

    public CalculateResultService(        
        IClimateRepository climateRepository,
        ICropTypeRepository cropTypeRepository,
        IManureTypeRepository manureTypeRepository,
        IMapper mapper,
        IIncorporationMethodRepository incorporationMethodRepository,
        IIncorporationDelayRepository incorporationDelayRepository,
        ITopSoilRepository topSoilRepository,
        ISubSoilRepository subSoilRepository,
        IClimateTypeRepository climateTypeRepository)
    {        
        _climateRepository = climateRepository;
        _cropTypeRepository = cropTypeRepository;
        _manureTypeRepository = manureTypeRepository;
        _mapper = mapper;
        _incorporationMethodRepository = incorporationMethodRepository;
        _incorporationDelayRepository = incorporationDelayRepository;
        _topSoilRepository = topSoilRepository;
        _subSoilRepository = subSoilRepository;
        _climateTypeRepository = climateTypeRepository;
    }

    public async Task<NutrientsResponse> CalculateNutrientsAsync(CalculateNutrientsRequest calculateNutrientsRequest)
    {        
        NutrientsResponse ret = new NutrientsResponse();
        ret.FieldID = calculateNutrientsRequest.Field.FieldID;
        ret.FieldName = calculateNutrientsRequest.Field.FieldName;

        Outputs outputs = new Outputs();

        ClimateDto climate = _mapper.Map<ClimateDto>(await _climateRepository.FetchByPostcodeAsync(calculateNutrientsRequest.Postcode));

        CropTypeDto cropType = _mapper.Map<CropTypeDto>(await _cropTypeRepository.FetchByIdAsync(calculateNutrientsRequest.Field.CropTypeID));
       
        TopSoilDto topSoil = _mapper.Map<TopSoilDto>(await _topSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.TopsoilID));
        SubSoilDto subSoil = _mapper.Map<SubSoilDto>(await _subSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.SubsoilID));
        List<ClimateTypeDto> climateTypes = _mapper.Map<List<ClimateTypeDto>>(await _climateTypeRepository.FetchAllAsync());

        int runType = calculateNutrientsRequest.RunType;

        foreach (var application in calculateNutrientsRequest.ManureApplications)
        {            
            IncorporationDelayDto? incorporationDelay = _mapper.Map<IncorporationDelayDto>(await _incorporationDelayRepository.FetchByIdAsync(application.IncorporationDelayID));
            ManureTypeDto manureType = _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(application.ManureDetails.ManureID));
            manureType.TotalN = application.ManureDetails.TotalN ?? manureType.TotalN;
            manureType.NH4N = application.ManureDetails.NH4N ?? manureType.NH4N;
            manureType.DryMatter = application.ManureDetails.DryMatter ?? manureType.DryMatter;
            manureType.Uric = application.ManureDetails.Uric ?? manureType.Uric;
            manureType.NO3N = application.ManureDetails.NO3N ?? manureType.NO3N;
            manureType.P2O5 = application.ManureDetails.P2O5 ?? manureType.P2O5;
            manureType.K2O = application.ManureDetails.K2O ?? manureType.K2O;
            manureType.SO3 = application.ManureDetails.SO3 ?? manureType.SO3;
            manureType.MgO = application.ManureDetails.MgO ?? manureType.MgO;
            
            MannerCalculator calculator = new MannerCalculator(calculateNutrientsRequest.Field, climate, cropType, application, manureType, incorporationDelay, topSoil, subSoil, climateTypes, runType);
            calculator.Calculate();

            outputs.TotalNitrogenApplied += calculator.MannerEngine.TotalNitrogenApplied;
            outputs.PotentialCropAvailableN += calculator.MannerEngine.PotentialCropAvailableN;
            outputs.NH3NLoss += calculator.MannerEngine.NH3NLoss;
            outputs.N2ONLoss += calculator.MannerEngine.N2ONLoss;
            outputs.N2NLoss += calculator.MannerEngine.N2NLoss;
            outputs.NO3NLoss += calculator.MannerEngine.NO3NLoss;
            outputs.MineralisedN += calculator.MannerEngine.MineralisedN;
            outputs.PotentialEconomicValue += calculator.MannerEngine.PotentialEconomicValue;
            outputs.P2O5CropAvailable += calculator.MannerEngine.P2O5CropAvailable;
            outputs.P2O5Total += calculator.MannerEngine.P2O5Total;
            outputs.K2OCropAvailable += calculator.MannerEngine.K2OCropAvailable;
            outputs.K2OTotal += calculator.MannerEngine.K2OTotal;
            outputs.SO3Total += calculator.MannerEngine.SO3Total;
            outputs.MgOTotal += calculator.MannerEngine.MgOTotal;
            outputs.ResultantNAvailable += calculator.MannerEngine.ResultantNAvailable;
            outputs.ResultantNAvailableSecondCut += calculator.MannerEngine.ResultantNAvailableSecondCut;
            outputs.ResultantNAvailableYear2 += calculator.MannerEngine.ResultantNAvailableYear2;
            outputs.CropUptake += calculator.MannerEngine.CropUptake;
        }
        ret.TotalN = Convert.ToInt32(Math.Round(outputs.TotalNitrogenApplied,0));
        ret.MineralisedN = Convert.ToInt32(Math.Round(outputs.MineralisedN, 0));
        ret.NitrateNLoss = Convert.ToInt32(Math.Round(outputs.NO3NLoss, 0));
        ret.AmmoniaNLoss = Convert.ToInt32(Math.Round(outputs.NH3NLoss, 0));
        ret.DenitrifiedNLoss = Convert.ToInt32(outputs.N2ONLoss + outputs.N2NLoss);
        ret.CurrentCropAvailableN = Convert.ToInt32(Math.Round(outputs.ResultantNAvailable, 0));
        ret.NextGrassNCropCurrentYear = Convert.ToInt32(Math.Round(outputs.ResultantNAvailableSecondCut, 0));
        ret.FollowingCropYear2AvailableN = Convert.ToInt32(Math.Round(outputs.ResultantNAvailableYear2,0));
        ret.NitrogenEfficiencePercentage = Convert.ToInt32(Math.Round((outputs.ResultantNAvailable + outputs.ResultantNAvailableSecondCut) * 100 / outputs.TotalNitrogenApplied, 0));
        ret.TotalP2O5 = Convert.ToInt32(Math.Round(outputs.P2O5Total, 0));
        ret.CropAvailableP2O5 = Convert.ToInt32(Math.Round(outputs.P2O5CropAvailable, 0));
        ret.TotalK2O= Convert.ToInt32(Math.Round(outputs.K2OTotal, 0));
        ret.CropAvailableK2O = Convert.ToInt32(Math.Round(outputs.K2OCropAvailable, 0));
        ret.TotalSO3= Convert.ToInt32(Math.Round(outputs.SO3Total, 0));
        ret.TotalMgO= Convert.ToInt32(Math.Round(outputs.MgOTotal, 0));

        return ret;
    }


}
