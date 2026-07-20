using AutoMapper;
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
public class MannerController : ControllerBase
{
    private readonly ILogger<MannerController> _logger;
    private readonly ICalculateResultService _calculateResultService;

    public MannerController(ILogger<MannerController> logger, ICalculateResultService calculateResultService)
    {
        _logger = logger;
        _calculateResultService = calculateResultService;
    }       

    [HttpPost("calculate-nutrients")]
    [SwaggerOperation(
        Summary = "Calculates Nutrients from manure applications", 
        Description = "Calculates the nutrients based on manure all application.", 
        Tags = ["Calculate Nutrients"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<StandardResponse>> CalculateNutrients(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        if (!string.IsNullOrWhiteSpace(calculateNutrientsRequest.Field?.FieldName))
        {
            _logger.LogTrace("MannerController: calculate-nutrients posted for field name : {FieldName}.", calculateNutrientsRequest.Field?.FieldName);
        }
        else if (calculateNutrientsRequest.Field?.FieldID > 0)
        {
            _logger.LogTrace("MannerController: calculate-nutrients posted for field id : {FieldID}.", calculateNutrientsRequest.Field?.FieldID);
        }
        else if (calculateNutrientsRequest.ManureApplications[0] != null)
        {
            _logger.LogTrace("MannerController: calculate-nutrients posted with manure : {ManureName}.", calculateNutrientsRequest.ManureApplications[0].ManureDetails.Name);
        }
        List<string> errors;

        string code = Function.GetOutwardCode(calculateNutrientsRequest.Postcode, out errors);

        if (errors.Any())
        {
            return Ok(new StandardResponse
            {
                Success = !errors.Any(),
                Data = null,
                Message = "Invalid Postcode.",
                Errors = errors
            });
        }
        if (code != null)
        {
            calculateNutrientsRequest.Postcode = code;
        }

        dynamic? nutrientsResponse = null;

        if (calculateNutrientsRequest.IndivisualApplicationOutput)
        {
            nutrientsResponse = await _calculateResultService.CalculateNutrientsIndivisualApplicationsAsync(calculateNutrientsRequest);
        }
        else
        {
            nutrientsResponse = await _calculateResultService.CalculateNutrientsAsync(calculateNutrientsRequest);
        }
           

        return Ok(new StandardResponse
        {
            Success = true,
            Data = nutrientsResponse
        });
    }
}
