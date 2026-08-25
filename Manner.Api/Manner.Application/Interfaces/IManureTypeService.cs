using Manner.Application.DTOs;

namespace Manner.Application.Interfaces;

public interface IManureTypeService : IService<ManureTypeDto>
{
    Task<ManureNutrientsDto> CalculateNutrieltsByDryMatterPercentageAsync(ManureNutrientsDto manureNutrientsDto);
    Task<IEnumerable<ManureTypeDto>?> FetchByCriteriaAsync(int? manureGroupId = null, int? manureTypeCategoryId = null, int? countryId = null, bool? highReadilyAvailableNitrogen = null, bool? isLiquid = null);
}
