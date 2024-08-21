using Manner.Application.DTOs;

namespace Manner.Application.Interfaces;

public interface IIncorporationMethodService : IService<IncorporationMethodDto>
{
    Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAsync(int methodId);
}