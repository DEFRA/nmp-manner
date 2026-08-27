using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
public class IncorporationDelayController : ControllerBase
{
    private readonly ILogger<IncorporationDelayController> _logger;
    private readonly IIncorporationDelayService _incorporationDelayService;    

    public IncorporationDelayController(ILogger<IncorporationDelayController> logger, IIncorporationDelayService incorporationDelayService)
    {
        _logger = logger;
        _incorporationDelayService = incorporationDelayService;        
    }

    [HttpGet("incorporation-delays")]
    [SwaggerOperation(
        Summary = "Retrieve all incorporation delays", 
        Description = "Fetches a list of all incorporation delays available.", 
        Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays()
    {
        _logger.LogTrace("IncorporationDelayController: incorporation-delays called.");
        var delays = await _incorporationDelayService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = delays
        });
    }

    [HttpGet("incorporation-delays/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation delay by ID", 
        Description = "Fetches a specific incorporation delay by its unique ID.", 
        Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays(int id)
    {
        _logger.LogTrace("IncorporationDelayController: incorporation-delays/{Id} called.", id);
        var delay = await _incorporationDelayService.FetchByIdAsync(id);
        return delay != null
            ? Ok(new StandardResponse { Success = true, Data = delay })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation delay not found." });
    }

    [HttpGet("incorporation-delays/by-incorp-method/{methodId}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation delays by incorporation method ID", 
        Description = "Fetches incorporation delays associated with a specific incorporation method.", 
        Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethod(int methodId)
    {
        _logger.LogTrace("IncorporationDelayController: incorporation-delays/by-incorp-method/{MethodId} called.", methodId);
        var delays = await _incorporationDelayService.FetchByIncorpMethodIdAsync(methodId);
        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the given method ID." });
    }

    [HttpGet("incorporation-delays/by-applicable-for")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation delays by ApplicableFor", 
        Description = "Fetches incorporation delays based on whether they apply to Liquid, Solid, Poultry, or All.", 
        Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByApplicableFor(
    [FromQuery, SwaggerParameter("Filter by ApplicableFor (L for Liquid, S for Solid, P for Poultry, NULL for N/A or Not Incorporated)", Required = true)] string applicableFor)
    {
        _logger.LogTrace("IncorporationDelayController: incorporation-delays/by-applicable-for/{ApplicableFor} called.", applicableFor);
        var delays = await _incorporationDelayService.FetchByApplicableForAsync(applicableFor);

        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the specified filter." });
    }

    [HttpGet("incorporation-delays/by-incorp-method-and-applicable-for/{methodId}")]
    [SwaggerOperation(
        Summary = "Retrieve incorporation delays by incorporation method ID and applicable for ", 
        Description = "Fetches incorporation delays associated with a specific incorporation method.", 
        Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethodAndApplicableFor(int methodId, [FromQuery, SwaggerParameter("Filter by ApplicableFor (L for Liquid, S for Solid, P for Poultry, NULL for N/A or Not Incorporated)", Required = true)] string applicableFor)
    {
        _logger.LogTrace("IncorporationDelayController: incorporation-delays/by-incorp-method-and-applicable-for/{MethodId}/{ApplicableFor} called.", methodId, applicableFor);
        var delays = await _incorporationDelayService.FetchByIncorpMethodIdAndApplicableForAsync(methodId, applicableFor);
        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the given method ID and applicable for." });
    }


}
