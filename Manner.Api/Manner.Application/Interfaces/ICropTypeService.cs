using Manner.Application.DTOs;
namespace Manner.Application.Interfaces;

public interface ICropTypeService : IService<CropTypeDto>
{
    Task<AutumnCropNitrogenUptakeResponse> FetchCropUptakeFactorDefault(AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest);
}
