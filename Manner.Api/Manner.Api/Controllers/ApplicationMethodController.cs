using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
public class ApplicationMethodController : ControllerBase
{
    private readonly ILogger<ApplicationMethodController> _logger;
    private readonly IApplicationMethodService _applicationMethodService;   

    public ApplicationMethodController(ILogger<ApplicationMethodController> logger, IApplicationMethodService applicationMethodService)
    {
        _logger = logger;
        _applicationMethodService = applicationMethodService;
    }

    [HttpGet("application-methods")]
    [SwaggerOperation(
        Summary = "Retrieve all application methods or filter by criteria",
        Description = "Fetches all application methods if no filters are provided. You can filter by optional parameters such as isLiquid and fieldType.",
        Tags = ["Application Methods"]
    )]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethods(
        [FromQuery, SwaggerParameter("Whether to filter by liquid application methods (true/false)", Required = false)] bool? isLiquid = null,
        [FromQuery, SwaggerParameter("The type of field to filter by (1 = arable, 2 = grass)", Required = false)] int? fieldType = null)
    {
        _logger.LogTrace("ApplicationMethodController: application-methods called.");
        IEnumerable<ApplicationMethodDto>? applicationMethods;

        if (!isLiquid.HasValue && !fieldType.HasValue)
        {
            // No filter provided, return all application methods
            applicationMethods = await _applicationMethodService.FetchAllAsync();
        }
        else
        {
            // Filters applied
            applicationMethods = await _applicationMethodService.FetchByCriteriaAsync(isLiquid, fieldType);
        }

        return applicationMethods != null && applicationMethods.Any()
            ? Ok(new StandardResponse { Success = true, Data = applicationMethods })
            : NotFound(new StandardResponse { Success = false, Message = "No application methods found matching the specified criteria." });
    }

    [HttpGet("application-methods/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve application method by ID", 
        Description = "Fetches a specific application method by its unique ID.", 
        Tags = ["Application Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethodById(int id)
    {
        _logger.LogTrace("ApplicationMethodController: application-methods/{Id} called.", id);
        var method = await _applicationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Application method not found." });
    }
}
