using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class ManureGroupController : ControllerBase
{
    private readonly ILogger<ManureGroupController> _logger;
    private readonly IManureGroupService _manureGroupService;    

    public ManureGroupController(ILogger<ManureGroupController> logger, IManureGroupService manureGroupService)
    {
        _logger = logger;
        _manureGroupService = manureGroupService;        
    }

    [HttpGet("manure-groups")]
    [SwaggerOperation(
        Summary = "Retrieve all manure groups", 
        Description = "Fetches a list of all manure groups available.", 
        Tags = ["Manure Groups"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups()
    {
        _logger.LogTrace("ManureGroupController: manure-groups called.");
        var groups = await _manureGroupService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = groups
        });
    }

    [HttpGet("manure-groups/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve manure group by ID", 
        Description = "Fetches a specific manure group by its unique ID.", 
        Tags = ["Manure Groups"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups(int id)
    {
        _logger.LogTrace("ManureGroupController: manure-groups/{Id} called.", id);
        var group = await _manureGroupService.FetchByIdAsync(id);
        return group != null
            ? Ok(new StandardResponse { Success = true, Data = group })
            : NotFound(new StandardResponse { Success = false, Message = "Manure group not found." });
    }

}
