using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
public class HomeController : Controller
{
    private readonly ILogger<MannerController> _logger;
    private readonly IRainTypeService _rainTypeService;
    public HomeController(ILogger<MannerController> logger, IRainTypeService rainTypeService)
    {
        _logger = logger;
        _rainTypeService = rainTypeService;
    }

    [HttpGet("/")]
    [SwaggerOperation(Summary = "Health Check", Description = "Health Check of API.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<StandardResponse?>> Index()
    {
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
            _logger.LogCritical(ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
