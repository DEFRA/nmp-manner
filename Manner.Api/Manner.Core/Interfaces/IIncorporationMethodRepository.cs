using Manner.Core.Entities;

namespace Manner.Core.Interfaces;

public interface IIncorporationMethodRepository : IRepository<IncorporationMethod>
{
    Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAndApplicableForAsync(int methodId, string applicableFor);
    Task<IEnumerable<IncorporationMethod>?> FetchByAppMethodIdAsync(int methodId);
}