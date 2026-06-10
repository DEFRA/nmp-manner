using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class CountryController : ControllerBase
{
    private readonly ILogger<CountryController> _logger;    
    private readonly ICountryService _countryService;    

    public CountryController(ILogger<CountryController> logger, ICountryService countryService)
    {
        _logger = logger;        
        _countryService = countryService;        
    }

    [HttpGet("countries")]
    [SwaggerOperation(
        Summary = "Retrieve all countries", 
        Description = "Fetches a list of all countries available.", 
        Tags = ["Countries"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries()
    {
        _logger.LogTrace("CountryController: countries called.");
        var data = await _countryService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? string.Empty : "No countries found."
        });
    }

    [HttpGet("countries/{id}")]
    [SwaggerOperation(Summary = "Retrieve country by ID", Description = "Fetches a specific country by its unique ID.", Tags = ["Countries"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries(int id)
    {
        _logger.LogTrace("CountryController: countries/{Id} called.", id);
        var country = await _countryService.FetchByIdAsync(id);
        return country != null
            ? Ok(new StandardResponse { Success = true, Data = country })
            : NotFound(new StandardResponse { Success = false, Message = "Country not found." });
    }

}
