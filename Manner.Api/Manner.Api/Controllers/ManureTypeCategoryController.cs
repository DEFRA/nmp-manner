using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;
[ApiController]
[Route("api/v1/")]
[Authorize]
public class ManureTypeCategoryController : ControllerBase
{
    private readonly ILogger<ManureTypeCategoryController> _logger;
    private readonly IManureTypeCategoryService _manureTypeCategoryService;
    

    public ManureTypeCategoryController(ILogger<ManureTypeCategoryController> logger, IManureTypeCategoryService manureTypeCategoryService)
    {
        _logger = logger;
        _manureTypeCategoryService = manureTypeCategoryService;        
    }


    [HttpGet("manure-type-categories")]
    [SwaggerOperation(
        Summary = "Retrieve all manure type categories", 
        Description = "Fetches a list of all manure type categories available.", 
        Tags = ["Manure Type Categories"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories()
    {
        _logger.LogTrace("ManureTypeCategoryController: manure-type-categories called.");
        var categories = await _manureTypeCategoryService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = categories
        });
    }

    [HttpGet("manure-type-categories/{id}")]
    [SwaggerOperation(
        Summary = "Retrieve manure type category by ID", 
        Description = "Fetches a specific manure type category by its unique ID.", 
        Tags = ["Manure Type Categories"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories(int id)
    {
        _logger.LogTrace("ManureTypeCategoryController: manure-type-categories/{Id} called.", id);
        var category = await _manureTypeCategoryService.FetchByIdAsync(id);
        return category != null
            ? Ok(new StandardResponse { Success = true, Data = category })
            : NotFound(new StandardResponse { Success = false, Message = "Manure type category not found." });
    }
}
