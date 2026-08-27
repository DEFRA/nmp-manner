using Manner.Api.Helpers;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
public class ClimateController : ControllerBase
{
    private readonly ILogger<ClimateController> _logger;
    private readonly IClimateService _climateService;  

    public ClimateController(ILogger<ClimateController> logger, IClimateService climateService)
    {
        _logger = logger;
        _climateService = climateService;
    }

    [HttpGet("climates/{postcode}")]
    [SwaggerOperation(
        Summary = "Retrieve climate data by postcode", 
        Description = "Fetches climate information for a given postcode.", 
        Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Climates(string postcode)
    {
        _logger.LogTrace("ClimateController: climates/{Postcode} called.", postcode);
        List<string> errors;
        string code = Function.GetOutwardCode(postcode, out errors);
        if (errors.Any())
        {
            return Ok(new StandardResponse
            {
                Success = !errors.Any(),
                Data = null,
                Message = Function.InvalidPostcodeMessage,
                Errors = errors
            });
        }
        ClimateDto? data = null;
        if (code != null)
        {
            data = await _climateService.FetchByPostcodeAsync(code);
        }
        return Ok(new StandardResponse
        {
            Success = data != null && !errors.Any(),
            Data = new { climate = data },
            Message = data == null ? "No climate data found for the provided postcode." : string.Empty,
            Errors = errors
        });
    }

    [HttpGet("climates/avarage-annual-rainfall/{postcode}")]
    [SwaggerOperation(
        Summary = "Retrieve average annual rainfall by postcode", 
        Description = "Fetches average annual rainfall for a given postcode.", 
        Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> FetchAverageAnualRainfall(string postcode)
    {
        _logger.LogTrace("ClimateController: climates/avarage-annual-rainfall/{Postcode} called.", postcode);
        List<string> errors;
        string code = Function.GetOutwardCode(postcode, out errors);

        if (errors.Any())
        {
            return Ok(new StandardResponse
            {
                Success = !errors.Any(),
                Data = null,
                Message = Function.InvalidPostcodeMessage,
                Errors = errors
            });
        }
        Rainfall? data = null;
        if (code != null)
        {
            data = await _climateService.FetchAverageAnualRainfall(code);
        }

        return Ok(new StandardResponse
        {
            Success = data != null && !errors.Any(),
            Data = new { AvarageAnnualRainfall = data },
            Message = data != null ? string.Empty : "No avarage annual rainfall data found for the provided postcode.",
            Errors = errors
        });

    }

    [HttpPost("climates/rainfall-post-application")]
    [SwaggerOperation(
        Summary = "Calculates Rainfall Post Application of Manure", 
        Description = "Calculates the effective rainfall based on application date and end of soil drainage date.", 
        Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainfallPostApplication([FromBody] RainfallPostApplicationRequest rainfallPostApplicationRequest)
    {
        _logger.LogTrace("ClimateController: rainfall-post-application posted for climate postcode : {ClimateDataPostcode}.", rainfallPostApplicationRequest.ClimateDataPostcode);

        List<string> errors;
        string code = Function.GetOutwardCode(rainfallPostApplicationRequest.ClimateDataPostcode, out errors);

        if (errors.Any())
        {
            return Ok(new StandardResponse
            {
                Success = !errors.Any(),
                Data = null,
                Message = Function.InvalidPostcodeMessage,
                Errors = errors
            });
        }

        if (code != null)
        {
            rainfallPostApplicationRequest.ClimateDataPostcode = code;
        }
        var rainfallResponse = await _climateService.FetchRainfallPostApplication(rainfallPostApplicationRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = rainfallResponse
        });
    }

    [HttpGet("climates/rainfall-april-to-september/{postcode}")]
    [SwaggerOperation(
        Summary = "Retrieve average April to September rainfall by postcode", 
        Description = "Fetches average April to September rainfall for a given postcode.", 
        Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainfallAprilToSeptember(string postcode)
    {
        _logger.LogTrace("MannerController: rainfall-april-to-september/{Postcode} called.", postcode);

        List<string> errors;
        string code = Function.GetOutwardCode(postcode, out errors);

        if (errors.Any())
        {
            return Ok(new StandardResponse
            {
                Success = !errors.Any(),
                Data = null,
                Message = Function.InvalidPostcodeMessage,
                Errors = errors
            });
        }

        var rainfallResponse = await _climateService.FetchAverageAprilToSeptemberRainfall(code);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = rainfallResponse
        });
    }
}
