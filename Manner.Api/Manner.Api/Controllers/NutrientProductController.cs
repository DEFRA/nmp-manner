using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
public class NutrientProductController : ControllerBase
{
    private readonly ILogger<NutrientProductController> _logger;
    private readonly INutrientProductService _nutrientProductService;

    public NutrientProductController(ILogger<NutrientProductController> logger, INutrientProductService nutrientProductService)
    {
        _logger = logger;
        _nutrientProductService = nutrientProductService;
    }

    [HttpGet("nutrient-products")]
    [SwaggerOperation(
        Summary = "Retrieve all nutrient products",
        Description = "Fetches a list of all nutrient products available.",
        Tags = ["Nutrient Products"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> NutrientProducts()
    {
        _logger.LogTrace("NutrientProductController: nutrient-products called.");
        var data = await _nutrientProductService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? string.Empty : "No nutrient products found."
        });
    }

    [HttpGet("nutrient-products/{id}")]
    [SwaggerOperation(Summary = "Retrieve nutrient product by ID", Description = "Fetches a specific nutrient product by its unique ID.", Tags = ["Nutrient Products"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> NutrientProducts(int id)
    {
        _logger.LogTrace("NutrientProductController: nutrient-products/{Id} called.", id);
        var nutrientProduct = await _nutrientProductService.FetchByIdAsync(id);
        return nutrientProduct != null
            ? Ok(new StandardResponse { Success = true, Data = nutrientProduct })
            : NotFound(new StandardResponse { Success = false, Message = "Nutrient product not found." });
    }

}