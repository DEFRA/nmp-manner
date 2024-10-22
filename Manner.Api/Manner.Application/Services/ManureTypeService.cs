using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _logger.LogTrace($"ManureTypeService : FetchAllAsync() callled");
            return _mapper.Map<IEnumerable<ManureTypeDto>>(await _manureTypeRepository.FetchAllAsync());
        }

        public async Task<ManureTypeDto?> FetchByIdAsync(int id)
        {
            _logger.LogTrace($"ManureTypeService : FetchByIdAsync({id}) callled");
            return _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(id));
        }

        public async Task<IEnumerable<ManureTypeDto>?> FetchByCriteriaAsync(
            int? manureGroupId = null,
            int? manureTypeCategoryId = null,
            int? countryId = null,
            bool? highReadilyAvailableNitrogen = null,
            bool? isLiquid = null)
        {
            _logger.LogTrace($"ManureTypeService : FetchByCriteriaAsync({manureGroupId},{manureTypeCategoryId},{countryId},{highReadilyAvailableNitrogen},{isLiquid}) callled");
            var manureTypes = await _manureTypeRepository.FetchByCriteriaAsync(
                manureGroupId,
                manureTypeCategoryId,
                countryId,
                highReadilyAvailableNitrogen,
                isLiquid
            );

            return _mapper.Map<IEnumerable<ManureTypeDto>>(manureTypes);
        }
    }
}
