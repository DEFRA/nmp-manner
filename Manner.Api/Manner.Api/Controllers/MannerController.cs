using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations; // Add this namespace for Swagger annotations
using System;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
//[Authorize]
public class MannerController : ControllerBase
{
    private readonly ILogger<MannerController> _logger;
    private readonly IIncorporationDelayService _incorporationDelayService;
    private readonly IManureGroupService _manureGroupService;
    private readonly IManureTypeCategoryService _manureTypeCategoryService;
    private readonly IManureTypeService _manureTypeService;
    private readonly IMoistureTypeService _moistureTypeService;
    private readonly IRainTypeService _rainTypeService;
    private readonly ISubSoilService _subSoilService;
    private readonly ITopSoilService _topSoilService;
    private readonly IWindspeedService _windspeedService;
    private readonly IClimateService _climateService;
    private readonly IApplicationMethodService _applicationMethodService;
    private readonly ICountryService _countryService;
    private readonly ICropTypeService _cropTypeService;
    private readonly IIncorporationMethodService _incorporationMethodService;

    public MannerController(ILogger<MannerController> logger,
        IClimateService climateService,
        IApplicationMethodService applicationMethodService,
        ICountryService countryService,
        ICropTypeService cropTypeService,
        IIncorporationMethodService incorporationMethodService,
        IIncorporationDelayService incorporationDelayService,
        IManureGroupService manureGroupService,
        IManureTypeCategoryService manureTypeCategoryService,
        IManureTypeService manureTypeService,
        IMoistureTypeService moistureTypeService,
        IRainTypeService rainTypeService,
        ISubSoilService subSoilService,
        ITopSoilService topSoilService,
        IWindspeedService windspeedService)
    {
        _logger = logger;
        _incorporationDelayService = incorporationDelayService;
        _manureGroupService = manureGroupService;
        _manureTypeCategoryService = manureTypeCategoryService;
        _manureTypeService = manureTypeService;
        _moistureTypeService = moistureTypeService;
        _rainTypeService = rainTypeService;
        _subSoilService = subSoilService;
        _topSoilService = topSoilService;
        _windspeedService = windspeedService;
        _climateService = climateService;
        _applicationMethodService = applicationMethodService;
        _countryService = countryService;
        _cropTypeService = cropTypeService;
        _incorporationMethodService = incorporationMethodService;
    }

    [HttpGet("climates/{postcode}")]
    [SwaggerOperation(Summary = "Retrieve climate data by postcode", Description = "Fetches climate information for a given postcode.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse?>> Climates(string postcode)
    {
        StandardResponse ret = new StandardResponse();
        try
        {
            (ret.Data, ret.Errors) = await _climateService.FetchByPostcodeAsync(postcode);
            if (ret.Data != null && !ret.Errors.Any())
            {
                ret.Success = true;
            }
            else
            {
                ret.Success = false;
                ret.Message = "No climate data found for the provided postcode.";
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching climate data.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("application-methods")]
    [SwaggerOperation(Summary = "Retrieve all application methods", Description = "Fetches a list of all application methods available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethods()
    {
        var ret = new StandardResponse();
        try
        {
            var data = await _applicationMethodService.FetchAllAsync();
            if (data != null && data.Any())
            {
                ret.Success = true;
                ret.Data = data;
            }
            else
            {
                ret.Success = false;
                ret.Message = "No application methods found.";
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching application methods.";
            ret.Errors.Add(ex.Message);

            return StatusCode(500, ret);
        }
    }

    [HttpGet("application-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve application method by ID", Description = "Fetches a specific application method by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethodById(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var method = await _applicationMethodService.FetchByIdAsync(id);
            if (method != null)
            {
                ret.Success = true;
                ret.Data = method;
            }
            else
            {
                ret.Success = false;
                ret.Message = "Application method not found.";
                return NotFound(ret);
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the application method.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }



    [HttpGet("crop-types")]
    [SwaggerOperation(Summary = "Retrieve all crop types", Description = "Fetches a list of all crop types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes()
    {
        var ret = new StandardResponse();
        try
        {
            var data = await _cropTypeService.FetchAllAsync();
            if (data != null && data.Any())
            {
                ret.Success = true;
                ret.Data = data;
            }
            else
            {
                ret.Success = false;
                ret.Message = "No crop types found.";
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching crop types.";
            ret.Errors.Add(ex.Message);

            return StatusCode(500, ret);
        }
    }

    [HttpGet("crop-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve crop type by ID", Description = "Fetches a specific crop type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var cropType = await _cropTypeService.FetchByIdAsync(id);
            if (cropType != null)
            {
                ret.Success = true;
                ret.Data = cropType;
            }
            else
            {
                ret.Success = false;
                ret.Message = "Crop type not found.";
                return NotFound(ret);
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the crop type.";
            ret.Errors.Add(ex.Message);

            return StatusCode(500, ret);
        }
    }


    [HttpGet("countries")]
    [SwaggerOperation(Summary = "Retrieve all countries", Description = "Fetches a list of all countries available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries()
    {
        var ret = new StandardResponse();
        try
        {
            var data = await _countryService.FetchAllAsync();
            if (data != null && data.Any())
            {
                ret.Success = true;
                ret.Data = data;
            }
            else
            {
                ret.Success = false;
                ret.Message = "No countries found.";
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching countries.";
            ret.Errors.Add(ex.Message);

            return StatusCode(500, ret);
        }
    }

    [HttpGet("countries/{id}")]
    [SwaggerOperation(Summary = "Retrieve country by ID", Description = "Fetches a specific country by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var country = await _countryService.FetchByIdAsync(id);
            if (country != null)
            {
                ret.Success = true;
                ret.Data = country;
            }
            else
            {
                ret.Success = false;
                ret.Message = "Country not found.";
                return NotFound(ret);
            }

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the country.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpPost("autumn-crop-nitrogen-uptake")]
    [SwaggerOperation(Summary = "Get Autumn Crop Nitrogen Uptake", Description = "Calculates and retrieves the nitrogen uptake for autumn crops based on the provided request data.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetAutumnCropNitrogenUptake([FromBody] AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
    {
        var ret = new StandardResponse();
        try
        {
            var uptakeResponse = await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest);

            ret.Success = true;
            ret.Data = uptakeResponse;

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while calculating nitrogen uptake.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("incorporation-delays")]
    [SwaggerOperation(Summary = "Retrieve all incorporation delays", Description = "Fetches a list of all incorporation delays available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays()
    {
        var ret = new StandardResponse();
        try
        {
            var delays = await _incorporationDelayService.FetchAllAsync();
            ret.Success = true;
            ret.Data = delays;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching incorporation delays.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("incorporation-delays/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delay by ID", Description = "Fetches a specific incorporation delay by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var delay = await _incorporationDelayService.FetchByIdAsync(id);
            if (delay == null)
            {
                ret.Success = false;
                ret.Message = "Incorporation delay not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = delay;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the incorporation delay.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("incorporation-delays/by-incorp-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delays by incorporation method ID", Description = "Fetches incorporation delays associated with a specific incorporation method.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethod(int methodId)
    {
        var ret = new StandardResponse();
        try
        {
            var delays = await _incorporationDelayService.FetchByIncorpMethodIdAsync(methodId);
            if (delays == null || !delays.Any())
            {
                ret.Success = false;
                ret.Message = "No incorporation delays found for the given method ID.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = delays;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching incorporation delays.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("incorporation-methods")]
    [SwaggerOperation(Summary = "Retrieve all incorporation methods", Description = "Fetches a list of all incorporation methods available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods()
    {
        var ret = new StandardResponse();
        try
        {
            var methods = await _incorporationMethodService.FetchAllAsync();
            ret.Success = true;
            ret.Data = methods;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching incorporation methods.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("incorporation-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation method by ID", Description = "Fetches a specific incorporation method by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var method = await _incorporationMethodService.FetchByIdAsync(id);
            if (method == null)
            {
                ret.Success = false;
                ret.Message = "Incorporation method not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = method;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the incorporation method.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("incorporation-methods/by-app-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation methods by application method ID", Description = "Fetches incorporation methods associated with a specific application method ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodId(int methodId)
    {
        var ret = new StandardResponse();
        try
        {
            var methods = await _incorporationMethodService.FetchByAppMethodIdAsync(methodId);
            if (methods == null || !methods.Any())
            {
                ret.Success = false;
                ret.Message = "No incorporation methods found for the given application method ID.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = methods;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching incorporation methods.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("manure-groups")]
    [SwaggerOperation(Summary = "Retrieve all manure groups", Description = "Fetches a list of all manure groups available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups()
    {
        var ret = new StandardResponse();
        try
        {
            var groups = await _manureGroupService.FetchAllAsync();
            ret.Success = true;
            ret.Data = groups;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching manure groups.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("manure-groups/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure group by ID", Description = "Fetches a specific manure group by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var group = await _manureGroupService.FetchByIdAsync(id);
            if (group == null)
            {
                ret.Success = false;
                ret.Message = "Manure group not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = group;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the manure group.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("manure-types")]
    [SwaggerOperation(
        Summary = "Retrieve all manure types or filter by criteria",
        Description = "Fetches all manure types if no filters are provided. You can filter by optional parameters such as manureGroupId, manureTypeCategoryId, countryId, highReadilyAvailableNitrogen, and isLiquid."
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
        var ret = new StandardResponse();
        try
        {
            IEnumerable<ManureTypeDto>? manureTypes;

            if (!manureGroupId.HasValue && !manureTypeCategoryId.HasValue && !countryId.HasValue &&
                !highReadilyAvailableNitrogen.HasValue && !isLiquid.HasValue)
            {
                // No filters provided, return all manure types
                manureTypes = await _manureTypeService.FetchAllAsync();
            }
            else
            {
                // Filters provided, apply them
                manureTypes = await _manureTypeService.FetchByCriteriaAsync(
                    manureGroupId,
                    manureTypeCategoryId,
                    countryId,
                    highReadilyAvailableNitrogen,
                    isLiquid
                );
            }

            if (manureTypes == null || !manureTypes.Any())
            {
                ret.Success = false;
                ret.Message = "No manure types found matching the specified criteria.";
                return NotFound(ret);
            }

            ret.Success = true;
            ret.Data = manureTypes;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching manure types.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("manure-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure type by ID", Description = "Fetches a specific manure type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypes(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var type = await _manureTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                ret.Success = false;
                ret.Message = $"Manure type with ID {id} not found.";
                return NotFound(ret);
            }

            ret.Success = true;
            ret.Data = type;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the manure type.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("manure-type-categories")]
    [SwaggerOperation(Summary = "Retrieve all manure type categories", Description = "Fetches a list of all manure type categories available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories()
    {
        var ret = new StandardResponse();
        try
        {
            var categories = await _manureTypeCategoryService.FetchAllAsync();
            ret.Success = true;
            ret.Data = categories;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching manure type categories.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("manure-type-categories/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure type category by ID", Description = "Fetches a specific manure type category by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var category = await _manureTypeCategoryService.FetchByIdAsync(id);
            if (category == null)
            {
                ret.Success = false;
                ret.Message = "Manure type category not found.";
                return NotFound(ret);
            }

            ret.Success = true;
            ret.Data = category;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the manure type category.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("moisture-types")]
    [SwaggerOperation(Summary = "Retrieve all moisture types", Description = "Fetches a list of all moisture types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes()
    {
        var ret = new StandardResponse();
        try
        {
            var types = await _moistureTypeService.FetchAllAsync();
            ret.Success = true;
            ret.Data = types;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching moisture types.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("moisture-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve moisture type by ID", Description = "Fetches a specific moisture type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var type = await _moistureTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                ret.Success = false;
                ret.Message = "Moisture type not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = type;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the moisture type.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("rain-types")]
    [SwaggerOperation(Summary = "Retrieve all rain types", Description = "Fetches a list of all rain types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes()
    {
        var ret = new StandardResponse();
        try
        {
            var types = await _rainTypeService.FetchAllAsync();
            ret.Success = true;
            ret.Data = types;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching rain types.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("rain-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve rain type by ID", Description = "Fetches a specific rain type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var type = await _rainTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                ret.Success = false;
                ret.Message = "Rain type not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = type;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the rain type.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("sub-soils")]
    [SwaggerOperation(Summary = "Retrieve all sub-soils", Description = "Fetches a list of all sub-soils available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils()
    {
        var ret = new StandardResponse();
        try
        {
            var soils = await _subSoilService.FetchAllAsync();
            ret.Success = true;
            ret.Data = soils;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching sub-soils.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("sub-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve sub-soil by ID", Description = "Fetches a specific sub-soil by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var soil = await _subSoilService.FetchByIdAsync(id);
            if (soil == null)
            {
                ret.Success = false;
                ret.Message = "Sub-soil not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = soil;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the sub-soil.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


    [HttpGet("top-soils")]
    [SwaggerOperation(Summary = "Retrieve all top-soils", Description = "Fetches a list of all top-soils available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils()
    {
        var ret = new StandardResponse();
        try
        {
            var soils = await _topSoilService.FetchAllAsync();
            ret.Success = true;
            ret.Data = soils;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching top-soils.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("top-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve top-soil by ID", Description = "Fetches a specific top-soil by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var soil = await _topSoilService.FetchByIdAsync(id);
            if (soil == null)
            {
                ret.Success = false;
                ret.Message = "Top-soil not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = soil;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the top-soil.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("windspeeds")]
    [SwaggerOperation(Summary = "Retrieve all windspeeds", Description = "Fetches a list of all windspeeds available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds()
    {
        var ret = new StandardResponse();
        try
        {
            var windspeeds = await _windspeedService.FetchAllAsync();
            ret.Success = true;
            ret.Data = windspeeds;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching windspeeds.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpGet("windspeeds/{id}")]
    [SwaggerOperation(Summary = "Retrieve windspeed by ID", Description = "Fetches a specific windspeed by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds(int id)
    {
        var ret = new StandardResponse();
        try
        {
            var windspeed = await _windspeedService.FetchByIdAsync(id);
            if (windspeed == null)
            {
                ret.Success = false;
                ret.Message = "Windspeed not found.";
                return NotFound(ret);
            }
            ret.Success = true;
            ret.Data = windspeed;
            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while fetching the windspeed.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }

    [HttpPost("effective-rainfall")]
    [SwaggerOperation(Summary = "Calculates Rainfall Post Application of Manure", Description = "Calculates the effective rainfall based on application date and end of soil drainage date.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetEffectiveRainfall([FromBody] EffectiveRainfallRequest effectiveRainfallRequest)
    {
        var ret = new StandardResponse();
        try
        {
            var rainfallResponse = await _climateService.FetchEffectiveRainFall(effectiveRainfallRequest);

            ret.Success = true;
            ret.Data = rainfallResponse;

            return Ok(ret);
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.Message = "An error occurred while calculating effective rainfall.";
            ret.Errors.Add(ex.Message);
            return StatusCode(500, ret);
        }
    }


}
