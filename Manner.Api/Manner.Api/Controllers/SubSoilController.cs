using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class SubSoilController : ControllerBase
{
    private readonly ILogger<SubSoilController> _logger;
    private readonly ISubSoilService _subSoilService;

    public SubSoilController(ILogger<SubSoilController> logger, IRainTypeService rainTypeService, ISubSoilService subSoilService, ITopSoilService topSoilService)
    {
        _logger = logger;        
        _subSoilService = subSoilService;                
    }

    [HttpGet("sub-soils")]
    [SwaggerOperation(
        Summary = "Retrieve all sub-soils", 
        Description = "Fetches a list of all sub-soils available.", 
        Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils()
    {
        _logger.LogTrace("SoilController: sub-soils called.");
        var soils = await _subSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("sub-soils/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve sub-soil by ID", 
        Description = "Fetches a specific sub-soil by its unique ID.", 
        Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils(int id)
    {
        _logger.LogTrace("SoilController: sub-soils/{Id} called.", id);
        var soil = await _subSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Sub-soil not found." });
    }
}
