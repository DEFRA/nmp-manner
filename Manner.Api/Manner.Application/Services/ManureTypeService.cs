using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Enums;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Manner.Application.Enums.Enumerations;

namespace Manner.Application.Services
{
    [Service(ServiceLifetime.Transient)]
    public class ManureTypeService(ILogger<ManureTypeService> logger, IManureTypeRepository manureTypeRepository, IMapper mapper) : IManureTypeService
    {
        private readonly IManureTypeRepository _manureTypeRepository = manureTypeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ManureTypeService> _logger = logger;
        public async Task<IEnumerable<ManureTypeDto>?> FetchAllAsync()
        {
            _logger.LogTrace("ManureTypeService : FetchAllAsync() callled");
            return _mapper.Map<IEnumerable<ManureTypeDto>>(await _manureTypeRepository.FetchAllAsync());
        }

        public async Task<ManureTypeDto?> FetchByIdAsync(int id)
        {
            _logger.LogTrace("ManureTypeService : FetchByIdAsync({Id}) callled", id);
            return _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(id));
        }

        public async Task<IEnumerable<ManureTypeDto>?> FetchByCriteriaAsync(
            int? manureGroupId = null,
            int? manureTypeCategoryId = null,
            int? countryId = null,
            bool? highReadilyAvailableNitrogen = null,
            bool? isLiquid = null)
        {
            _logger.LogTrace("ManureTypeService : FetchByCriteriaAsync({ManureGroupId},{ManureTypeCategoryId},{CountryId},{HighReadilyAvailableNitrogen},{IsLiquid}) callled", manureGroupId, manureTypeCategoryId, countryId, highReadilyAvailableNitrogen, isLiquid);
            var manureTypes = await _manureTypeRepository.FetchByCriteriaAsync(
                manureGroupId,
                manureTypeCategoryId,
                countryId,
                highReadilyAvailableNitrogen,
                isLiquid
            );

            return _mapper.Map<IEnumerable<ManureTypeDto>>(manureTypes);
        }

        public async Task<ManureNutrientsDto> CalculateNutrieltsByDryMatterPercentageAsync(ManureNutrientsDto manureNutrientsDto)
        {
            CalculateNitrogenContent(manureNutrientsDto);
            CalculateUricContent(manureNutrientsDto);
            CalculateNH4NContent(manureNutrientsDto);
            CalculatePContent(manureNutrientsDto);            
            CalculateKContent(manureNutrientsDto);
            CalculateSContent(manureNutrientsDto);
            CalculateMgContent(manureNutrientsDto);
            return await Task.FromResult(manureNutrientsDto);
        }

        private static void CalculateNitrogenContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.TotalN = (0.25m * manureNutrientsDto.DryMatter) + 1.1m;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.TotalN = (0.39m * manureNutrientsDto.DryMatter) + 2.04m;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.TotalN = (0.46m * manureNutrientsDto.DryMatter) + 0.2m;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.TotalN = (0.44m * manureNutrientsDto.DryMatter) + 3.6m;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateUricContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.Uric = ((0.08m * manureNutrientsDto.DryMatter) + 4.54m - 0.2m) * 0.55m;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.Uric = ((manureNutrientsDto.TotalN * 0.35m) - 0.2m) * 0.4m;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateNH4NContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.NH4N = ((58.5m - 2.28m * manureNutrientsDto.DryMatter) / 100m) * manureNutrientsDto.TotalN;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.NH4N = ((82m - 3.03m * manureNutrientsDto.DryMatter) / 100m) * manureNutrientsDto.TotalN;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.NH4N = ((0.08m * manureNutrientsDto.DryMatter) + 4.54m - 0.2m) * 0.45m;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.NH4N = ((manureNutrientsDto.TotalN * 0.35m) - 0.2m) * 0.6m;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculatePContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.P2O5 = (0.15m * manureNutrientsDto.DryMatter) + 0.3m;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.P2O5 = (0.36m * manureNutrientsDto.DryMatter) + 0.04m; //Was 0.4 * manureNutrientsDto.DryMatter + 0.2
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.P2O5 = (0.22m * manureNutrientsDto.DryMatter) + 3.62m; // Was 0.33 * manureNutrientsDto.DryMatter + 2.45
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.P2O5 = (0.37m * manureNutrientsDto.DryMatter) + 2.8m;
                    }
                    break;
                default:

                    break;
            }
        }

        private static void CalculateKContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.K2O = (0.22m * manureNutrientsDto.DryMatter) + 1.25m; // Was 0.2 * manureNutrientsDto.DryMatter + 2
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.K2O = (0.2m * manureNutrientsDto.DryMatter) + 1.44m; // Was 0.2 * manureNutrientsDto.DryMatter + 1.6

                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.K2O = (0.3m * manureNutrientsDto.DryMatter) + 2.48m; // Was 0.27 * manureNutrientsDto.DryMatter + 0.05
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.K2O = (0.19m * manureNutrientsDto.DryMatter) + 6.6m;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateSContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.SO3 = (0.0875m * manureNutrientsDto.DryMatter) + 0.15m;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.SO3 = (0.125m * manureNutrientsDto.DryMatter) + 0.47m;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.SO3 = (0.13m * manureNutrientsDto.DryMatter) + 0.39m; // Was 0.11 * manureNutrientsDto.DryMatter + 0.15
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.SO3 = (0.14m * manureNutrientsDto.DryMatter) - 0.4m;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateMgContent(ManureNutrientsDto manureNutrientsDto)
        {
            switch (manureNutrientsDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureNutrientsDto.MgO = (0.0875m * manureNutrientsDto.DryMatter) + 0.04m;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureNutrientsDto.MgO = (0.15m * manureNutrientsDto.DryMatter) + 0.1m;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureNutrientsDto.MgO = (0.08m * manureNutrientsDto.DryMatter) + 1.1m;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureNutrientsDto.MgO = (0.06m * manureNutrientsDto.DryMatter) + 0.8m;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

