using Manner.Core.Entities;

namespace Manner.Core.Interfaces;

public interface IIncorporationDelayRepository : IRepository<IncorporationDelay>
{
    //Task<IEnumerable<IncorporationDelay>?> FetchAllAsync();
    Task<IEnumerable<IncorporationDelay>?> FetchByApplicableForAsync(string applicableFor);
    //Task<IncorporationDelay?> FetchByIdAsync(int id);
    Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId);
}
