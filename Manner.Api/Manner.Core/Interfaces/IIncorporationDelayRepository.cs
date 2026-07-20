using Manner.Core.Entities;

namespace Manner.Core.Interfaces;

public interface IIncorporationDelayRepository : IRepository<IncorporationDelay>
{    
    Task<IEnumerable<IncorporationDelay>?> FetchByApplicableForAsync(string applicableFor);
    Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAndApplicableForAsync(int methodId, string applicableFor);    
    Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId);
}
