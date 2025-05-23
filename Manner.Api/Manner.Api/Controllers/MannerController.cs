﻿using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using System.Threading.Tasks;

namespace Manner.Api.Controllers;

[ApiController]
[Route("api/v1/")]
[Authorize]
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
    private readonly ICalculateResultService _calculateResultService;

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
        IWindspeedService windspeedService,
        ICalculateResultService calculateResultService)
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
        _calculateResultService = calculateResultService;
    }

    [HttpGet("climates/{postcode}")]
    [SwaggerOperation(Summary = "Retrieve climate data by postcode", Description = "Fetches climate information for a given postcode.", Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Climates(string postcode)
    {
        _logger.LogTrace($"MannerController: climates/{postcode} called.");
        string code = (postcode.Length > 4) ? postcode.Substring(0, 4).Trim() : postcode.Trim();
        List<string> errors = new List<string>();


        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Postcode should not be empty");

        }
        else
        {
            if (code != null)
            {
                if (code.Length < 3 && code.Length > 4)
                {
                    errors.Add("Invalid post code. Post code should be 3 or 4 length");
                }
            }
        }


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
    [SwaggerOperation(Summary = "Retrieve average annual rainfall by postcode", Description = "Fetches average annual rainfall for a given postcode.", Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> FetchAverageAnualRainfall(string postcode)
    {
        _logger.LogTrace($"MannerController: climates/avarage-annual-rainfall/{postcode} called.");
        string code = string.Empty;
        code = (postcode.Length > 4) ? postcode.Substring(0, 4).Trim() : postcode.Trim();

        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Postcode should not be empty.");

        }
        if (code != null)
        {
            if (code.Length < 3 && code.Length > 4)
            {
                errors.Add("Invalid post code. Post code should be 3 or 4 length.");
            }
        }

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
        Rainfall? data = null;
        if(code != null)
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

    [HttpGet("application-methods")]
    [SwaggerOperation(
        Summary = "Retrieve all application methods or filter by criteria",
        Description = "Fetches all application methods if no filters are provided. You can filter by optional parameters such as isLiquid and fieldType.",
        Tags = ["Application Methods"]
    )]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethods(
        [FromQuery, SwaggerParameter("Whether to filter by liquid application methods (true/false)", Required = false)] bool? isLiquid = null,
        [FromQuery, SwaggerParameter("The type of field to filter by (1 = arable, 2 = grass)", Required = false)] int? fieldType = null)
    {
        _logger.LogTrace($"MannerController: application-methods called.");
        IEnumerable<ApplicationMethodDto>? applicationMethods;

        if (!isLiquid.HasValue && !fieldType.HasValue)
        {
            // No filter provided, return all application methods
            applicationMethods = await _applicationMethodService.FetchAllAsync();
        }
        else
        {
            // Filters applied
            applicationMethods = await _applicationMethodService.FetchByCriteriaAsync(isLiquid, fieldType);
        }

        return applicationMethods != null && applicationMethods.Any()
            ? Ok(new StandardResponse { Success = true, Data = applicationMethods })
            : NotFound(new StandardResponse { Success = false, Message = "No application methods found matching the specified criteria." });
    }

    [HttpGet("application-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve application method by ID", Description = "Fetches a specific application method by its unique ID.", Tags = ["Application Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ApplicationMethodById(int id)
    {
        _logger.LogTrace($"MannerController: application-methods/{id} called.");
        var method = await _applicationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Application method not found." });
    }

    [HttpGet("crop-types")]
    [SwaggerOperation(Summary = "Retrieve all crop types", Description = "Fetches a list of all crop types available.",Tags = ["Crop Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes()
    {
        _logger.LogTrace($"MannerController: crop-types called.");
        var data = await _cropTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = data != null && data.Any(),
            Data = data,
            Message = data != null && data.Any() ? string.Empty : "No crop types found."
        });
    }

    [HttpGet("crop-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve crop type by ID", Description = "Fetches a specific crop type by its unique ID.", Tags = ["Crop Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> CropTypes(int id)
    {
        _logger.LogTrace($"MannerController: crop-types/{id} called.");
        var cropType = await _cropTypeService.FetchByIdAsync(id);
        return cropType != null
            ? Ok(new StandardResponse { Success = true, Data = cropType })
            : NotFound(new StandardResponse { Success = false, Message = "Crop type not found." });
    }

    [HttpGet("countries")]
    [SwaggerOperation(Summary = "Retrieve all countries", Description = "Fetches a list of all countries available.", Tags = ["Countries"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Countries()
    {
        _logger.LogTrace($"MannerController: countries called.");
        
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
        _logger.LogTrace($"MannerController: countries/{id} called.");
        var country = await _countryService.FetchByIdAsync(id);
        return country != null
            ? Ok(new StandardResponse { Success = true, Data = country })
            : NotFound(new StandardResponse { Success = false, Message = "Country not found." });
    }

    [HttpGet("incorporation-delays")]
    [SwaggerOperation(Summary = "Retrieve all incorporation delays", Description = "Fetches a list of all incorporation delays available.", Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays()
    {
        _logger.LogTrace($"MannerController: incorporation-delays called.");
        var delays = await _incorporationDelayService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = delays
        });
    }

    [HttpGet("incorporation-delays/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delay by ID", Description = "Fetches a specific incorporation delay by its unique ID.", Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelays(int id)
    {
        _logger.LogTrace($"MannerController: incorporation-delays/{id} called.");
        var delay = await _incorporationDelayService.FetchByIdAsync(id);
        return delay != null
            ? Ok(new StandardResponse { Success = true, Data = delay })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation delay not found." });
    }

    [HttpGet("incorporation-delays/by-incorp-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delays by incorporation method ID", Description = "Fetches incorporation delays associated with a specific incorporation method.", Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethod(int methodId)
    {
        _logger.LogTrace($"MannerController: incorporation-delays/by-incorp-method/{methodId} called.");
        var delays = await _incorporationDelayService.FetchByIncorpMethodIdAsync(methodId);
        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the given method ID." });
    }

    [HttpGet("incorporation-delays/by-applicable-for")]
    [SwaggerOperation(Summary = "Retrieve incorporation delays by ApplicableFor", Description = "Fetches incorporation delays based on whether they apply to Liquid, Solid, Poultry, or All.", Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByApplicableFor(
    [FromQuery, SwaggerParameter("Filter by ApplicableFor (L for Liquid, S for Solid, P for Poultry, NULL for N/A or Not Incorporated)", Required = true)] string applicableFor)
    {
        _logger.LogTrace($"MannerController: incorporation-delays/by-applicable-for/{applicableFor} called.");
        var delays = await _incorporationDelayService.FetchByApplicableForAsync(applicableFor);

        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the specified filter." });
    }

    [HttpGet("incorporation-delays/by-incorp-method-and-applicable-for/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation delays by incorporation method ID and applicable for ", Description = "Fetches incorporation delays associated with a specific incorporation method.", Tags = ["Incorporation Delays"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationDelaysByMethodAndApplicableFor(int methodId, [FromQuery, SwaggerParameter("Filter by ApplicableFor (L for Liquid, S for Solid, P for Poultry, NULL for N/A or Not Incorporated)", Required = true)] string applicableFor)
    {
        _logger.LogTrace($"MannerController: incorporation-delays/by-incorp-method-and-applicable-for/{methodId}/{applicableFor} called.");
        var delays = await _incorporationDelayService.FetchByIncorpMethodIdAndApplicableForAsync(methodId, applicableFor);
        return delays != null && delays.Any()
            ? Ok(new StandardResponse { Success = true, Data = delays })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation delays found for the given method ID and applicable for." });
    }


    [HttpGet("incorporation-methods")]
    [SwaggerOperation(Summary = "Retrieve all incorporation methods", Description = "Fetches a list of all incorporation methods available.", Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods()
    {
        _logger.LogTrace($"MannerController: incorporation-methods called.");
        var methods = await _incorporationMethodService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = methods
        });
    }

    [HttpGet("incorporation-methods/{id}")]
    [SwaggerOperation(Summary = "Retrieve incorporation method by ID", Description = "Fetches a specific incorporation method by its unique ID.", Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethods(int id)
    {
        _logger.LogTrace($"MannerController: incorporation-methods/{id} called.");
        var method = await _incorporationMethodService.FetchByIdAsync(id);
        return method != null
            ? Ok(new StandardResponse { Success = true, Data = method })
            : NotFound(new StandardResponse { Success = false, Message = "Incorporation method not found." });
    }

    [HttpGet("incorporation-methods/by-app-method/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation methods by application method ID", Description = "Fetches incorporation methods associated with a specific application method ID.", Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodId(int methodId)
    {
        _logger.LogTrace($"MannerController: incorporation-methods/by-app-method/{methodId} called.");
        var methods = await _incorporationMethodService.FetchByAppMethodIdAsync(methodId);
        return methods != null && methods.Any()
            ? Ok(new StandardResponse { Success = true, Data = methods })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation methods found for the given application method ID." });
    }

    [HttpGet("incorporation-methods/by-app-method-and-applicable-for/{methodId}")]
    [SwaggerOperation(Summary = "Retrieve incorporation methods by application method ID", Description = "Fetches incorporation methods associated with a specific application method ID.", Tags = ["Incorporation Methods"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> IncorporationMethodsByMethodIdAndApplicableFor(int methodId, [FromQuery, SwaggerParameter("Filter by ApplicableFor ('G' for Grass, 'A' for Arable and Horticulture, 'B' for Both, 'NULL' for N/A)", Required = true)] string applicableFor)
    {
        _logger.LogTrace($"MannerController: incorporation-methods/by-app-method-and-applicable-for/{methodId} called.");
        var methods = await _incorporationMethodService.FetchByAppMethodIdAndApploicableForAsync(methodId, applicableFor);
        return methods != null && methods.Any()
            ? Ok(new StandardResponse { Success = true, Data = methods })
            : NotFound(new StandardResponse { Success = false, Message = "No incorporation methods found for the given application method ID and Applicable for" });
    }

    [HttpGet("manure-groups")]
    [SwaggerOperation(Summary = "Retrieve all manure groups", Description = "Fetches a list of all manure groups available.", Tags = ["Manure Groups"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups()
    {
        _logger.LogTrace($"MannerController: manure-groups called.");
        var groups = await _manureGroupService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = groups
        });
    }

    [HttpGet("manure-groups/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure group by ID", Description = "Fetches a specific manure group by its unique ID.", Tags = ["Manure Groups"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureGroups(int id)
    {
        _logger.LogTrace($"MannerController: manure-groups/{id} called.");
        var group = await _manureGroupService.FetchByIdAsync(id);
        return group != null
            ? Ok(new StandardResponse { Success = true, Data = group })
            : NotFound(new StandardResponse { Success = false, Message = "Manure group not found." });
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
        _logger.LogTrace($"MannerController: manure-types called.");
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
    [SwaggerOperation(Summary = "Retrieve manure type by ID", Description = "Fetches a specific manure type by its unique ID.", Tags = ["Manure Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypes(int id)
    {
        _logger.LogTrace($"MannerController: manure-types/{id} called.");
        var type = await _manureTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = $"Manure type with ID {id} not found." });
    }

    [HttpGet("manure-type-categories")]
    [SwaggerOperation(Summary = "Retrieve all manure type categories", Description = "Fetches a list of all manure type categories available.", Tags = ["Manure Type Categories"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories()
    {
        _logger.LogTrace($"MannerController: manure-type-categories called.");
        var categories = await _manureTypeCategoryService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = categories
        });
    }

    [HttpGet("manure-type-categories/{id}")]
    [SwaggerOperation(Summary = "Retrieve manure type category by ID", Description = "Fetches a specific manure type category by its unique ID.",Tags = ["Manure Type Categories"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> ManureTypeCategories(int id)
    {
        _logger.LogTrace($"MannerController: manure-type-categories/{id} called.");
        var category = await _manureTypeCategoryService.FetchByIdAsync(id);
        return category != null
            ? Ok(new StandardResponse { Success = true, Data = category })
            : NotFound(new StandardResponse { Success = false, Message = "Manure type category not found." });
    }

    [HttpGet("moisture-types")]
    [SwaggerOperation(Summary = "Retrieve all moisture types", Description = "Fetches a list of all moisture types available.", Tags = ["Moisture Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes()
    {
        _logger.LogTrace($"MannerController: moisture-types called.");
        var types = await _moistureTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("moisture-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve moisture type by ID", Description = "Fetches a specific moisture type by its unique ID.", Tags = ["Moisture Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> MoistureTypes(int id)
    {
        _logger.LogTrace($"MannerController: moisture-types/{id} called.");
        var type = await _moistureTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Moisture type not found." });
    }

    [HttpGet("rain-types")]
    [SwaggerOperation(Summary = "Retrieve all rain types", Description = "Fetches a list of all rain types available.", Tags = ["Rain Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes()
    {
        _logger.LogTrace($"MannerController: rain-types called.");
        var types = await _rainTypeService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = types
        });
    }

    [HttpGet("rain-types/{id}")]
    [SwaggerOperation(Summary = "Retrieve rain type by ID", Description = "Fetches a specific rain type by its unique ID.", Tags = ["Rain Types"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainTypes(int id)
    {
        _logger.LogTrace($"MannerController: rain-types/{id} called.");
        var type = await _rainTypeService.FetchByIdAsync(id);
        return type != null
            ? Ok(new StandardResponse { Success = true, Data = type })
            : NotFound(new StandardResponse { Success = false, Message = "Rain type not found." });
    }

    [HttpGet("sub-soils")]
    [SwaggerOperation(Summary = "Retrieve all sub-soils", Description = "Fetches a list of all sub-soils available.", Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils()
    {
        _logger.LogTrace($"MannerController: sub-soils called.");
        var soils = await _subSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("sub-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve sub-soil by ID", Description = "Fetches a specific sub-soil by its unique ID.", Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> SubSoils(int id)
    {
        _logger.LogTrace($"MannerController: sub-soils/{id} called.");
        var soil = await _subSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Sub-soil not found." });
    }

    [HttpGet("top-soils")]
    [SwaggerOperation(Summary = "Retrieve all top-soils", Description = "Fetches a list of all top-soils available.", Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils()
    {
        _logger.LogTrace($"MannerController: top-soils called.");
        var soils = await _topSoilService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = soils
        });
    }

    [HttpGet("top-soils/{id}")]
    [SwaggerOperation(Summary = "Retrieve top-soil by ID", Description = "Fetches a specific top-soil by its unique ID.", Tags = ["Soils"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> TopSoils(int id)
    {
        _logger.LogTrace($"MannerController: top-soils/{id} called.");
        var soil = await _topSoilService.FetchByIdAsync(id);
        return soil != null
            ? Ok(new StandardResponse { Success = true, Data = soil })
            : NotFound(new StandardResponse { Success = false, Message = "Top-soil not found." });
    }

    [HttpGet("windspeeds")]
    [SwaggerOperation(Summary = "Retrieve all windspeeds", Description = "Fetches a list of all windspeeds available.", Tags = ["Windspeeds"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds()
    {
        _logger.LogTrace($"MannerController: windspeeds called.");
        var windspeeds = await _windspeedService.FetchAllAsync();
        return Ok(new StandardResponse
        {
            Success = true,
            Data = windspeeds
        });
    }

    [HttpGet("windspeeds/{id}")]
    [SwaggerOperation(Summary = "Retrieve windspeed by ID", Description = "Fetches a specific windspeed by its unique ID.", Tags = ["Windspeeds"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> Windspeeds(int id)
    {
        _logger.LogTrace($"MannerController: windspeeds/{id} called.");
        var windspeed = await _windspeedService.FetchByIdAsync(id);
        return windspeed != null
            ? Ok(new StandardResponse { Success = true, Data = windspeed })
            : NotFound(new StandardResponse { Success = false, Message = "Windspeed not found." });
    }

    #region Manner API
    [HttpPost("autumn-crop-nitrogen-uptake")]
    [SwaggerOperation(Summary = "Get Autumn Crop Nitrogen Uptake", Description = "Calculates and retrieves the nitrogen uptake for autumn crops based on the provided request data.", Tags = ["Autumn Crop Nitrogen Uptake"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> GetAutumnCropNitrogenUptake([FromBody] AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
    {
        _logger.LogTrace($"MannerController: autumn-crop-nitrogen-uptake posted for crop type Id : {autumnCropNitrogenUptakeRequest.CropTypeId}.");
        var uptakeResponse = await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = uptakeResponse
        });
    }

    [HttpPost("rainfall-post-application")]
    [SwaggerOperation(Summary = "Calculates Rainfall Post Application of Manure", Description = "Calculates the effective rainfall based on application date and end of soil drainage date.", Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainfallPostApplication([FromBody] RainfallPostApplicationRequest rainfallPostApplicationRequest)
    {
        _logger.LogTrace($"MannerController: rainfall-post-application posted for climate postcode : {rainfallPostApplicationRequest.ClimateDataPostcode}.");
        string code = string.Empty;
        code = (rainfallPostApplicationRequest.ClimateDataPostcode.Length > 4) ? rainfallPostApplicationRequest.ClimateDataPostcode.Substring(0, 4).Trim() : rainfallPostApplicationRequest.ClimateDataPostcode.Trim();

        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Postcode should not be empty.");

        }
        if (code != null)
        {
            if (code.Length < 3 && code.Length > 4)
            {
                errors.Add("Invalid post code. Post code should be 3 or 4 length.");
            }
        }

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
            rainfallPostApplicationRequest.ClimateDataPostcode = code;
        }
        var rainfallResponse = await _climateService.FetchRainfallPostApplication(rainfallPostApplicationRequest);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = rainfallResponse
        });
    }
        
    [HttpGet("rainfall-april-to-september/{postcode}")]
    [SwaggerOperation(Summary = "Retrieve average April to September rainfall by postcode", Description = "Fetches average April to September rainfall for a given postcode.", Tags = ["Climates"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StandardResponse>> RainfallAprilToSeptember(string postcode)
    {
        _logger.LogTrace($"MannerController: climates/{postcode} called.");
        string code = (postcode.Length > 4) ? postcode.Substring(0, 4).Trim() : postcode.Trim();
        List<string> errors = new List<string>();


        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Postcode should not be empty");

        }
        else
        {
            if (code != null)
            {
                if (code.Length < 3 && code.Length > 4)
                {
                    errors.Add("Invalid post code. Post code should be 3 or 4 length");
                }
            }
        }


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
                
        var rainfallResponse = await _climateService.FetchAverageAprilToSeptemberRainfall(code);
        return Ok(new StandardResponse
        {
            Success = true,
            Data = rainfallResponse
        });
    }

    [HttpPost("calculate-nutrients")]
    [SwaggerOperation(Summary = "Calculates Nutrients from manure applications", Description = "Calculates the nutrients based on manure all application.",Tags = ["Calculate Nutrients"])]
    [ProducesResponseType(typeof(StandardResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<StandardResponse>> CalculateNutrients(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        if (!string.IsNullOrWhiteSpace(calculateNutrientsRequest.Field?.FieldName))
        {
            _logger.LogTrace($"MannerController: calculate-nutrients posted for field name : {calculateNutrientsRequest.Field?.FieldName}.");
        }
        else if (calculateNutrientsRequest.Field?.FieldID > 0)
        {
            _logger.LogTrace($"MannerController: calculate-nutrients posted for field id : {calculateNutrientsRequest.Field?.FieldID}.");
        }
        else if (calculateNutrientsRequest.ManureApplications[0] != null)
        {
            _logger.LogTrace($"MannerController: calculate-nutrients posted with manure : {calculateNutrientsRequest.ManureApplications[0].ManureDetails.Name}.");
        }

        string code = string.Empty;
        code = (calculateNutrientsRequest.Postcode.Length > 4) ? calculateNutrientsRequest.Postcode.Substring(0, 4).Trim() : calculateNutrientsRequest.Postcode.Trim();

        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Postcode should not be empty.");

        }
        if (code != null)
        {
            if (code.Length < 3 && code.Length > 4)
            {
                errors.Add("Invalid post code. Post code should be 3 or 4 character length.");
            }
        }

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
        if(code != null)
        {
            calculateNutrientsRequest.Postcode = code;
        }
        


        var nutrientsResponse = await _calculateResultService.CalculateNutrientsAsync(calculateNutrientsRequest);

        return Ok(new StandardResponse
        {
            Success = true,
            Data = nutrientsResponse
        });
    }

    #endregion
}
