using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
public class NutrientController : ControllerBase
{
    private readonly ILogger<NutrientController> _logger;
    private readonly INutrientService _nutrientService;

    public NutrientController(ILogger<NutrientController> logger, INutrientService nutrientService)
    {
        _logger = logger;
        _nutrientService = nutrientService;
    }

    [HttpGet("nutrients")]
    [SwaggerOperation(
        Summary = "Retrieve all nutrients",
        Description = "Fetches a list of all nutrients available.",
        Tags = ["Nutrients"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Nutrients()
    {
        _logger.LogTrace("NutrientController: nutrients called.");
        var data = await _nutrientService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? string.Empty : "No nutrients found."
        });
    }

    [HttpGet("nutrients/{id}")]
    [SwaggerOperation(Summary = "Retrieve nutrient by ID", Description = "Fetches a specific nutrient by its unique ID.", Tags = ["Nutrients"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Nutrients(int id)
    {
        _logger.LogTrace("NutrientController: nutrients/{Id} called.", id);
        var nutrient = await _nutrientService.FetchByIdAsync(id);
        return nutrient != null
            ? Ok(new StandardResponse { Success = true, Data = nutrient })
            : NotFound(new StandardResponse { Success = false, Message = "Nutrient not found." });
    }
}
