using Manner.Core.Entities;

namespace Manner.Core.Interfaces;

public interface IManureTypeRepository : IRepository<ManureType>
{
    Task<IEnumerable<ManureType>?> FetchByCriteriaAsync(int? manureGroupId = null, int? manureTypeCategoryId = null, int? countryId = null, bool? highReadilyAvailableNitrogen = null, bool? isLiquid = null);
}
