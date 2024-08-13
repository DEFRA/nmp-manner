using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manner.Application.Services
{
    [Service(ServiceLifetime.Transient)]
    public class ManureTypeService : IManureTypeService
    {
        private readonly IManureTypeRepository _manureTypeRepository;
        private readonly IMapper _mapper;

        public ManureTypeService(IManureTypeRepository manureTypeRepository, IMapper mapper)
        {
            _manureTypeRepository = manureTypeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ManureTypeDto>?> FetchAllAsync()
        {
            return _mapper.Map<IEnumerable<ManureTypeDto>>(await _manureTypeRepository.FetchAllAsync());
        }

        public async Task<ManureTypeDto?> FetchByIdAsync(int id)
        {
            return _mapper.Map<ManureTypeDto>(await _manureTypeRepository.FetchByIdAsync(id));
        }

        public async Task<IEnumerable<ManureTypeDto>?> FetchByCriteriaAsync(
            int? manureGroupId = null,
            int? manureTypeCategoryId = null,
            int? countryId = null,
            bool? highReadilyAvailableNitrogen = null,
            bool? isLiquid = null)
        {
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
