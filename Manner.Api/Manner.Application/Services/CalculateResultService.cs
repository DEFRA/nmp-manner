using AutoMapper;
using Manner.Application.Calculators;
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
        ret.Field.FieldID = calculateNutrientsRequest.Field.FieldID;
        ret.Field.FieldName = calculateNutrientsRequest.Field.FieldName;

        ClimateDto climate = _mapper.Map<ClimateDto>(await _climateRepository.FetchByPostcodeAsync(calculateNutrientsRequest.Postcode));

        CropTypeDto cropType = _mapper.Map<CropTypeDto>(await _cropTypeRepository.FetchByIdAsync(calculateNutrientsRequest.Field.CropTypeID));
       
        TopSoilDto topSoil = _mapper.Map<TopSoilDto>(await _topSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.TopsoilID));
        SubSoilDto subSoil = _mapper.Map<SubSoilDto>(await _subSoilRepository.FetchByIdAsync(calculateNutrientsRequest.Field.SubsoilID));
        List<ClimateTypeDto> climateTypes = _mapper.Map<List<ClimateTypeDto>>(await _climateTypeRepository.FetchAllAsync());

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
            
            MannerCalculator calculator = new MannerCalculator(calculateNutrientsRequest.Field, climate, cropType, application, manureType, incorporationDelay, topSoil, subSoil, climateTypes);
            calculator.Calculate();

            ret.Outputs.TotalNitrogenApplied += calculator.MannerEngine.TotalNitrogenApplied;
            ret.Outputs.PotentialCropAvailableN += calculator.MannerEngine.PotentialCropAvailableN;
            ret.Outputs.NH3NLoss += calculator.MannerEngine.NH3NLoss;
            ret.Outputs.N2ONLoss += calculator.MannerEngine.N2ONLoss;
            ret.Outputs.N2NLoss += calculator.MannerEngine.N2NLoss;
            ret.Outputs.NO3NLoss += calculator.MannerEngine.NO3NLoss;
            ret.Outputs.MineralisedN += calculator.MannerEngine.MineralisedN;
            ret.Outputs.PotentialEconomicValue += calculator.MannerEngine.PotentialEconomicValue;
            ret.Outputs.P2O5CropAvailable += calculator.MannerEngine.P2O5CropAvailable;
            ret.Outputs.P2O5Total += calculator.MannerEngine.P2O5Total;
            ret.Outputs.K2OCropAvailable += calculator.MannerEngine.K2OCropAvailable;
            ret.Outputs.K2OTotal += calculator.MannerEngine.K2OTotal;
            ret.Outputs.SO3Total += calculator.MannerEngine.SO3Total;
            ret.Outputs.MgOTotal += calculator.MannerEngine.MgOTotal;                   
            ret.Outputs.ResultantNAvailable += calculator.MannerEngine.ResultantNAvailable;
            ret.Outputs.ResultantNAvailableSecondCut += calculator.MannerEngine.ResultantNAvailableSecondCut;
            ret.Outputs.ResultantNAvailableYear2 += calculator.MannerEngine.ResultantNAvailableYear2;                        
            ret.Outputs.CropUptake += calculator.MannerEngine.CropUptake;
        }

        return ret;
    }


}
