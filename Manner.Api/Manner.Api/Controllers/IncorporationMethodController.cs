using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]

public class IncorporationMethodController : ControllerBase
{
    private readonly ILogger<IncorporationMethodController> _logger;    
    private readonly IIncorporationMethodService _incorporationMethodService;    

    public IncorporationMethodController(ILogger<IncorporationMethodController> logger,
        IIncorporationMethodService incorporationMethodService)
    {
        _logger = logger;
        _incorporationMethodService = incorporationMethodService;
    }


    [HttpGet("incorporation-methods")]
    [SwaggerOperation(
        Summary = "Retrieve all incorporation methods", 
        Description = "Fetches a list of all incorporation methods available.", 
        Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods()
    {
        _logger.LogTrace("IncorporationMethodController: incorporation-methods called.");
        var methods = await _incorporationMethodService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = methods
        });
    }

    [HttpGet("incorporation-methods/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation method by ID", 
        Description = "Fetches a specific incorporation method by its unique ID.", 
        Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods(int id)
    {
        _logger.LogTrace("IncorporationMethodController: incorporation-methods/{Id} called.", id);
        var method = await _incorporationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation method not found." });
    }

    [HttpGet("incorporation-methods/by-app-method/{methodId}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation methods by application method ID", 
        Description = "Fetches incorporation methods associated with a specific application method ID.", 
        Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodId(int methodId)
    {
        _logger.LogTrace("IncorporationMethodController: incorporation-methods/by-app-method/{MethodId} called.", methodId);
        var methods = await _incorporationMethodService.FetchByAppMethodIdAsync(methodId);
        return methods != null && methods.Any()
            ? Ok(new StandardResponse { Success = true, Data = methods })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation methods found for the given application method ID." });
    }

    [HttpGet("incorporation-methods/by-app-method-and-applicable-for/{methodId}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation methods by application method ID", 
        Description = "Fetches incorporation methods associated with a specific application method ID.", 
        Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodIdAndApplicableFor(int methodId, [FromQuery, SwaggerParameter("Filter by ApplicableFor ('G' for Grass, 'A' for Arable and Horticulture, 'B' for Both, 'NULL' for N/A)", Required = true)] string applicableFor)
    {
        _logger.LogTrace("IncorporationMethodController: incorporation-methods/by-app-method-and-applicable-for/{MethodId}/{ApplicableFor} called.", methodId, applicableFor);
        var methods = await _incorporationMethodService.FetchByAppMethodIdAndApplicableForAsync(methodId, applicableFor);
        return methods != null && methods.Any()
            ? Ok(new StandardResponse { Success = true, Data = methods })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation methods found for the given application method ID and Applicable for" });
    }
}


