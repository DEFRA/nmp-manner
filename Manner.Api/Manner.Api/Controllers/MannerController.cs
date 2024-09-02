using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

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
    public async Task<ActionResult<StandardResponse>> Climates(string postcode)
    {
        var (data, errors) = await _climateService.FetchByPostcodeAsync(postcode);
        return Ok(new StandardResponse
        {
            Success = data != null && !errors.Any(),
            Data = data,
            Message = data != null ? null : "No climate data found for the provided postcode.",
            Errors = errors
        });
    }

    [HttpGet("application-methods")]
    [SwaggerOperation(Summary = "Retrieve all application methods", Description = "Fetches a list of all application methods available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethods()
    {
        var data = await _applicationMethodService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? null : "No application methods found."
        });
    }

    [HttpGet("application-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve application method by ID", Description = "Fetches a specific application method by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethodById(int id)
    {
        var method = await _applicationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Application method not found." });
    }

    [HttpGet("crop-types")]
    [SwaggerOperation(Summary = "Retrieve all crop types", Description = "Fetches a list of all crop types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes()
    {
        var data = await _cropTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? null : "No crop types found."
        });
    }

    [HttpGet("crop-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve crop type by ID", Description = "Fetches a specific crop type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes(int id)
    {
        var cropType = await _cropTypeService.FetchByIdAsync(id);
        return cropType != null
            ? Ok(new StandardResponse { Success = true, Data = cropType })
            : NotFound(new StandardResponse { Success = false, Message = "Crop type not found." });
    }

    [HttpGet("countries")]
    [SwaggerOperation(Summary = "Retrieve all countries", Description = "Fetches a list of all countries available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries()
    {
        var data = await _countryService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? null : "No countries found."
        });
    }

    [HttpGet("countries/{id}")]
    [SwaggerOperation(Summary = "Retrieve country by ID", Description = "Fetches a specific country by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries(int id)
    {
        var country = await _countryService.FetchByIdAsync(id);
        return country != null
            ? Ok(new StandardResponse { Success = true, Data = country })
            : NotFound(new StandardResponse { Success = false, Message = "Country not found." });
    }

    [HttpPost("autumn-crop-nitrogen-uptake")]
    [SwaggerOperation(Summary = "Get Autumn Crop Nitrogen Uptake", Description = "Calculates and retrieves the nitrogen uptake for autumn crops based on the provided request data.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetAutumnCropNitrogenUptake([FromBody] AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
    {
        var uptakeResponse = await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = uptakeResponse
        });
    }

    [HttpGet("incorporation-delays")]
    [SwaggerOperation(Summary = "Retrieve all incorporation delays", Description = "Fetches a list of all incorporation delays available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays()
    {
        var delays = await _incorporationDelayService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = delays
        });
    }

    [HttpGet("incorporation-delays/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delay by ID", Description = "Fetches a specific incorporation delay by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays(int id)
    {
        var delay = await _incorporationDelayService.FetchByIdAsync(id);
        return delay != null
            ? Ok(new StandardResponse { Success = true, Data = delay })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation delay not found." });
    }

    [HttpGet("incorporation-delays/by-incorp-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delays by incorporation method ID", Description = "Fetches incorporation delays associated with a specific incorporation method.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethod(int methodId)
    {
        var delays = await _incorporationDelayService.FetchByIncorpMethodIdAsync(methodId);
        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the given method ID." });
    }

    [HttpGet("incorporation-methods")]
    [SwaggerOperation(Summary = "Retrieve all incorporation methods", Description = "Fetches a list of all incorporation methods available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods()
    {
        var methods = await _incorporationMethodService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = methods
        });
    }

    [HttpGet("incorporation-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation method by ID", Description = "Fetches a specific incorporation method by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods(int id)
    {
        var method = await _incorporationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation method not found." });
    }

    [HttpGet("incorporation-methods/by-app-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation methods by application method ID", Description = "Fetches incorporation methods associated with a specific application method ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodId(int methodId)
    {
        var methods = await _incorporationMethodService.FetchByAppMethodIdAsync(methodId);
        return methods != null && methods.Any()
            ? Ok(new StandardResponse { Success = true, Data = methods })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation methods found for the given application method ID." });
    }

    [HttpGet("manure-groups")]
    [SwaggerOperation(Summary = "Retrieve all manure groups", Description = "Fetches a list of all manure groups available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups()
    {
        var groups = await _manureGroupService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = groups
        });
    }

    [HttpGet("manure-groups/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure group by ID", Description = "Fetches a specific manure group by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups(int id)
    {
        var group = await _manureGroupService.FetchByIdAsync(id);
        return group != null
            ? Ok(new StandardResponse { Success = true, Data = group })
            : NotFound(new StandardResponse { Success = false, Message = "Manure group not found." });
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
    [SwaggerOperation(Summary = "Retrieve manure type by ID", Description = "Fetches a specific manure type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypes(int id)
    {
        var type = await _manureTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = $"Manure type with ID {id} not found." });
    }

    [HttpGet("manure-type-categories")]
    [SwaggerOperation(Summary = "Retrieve all manure type categories", Description = "Fetches a list of all manure type categories available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories()
    {
        var categories = await _manureTypeCategoryService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = categories
        });
    }

    [HttpGet("manure-type-categories/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure type category by ID", Description = "Fetches a specific manure type category by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories(int id)
    {
        var category = await _manureTypeCategoryService.FetchByIdAsync(id);
        return category != null
            ? Ok(new StandardResponse { Success = true, Data = category })
            : NotFound(new StandardResponse { Success = false, Message = "Manure type category not found." });
    }

    [HttpGet("moisture-types")]
    [SwaggerOperation(Summary = "Retrieve all moisture types", Description = "Fetches a list of all moisture types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes()
    {
        var types = await _moistureTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("moisture-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve moisture type by ID", Description = "Fetches a specific moisture type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes(int id)
    {
        var type = await _moistureTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Moisture type not found." });
    }

    [HttpGet("rain-types")]
    [SwaggerOperation(Summary = "Retrieve all rain types", Description = "Fetches a list of all rain types available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes()
    {
        var types = await _rainTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("rain-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve rain type by ID", Description = "Fetches a specific rain type by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes(int id)
    {
        var type = await _rainTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Rain type not found." });
    }

    [HttpGet("sub-soils")]
    [SwaggerOperation(Summary = "Retrieve all sub-soils", Description = "Fetches a list of all sub-soils available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils()
    {
        var soils = await _subSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("sub-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve sub-soil by ID", Description = "Fetches a specific sub-soil by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils(int id)
    {
        var soil = await _subSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Sub-soil not found." });
    }

    [HttpGet("top-soils")]
    [SwaggerOperation(Summary = "Retrieve all top-soils", Description = "Fetches a list of all top-soils available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils()
    {
        var soils = await _topSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("top-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve top-soil by ID", Description = "Fetches a specific top-soil by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils(int id)
    {
        var soil = await _topSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Top-soil not found." });
    }

    [HttpGet("windspeeds")]
    [SwaggerOperation(Summary = "Retrieve all windspeeds", Description = "Fetches a list of all windspeeds available.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds()
    {
        var windspeeds = await _windspeedService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = windspeeds
        });
    }

    [HttpGet("windspeeds/{id}")]
    [SwaggerOperation(Summary = "Retrieve windspeed by ID", Description = "Fetches a specific windspeed by its unique ID.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds(int id)
    {
        var windspeed = await _windspeedService.FetchByIdAsync(id);
        return windspeed != null
            ? Ok(new StandardResponse { Success = true, Data = windspeed })
            : NotFound(new StandardResponse { Success = false, Message = "Windspeed not found." });
    }

    [HttpPost("effective-rainfall")]
    [SwaggerOperation(Summary = "Calculates Rainfall Post Application of Manure", Description = "Calculates the effective rainfall based on application date and end of soil drainage date.")]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetEffectiveRainfall([FromBody] EffectiveRainfallRequest effectiveRainfallRequest)
    {
        var rainfallResponse = await _climateService.FetchEffectiveRainFall(effectiveRainfallRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = rainfallResponse
        });
    }
}
