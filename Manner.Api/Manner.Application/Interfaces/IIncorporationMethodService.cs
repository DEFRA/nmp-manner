using Manner.Application.DTOs;

namespace Manner.Application.Interfaces;

public interface IIncorporationMethodService : IService<IncorporationMethodDto>
{
    Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAndApploicableForAsync(int methodId, string applicableFor);
    Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAsync(int methodId);
}