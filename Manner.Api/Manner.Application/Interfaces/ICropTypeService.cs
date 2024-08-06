using Manner.Application.DTOs;
namespace Manner.Application.Interfaces;

public interface ICropTypeService : IService<CropTypeDto>
{
    Task<int> FetchCropUptakeFactorDefault(AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest);
}
