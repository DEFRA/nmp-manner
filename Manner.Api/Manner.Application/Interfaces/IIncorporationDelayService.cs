using Manner.Application.DTOs;

namespace Manner.Application.Interfaces;

public interface IIncorporationDelayService : IService<IncorporationDelayDto>
{
    Task<IEnumerable<IncorporationDelayDto>?> FetchByApplicableForAsync(string applicableFor);
    Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAsync(int methodId);

    Task<IEnumerable<IncorporationDelayDto>?> FetchByIncorpMethodIdAndApplicableForAsync(int methodId, string applicableFor);
}
