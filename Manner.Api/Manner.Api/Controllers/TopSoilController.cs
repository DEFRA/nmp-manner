using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class TopSoilController : ControllerBase
{
    private readonly ILogger<TopSoilController> _logger;    
    private readonly ITopSoilService _topSoilService;


    public TopSoilController(ILogger<TopSoilController> logger, IRainTypeService rainTypeService, ITopSoilService topSoilService)
    {
        _logger = logger;        
        _topSoilService = topSoilService;
    }

    [HttpGet("top-soils")]
    [SwaggerOperation(
        Summary = "Retrieve all top-soils",
        Description = "Fetches a list of all top-soils available.",
        Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils()
    {
        _logger.LogTrace("SoilController: top-soils called.");
        var soils = await _topSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("top-soils/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve top-soil by ID",
        Description = "Fetches a specific top-soil by its unique ID.",
        Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils(int id)
    {
        _logger.LogTrace("MannerController: top-soils/{Id} called.", id);
        var soil = await _topSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Top-soil not found." });
    }
}
