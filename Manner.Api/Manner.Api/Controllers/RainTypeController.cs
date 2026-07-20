using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class RainTypeController : ControllerBase
{
    private readonly ILogger<RainTypeController> _logger;
    private readonly IRainTypeService _rainTypeService;
    public RainTypeController(ILogger<RainTypeController> logger, IRainTypeService rainTypeService)
    {
        _logger = logger;
        _rainTypeService = rainTypeService;
    }

    [HttpGet("rain-types")]
    [SwaggerOperation(
        Summary = "Retrieve all rain types", 
        Description = "Fetches a list of all rain types available.", 
        Tags = ["Rain Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes()
    {
        _logger.LogTrace("RainTypeController: rain-types called.");
        var types = await _rainTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("rain-types/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve rain type by ID", 
        Description = "Fetches a specific rain type by its unique ID.", 
        Tags = ["Rain Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes(int id)
    {
        _logger.LogTrace("RainTypeController: rain-types/{Id} called.", id);
        var type = await _rainTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Rain type not found." });
    }
}
