using Manner.Core.Entities;

namespace Manner.Core.Interfaces;
public interface IApplicationMethodRepository : IRepository<ApplicationMethod>
{
    Task<IEnumerable<ApplicationMethod>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null);
}
