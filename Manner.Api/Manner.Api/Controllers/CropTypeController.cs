using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class CropTypeController : ControllerBase
{
    private readonly ILogger<CropTypeController> _logger;
    private readonly ICropTypeService _cropTypeService;
    public CropTypeController(ILogger<CropTypeController> logger, ICropTypeService cropTypeService)
    {
        _logger = logger;
        _cropTypeService = cropTypeService;
    }


    [HttpGet("crop-types")]
    [SwaggerOperation(Summary = "Retrieve all crop types", Description = "Fetches a list of all crop types available.", Tags = ["Crop Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes()
    {
        _logger.LogTrace("CropTypeController: crop-types called.");
        var data = await _cropTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? string.Empty : "No crop types found."
        });
    }

    [HttpGet("crop-types/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve crop type by ID", 
        Description = "Fetches a specific crop type by its unique ID.", 
        Tags = ["Crop Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes(int id)
    {
        _logger.LogTrace("CropTypeController: crop-types/{Id} called.", id);
        var cropType = await _cropTypeService.FetchByIdAsync(id);
        return cropType != null
            ? Ok(new StandardResponse { Success = true, Data = cropType })
            : NotFound(new StandardResponse { Success = false, Message = "Crop type not found." });
    }

    [HttpPost("crop-types/autumn-crop-nitrogen-uptake")]
    [SwaggerOperation(
        Summary = "Get Autumn Crop Nitrogen Uptake", 
        Description = "Calculates and retrieves the nitrogen uptake for autumn crops based on the provided request data.", 
        Tags = ["Crop Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetAutumnCropNitrogenUptake([FromBody] AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
    {
        _logger.LogTrace("CropTypeController: autumn-crop-nitrogen-uptake posted for crop type Id : {CropTypeId}.", autumnCropNitrogenUptakeRequest.CropTypeId);
        var uptakeResponse = await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = uptakeResponse
        });
    }
}
