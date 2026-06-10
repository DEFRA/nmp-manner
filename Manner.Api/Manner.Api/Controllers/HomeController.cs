using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
#pragma warning disable S6931
public class HomeController : ControllerBase
#pragma warning restore S6931
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRainTypeService _rainTypeService;
    public HomeController(ILogger<HomeController> logger, IRainTypeService rainTypeService)
    {
        _logger = logger;
        _rainTypeService = rainTypeService;
    }

    [HttpGet("/")]
    [SwaggerOperation(
        Summary = "Health Check", 
        Description = "Health Check of API.", 
        Tags = ["Health Checks"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<StandardResponse?>> Index()
    {
        _logger.LogTrace("HomeController : Index action called");
        StandardResponse ret = new StandardResponse();
        try
        {
            var type = await _rainTypeService.FetchByIdAsync(1);
            if (type != null)
            {
                ret.Success = true;
                ret.Message = "API is OK";
            }
            else
            {
                ret.Success = false;
                ret.Message = "API is not OK";
                ret.Errors.Add("Internal Server Error");
                _logger.LogError("Internal Server Error");
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "API is not OK";
            ret.Errors.Add(ex.Message);
            _logger.LogCritical(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
