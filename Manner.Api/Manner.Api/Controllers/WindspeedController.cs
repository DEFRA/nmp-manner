using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class WindspeedController : ControllerBase
{
    private readonly ILogger<WindspeedController> _logger;
    private readonly IWindspeedService _windspeedService;


    public WindspeedController(ILogger<WindspeedController> logger,
        IRainTypeService rainTypeService,

        IWindspeedService windspeedService,
        ICalculateResultService calculateResultService)
    {
        _logger = logger;
        _windspeedService = windspeedService;        
    }

    [HttpGet("windspeeds")]
    [SwaggerOperation(
        Summary = "Retrieve all windspeeds", 
        Description = "Fetches a list of all windspeeds available.", 
        Tags = ["Windspeeds"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds()
    {
        _logger.LogTrace("WindspeedController: windspeeds called.");
        var windspeeds = await _windspeedService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = windspeeds
        });
    }

    [HttpGet("windspeeds/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve windspeed by ID", 
        Description = "Fetches a specific windspeed by its unique ID.", 
        Tags = ["Windspeeds"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds(int id)
    {
        _logger.LogTrace("WindspeedController: windspeeds/{Id} called.", id);
        var windspeed = await _windspeedService.FetchByIdAsync(id);
        return windspeed != null
            ? Ok(new StandardResponse { Success = true, Data = windspeed })
            : NotFound(new StandardResponse { Success = false, Message = "Windspeed not found." });
    }

}
