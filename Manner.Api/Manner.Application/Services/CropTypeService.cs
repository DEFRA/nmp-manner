using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Exceptions;
using Manner.Application.Interfaces;
using Manner.Application.Validators;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class CropTypeService(ILogger<CropTypeService> logger, ICropTypeRepository cropTypeRepository, IMapper mapper) : ICropTypeService
{
    private readonly ICropTypeRepository _cropTypeRepository = cropTypeRepository;    
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CropTypeService> _logger = logger;

    public async Task<IEnumerable<CropTypeDto>?> FetchAllAsync()
    {
        _logger.LogTrace($"CropTypeService : FetchAllAsync() callled");
        return _mapper.Map<IEnumerable<CropTypeDto>>(await _cropTypeRepository.FetchAllAsync());
    }

    public async Task<CropTypeDto?> FetchByIdAsync(int id)
    {
        _logger.LogTrace($"CropTypeService : FetchByIdAsync({id}) callled");
        return _mapper.Map<CropTypeDto>(await _cropTypeRepository.FetchByIdAsync(id));
    }

    public async Task<AutumnCropNitrogenUptakeResponse> FetchCropUptakeFactorDefault(AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
    {
        _logger.LogTrace($"CropTypeService : FetchCropUptakeFactorDefault({autumnCropNitrogenUptakeRequest.CropTypeId},{autumnCropNitrogenUptakeRequest.ApplicationMonth} ) callled");
        AutumnCropNitrogenUptakeResponse ret = new();
        ret.CropTypeId = autumnCropNitrogenUptakeRequest.CropTypeId;
        ret.CropType = "Others";
        if (autumnCropNitrogenUptakeRequest.ApplicationMonth >= 8 && autumnCropNitrogenUptakeRequest.ApplicationMonth <= 10)
        {
            var croptype = await _cropTypeRepository.FetchByIdAsync(autumnCropNitrogenUptakeRequest.CropTypeId);
            if (croptype != null)
            {
                ret.CropType = croptype.Name;
                ret.NitrogenUptake.Value = croptype.CropUptakeFactor;
            }
        }      

        return ret;
    }
}
