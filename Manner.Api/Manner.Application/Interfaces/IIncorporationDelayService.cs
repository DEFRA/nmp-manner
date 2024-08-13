using Manner.Application.DTOs;

namespace Manner.Application.Interfaces;

public interface IIncorporationDelayService : IService<IncorporationDelayDto>
{
    Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAsync(int methodId);
}
