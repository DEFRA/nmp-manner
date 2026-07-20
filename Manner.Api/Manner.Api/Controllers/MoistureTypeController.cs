using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class MoistureTypeController : ControllerBase
{
    private readonly ILogger<MoistureTypeController> _logger;
    private readonly IMoistureTypeService _moistureTypeService;    

    public MoistureTypeController(ILogger<MoistureTypeController> logger, IMoistureTypeService moistureTypeService)
    {
        _logger = logger;

        _moistureTypeService = moistureTypeService;        
    }

    [HttpGet("moisture-types")]
    [SwaggerOperation(
        Summary = "Retrieve all moisture types", 
        Description = "Fetches a list of all moisture types available.", 
        Tags = ["Moisture Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes()
    {
        _logger.LogTrace("MoistureTypeController: moisture-types called.");
        var types = await _moistureTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("moisture-types/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve moisture type by ID", 
        Description = "Fetches a specific moisture type by its unique ID.", 
        Tags = ["Moisture Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes(int id)
    {
        _logger.LogTrace("MoistureTypeController: moisture-types/{Id} called.", id);
        var type = await _moistureTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Moisture type not found." });
    }
}
