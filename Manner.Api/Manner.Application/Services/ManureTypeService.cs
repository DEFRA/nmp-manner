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

        public async Task<ManureTypeDto> CalculateNutrieltsByDryMatterPercentageAsync(ManureTypeDto manureTypeDto)
        {
            CalculateNitrogenContent(manureTypeDto);
            CalculateUricContent(manureTypeDto);
            CalculateNH4NContent(manureTypeDto);
            CalculatePContent(manureTypeDto);            
            CalculateKContent(manureTypeDto);
            CalculateSContent(manureTypeDto);
            CalculateMgContent(manureTypeDto);
            return await Task.FromResult(manureTypeDto);
        }

        private static void CalculateNitrogenContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.TotalN = (0.25 * manureTypeDto.DryMatter) + 1.1;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.TotalN = (0.39 * manureTypeDto.DryMatter) + 2.04;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.TotalN = (0.46 * manureTypeDto.DryMatter) + 0.2;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.TotalN = (0.44 * manureTypeDto.DryMatter) + 3.6;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateUricContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.Uric = ((0.08 * manureTypeDto.DryMatter) + 4.54 - 0.2) * 0.55;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.Uric = ((manureTypeDto.TotalN * 0.35) - 0.2) * 0.4;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateNH4NContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.NH4N = ((58.5 - 2.28 * manureTypeDto.DryMatter) / 100) * manureTypeDto.TotalN;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.NH4N = ((82 - 3.03 * manureTypeDto.DryMatter) / 100) * manureTypeDto.TotalN;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.NH4N = ((0.08 * manureTypeDto.DryMatter) + 4.54 - 0.2) * 0.45;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.NH4N = ((manureTypeDto.TotalN * 0.35) - 0.2) * 0.6;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculatePContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.P2O5 = (0.15 * manureTypeDto.DryMatter) + 0.3;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.P2O5 = (0.36 * manureTypeDto.DryMatter) + 0.04; //Was 0.4 * manureTypeDto.DryMatter + 0.2
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.P2O5 = (0.22 * manureTypeDto.DryMatter) + 3.62; // Was 0.33 * manureTypeDto.DryMatter + 2.45
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.P2O5 = (0.37 * manureTypeDto.DryMatter) + 2.8;
                    }
                    break;
                default:

                    break;
            }
        }

        private static void CalculateKContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.K2O = (0.22 * manureTypeDto.DryMatter) + 1.25; // Was 0.2 * manureTypeDto.DryMatter + 2
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.K2O = (0.2 * manureTypeDto.DryMatter) + 1.44; // Was 0.2 * manureTypeDto.DryMatter + 1.6

                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.K2O = (0.3 * manureTypeDto.DryMatter) + 2.48; // Was 0.27 * manureTypeDto.DryMatter + 0.05
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.K2O = (0.19 * manureTypeDto.DryMatter) + 6.6;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateSContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.SO3 = (0.0875 * manureTypeDto.DryMatter) + 0.15;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.SO3 = (0.125 * manureTypeDto.DryMatter) + 0.47;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.SO3 = (0.13 * manureTypeDto.DryMatter) + 0.39; // Was 0.11 * manureTypeDto.DryMatter + 0.15
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.SO3 = (0.14 * manureTypeDto.DryMatter) - 0.4;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CalculateMgContent(ManureTypeDto manureTypeDto)
        {
            switch (manureTypeDto.ID)
            {
                case (int)ManureTypes.BeefSlurry:
                case (int)ManureTypes.DairySlurry:
                case (int)ManureTypes.CattleSlurry:
                    {
                        manureTypeDto.MgO = (0.0875 * manureTypeDto.DryMatter) + 0.04;
                    }
                    break;
                case (int)ManureTypes.PigSlurry:
                    {
                        manureTypeDto.MgO = (0.15 * manureTypeDto.DryMatter) + 0.1;
                    }
                    break;
                case (int)ManureTypes.PoultryManure:
                    {
                        manureTypeDto.MgO = (0.08 * manureTypeDto.DryMatter) + 1.1;
                    }
                    break;
                case (int)ManureTypes.BroilerTurkeyLitter:
                    {
                        manureTypeDto.MgO = (0.06 * manureTypeDto.DryMatter) + 0.8;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

