using Manner.Application.DTOs;
using Manner.Application.Enums;
using Manner.Application.Interfaces;
using Manner.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Manner.Application.Enums.Enumerations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class ManureTypeController : ControllerBase
{
    private readonly ILogger<ManureTypeController> _logger;
    private readonly IManureTypeService _manureTypeService;

    public ManureTypeController(ILogger<ManureTypeController> logger, IManureTypeService manureTypeService)
    {
        _logger = logger;
        _manureTypeService = manureTypeService;
    }

    [HttpGet("manure-types")]
    [SwaggerOperation(
        Summary = "Retrieve all manure types or filter by criteria",
        Description = "Fetches all manure types if no filters are provided. You can filter by optional parameters such as manureGroupId, manureTypeCategoryId, countryId, highReadilyAvailableNitrogen, and isLiquid.",
        Tags = ["Manure Types"]
        )]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypes(
        [FromQuery, SwaggerParameter("ID of the manure group to filter by", Required = false)] int? manureGroupId = null,
        [FromQuery, SwaggerParameter("ID of the manure type category to filter by", Required = false)] int? manureTypeCategoryId = null,
        [FromQuery, SwaggerParameter("ID of the country to filter by", Required = false)] int? countryId = null,
        [FromQuery, SwaggerParameter("Whether to filter by highly readily available nitrogen (true/false)", Required = false)] bool? highReadilyAvailableNitrogen = null,
        [FromQuery, SwaggerParameter("Whether to filter by liquid manure types (true/false)", Required = false)] bool? isLiquid = null)
    {
        _logger.LogTrace("ManureTypeController: manure-types called.");
        IEnumerable<ManureTypeDto>? manureTypes;

        if (!manureGroupId.HasValue && !manureTypeCategoryId.HasValue && !countryId.HasValue &&
            !highReadilyAvailableNitrogen.HasValue && !isLiquid.HasValue)
        {
            manureTypes = await _manureTypeService.FetchAllAsync();
        }
        else
        {
            manureTypes = await _manureTypeService.FetchByCriteriaAsync(
                manureGroupId,
                manureTypeCategoryId,
                countryId,
                highReadilyAvailableNitrogen,
                isLiquid
            );
        }

        return manureTypes != null && manureTypes.Any()
            ? Ok(new StandardResponse { Success = true, Data = manureTypes })
            : NotFound(new StandardResponse { Success = false, Message = "No manure types found matching the specified criteria." });
    }

    [HttpGet("manure-types/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve manure type by ID",
        Description = "Fetches a specific manure type by its unique ID.",
        Tags = ["Manure Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypes(int id)
    {
        _logger.LogTrace("ManureTypeController: manure-types/{Id} called.", id);
        ManureTypeDto? manureTypeDto = await _manureTypeService.FetchByIdAsync(id);
        return manureTypeDto != null
            ? Ok(new StandardResponse { Success = true, Data = manureTypeDto })
            : NotFound(new StandardResponse { Success = false, Message = $"Manure type with ID {id} not found." });
    }

    [HttpPost("calculate-nutrients-by-dry-matter-percentage")]
    [SwaggerOperation(
        Summary = "Calculates Nutrients by Dry Matter percentage",
        Description = "Calculates Nutrients by Dry Matter percentage.",
        Tags = ["Manure Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<StandardResponse>> CalculateNutrientsBtDryMatterPercentage(ManureNutrientsDto manureNutrientsDto)
    {
        ManureNutrientsDto manureNutrientsData = await _manureTypeService.CalculateNutrieltsByDryMatterPercentageAsync(manureNutrientsDto);
        return (manureNutrientsData != null)
            ? await Task.FromResult(Ok(new StandardResponse { Success = true, Data = manureNutrientsData }))
            : BadRequest(new StandardResponse { Success = false, Message = $"Manure Nutrients calculation having an error." });
    }
}

    
