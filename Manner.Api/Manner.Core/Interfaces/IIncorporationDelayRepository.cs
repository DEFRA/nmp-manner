using Manner.Core.Entities;

namespace Manner.Core.Interfaces;

public interface IIncorporationDelayRepository : IRepository<IncorporationDelay>
{
    Task<IEnumerable<IncorporationDelay>?> FetchByIncorpMethodIdAsync(int methodId);
}
