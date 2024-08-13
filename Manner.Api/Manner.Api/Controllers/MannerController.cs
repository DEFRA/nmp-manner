using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations; // Add this namespace for Swagger annotations
using System.Data.SqlTypes;

namespace Manner.Api.Controllers
{
    [ApiController]
    [Route("api/manner")]
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
        [ProducesResponseType(typeof(ClimateDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ClimateDto?>> Climates(string postcode)
        {
            var climate = await _climateService.FetchByPostcodeAsync(postcode);
            if (climate == null)
            {
                return NotFound();
            }
            return Ok(climate);
        }

        [HttpGet("application-methods")]
        [SwaggerOperation(Summary = "Retrieve all application methods", Description = "Fetches a list of all application methods available.")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationMethodDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApplicationMethodDto>?>> ApplicationMethods()
        {
            return Ok(await _applicationMethodService.FetchAllAsync());
        }

        [HttpGet("application-methods/{id}")]
        [SwaggerOperation(Summary = "Retrieve application method by ID", Description = "Fetches a specific application method by its unique ID.")]
        [ProducesResponseType(typeof(ApplicationMethodDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApplicationMethodDto>?> ApplicationMethods(int id)
        {
            var method = await _applicationMethodService.FetchByIdAsync(id);
            if (method == null)
            {
                return NotFound();
            }
            return Ok(method);
        }

        [HttpGet("crop-types")]
        [SwaggerOperation(Summary = "Retrieve all crop types", Description = "Fetches a list of all crop types available.")]
        [ProducesResponseType(typeof(IEnumerable<CropTypeDto>), 200)]
        public async Task<ActionResult<IEnumerable<CropTypeDto>?>> CropTypes()
        {
            return Ok(await _cropTypeService.FetchAllAsync());
        }

        [HttpGet("crop-types/{id}")]
        [SwaggerOperation(Summary = "Retrieve crop type by ID", Description = "Fetches a specific crop type by its unique ID.")]
        [ProducesResponseType(typeof(CropTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CropTypeDto>?> CropTypes(int id)
        {
            var cropType = await _cropTypeService.FetchByIdAsync(id);
            if (cropType == null)
            {
                return NotFound();
            }
            return Ok(cropType);
        }

        [HttpGet("countries")]
        [SwaggerOperation(Summary = "Retrieve all countries", Description = "Fetches a list of all countries available.")]
        [ProducesResponseType(typeof(IEnumerable<CountryDto>), 200)]
        public async Task<ActionResult<IEnumerable<CountryDto>?>> Countries()
        {
            return Ok(await _countryService.FetchAllAsync());
        }

        [HttpGet("countries/{id}")]
        [SwaggerOperation(Summary = "Retrieve country by ID", Description = "Fetches a specific country by its unique ID.")]
        [ProducesResponseType(typeof(CountryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CountryDto>?> Countries(int id)
        {
            var country = await _countryService.FetchByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        [HttpPost("autumn-crop-nitrogen-uptake")]
        [SwaggerOperation(Summary = "Get Autumn Crop Nitrogen Uptake", Description = "Calculates and retrieves the nitrogen uptake for autumn crops based on the provided request data.")]
        [ProducesResponseType(typeof(AutumnCropNitrogenUptakeResponse), 200)]
        public async Task<ActionResult<AutumnCropNitrogenUptakeResponse>> GetAutumnCropNitrogenUptake(AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
        {
            return Ok(await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest));
        }

        [HttpGet("incorporation-delays")]
        [SwaggerOperation(Summary = "Retrieve all incorporation delays", Description = "Fetches a list of all incorporation delays available.")]
        [ProducesResponseType(typeof(IEnumerable<IncorporationDelayDto>), 200)]
        public async Task<ActionResult<IEnumerable<IncorporationDelayDto>?>> IncorporationDelays()
        {
            var delays = await _incorporationDelayService.FetchAllAsync();
            return Ok(delays);
        }

        [HttpGet("incorporation-delays/{id}")]
        [SwaggerOperation(Summary = "Retrieve incorporation delay by ID", Description = "Fetches a specific incorporation delay by its unique ID.")]
        [ProducesResponseType(typeof(IncorporationDelayDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncorporationDelayDto?>> IncorporationDelays(int id)
        {
            var delay = await _incorporationDelayService.FetchByIdAsync(id);
            if (delay == null)
            {
                return NotFound();
            }
            return Ok(delay);
        }

        [HttpGet("incorporation-delays/by-incorp-method/{methodId}")]
        [SwaggerOperation(Summary = "Retrieve incorporation delays by incorporation method ID", Description = "Fetches incorporation delays associated with a specific incorporation method.")]
        [ProducesResponseType(typeof(IEnumerable<IncorporationDelayDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<IncorporationDelayDto>?>> IncorporationDelaysByMethod(int methodId)
        {
            var delays = await _incorporationDelayService.FetchByIncorpMethodIdAsync(methodId);
            if (delays == null || !delays.Any())
            {
                return NotFound();
            }
            return Ok(delays);
        }

        [HttpGet("incorporation-methods")]
        [SwaggerOperation(Summary = "Retrieve all incorporation methods", Description = "Fetches a list of all incorporation methods available.")]
        [ProducesResponseType(typeof(IEnumerable<IncorporationMethodDto>), 200)]
        public async Task<ActionResult<IEnumerable<IncorporationMethodDto>?>> IncorporationMethods()
        {
            var methods = await _incorporationMethodService.FetchAllAsync();
            return Ok(methods);
        }

        [HttpGet("incorporation-methods/{id}")]
        [SwaggerOperation(Summary = "Retrieve incorporation method by ID", Description = "Fetches a specific incorporation method by its unique ID.")]
        [ProducesResponseType(typeof(IncorporationMethodDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncorporationMethodDto?>> IncorporationMethods(int id)
        {
            var method = await _incorporationMethodService.FetchByIdAsync(id);
            if (method == null)
            {
                return NotFound();
            }
            return Ok(method);
        }

        [HttpGet("incorporation-methods/by-app-method/{methodId}")]
        [SwaggerOperation(Summary = "Retrieve incorporation methods by application method ID", Description = "Fetches incorporation methods associated with a specific application method ID.")]
        [ProducesResponseType(typeof(IEnumerable<IncorporationMethodDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<IncorporationMethodDto>?>> IncorporationMethodsByMethodId(int methodId)
        {
            var methods = await _incorporationMethodService.FetchByAppMethodIdAsync(methodId);
            if (methods == null || !methods.Any())
            {
                return NotFound();
            }
            return Ok(methods);
        }

        [HttpGet("manure-groups")]
        [SwaggerOperation(Summary = "Retrieve all manure groups", Description = "Fetches a list of all manure groups available.")]
        [ProducesResponseType(typeof(IEnumerable<ManureGroupDto>), 200)]
        public async Task<ActionResult<IEnumerable<ManureGroupDto>?>> ManureGroups()
        {
            var groups = await _manureGroupService.FetchAllAsync();
            return Ok(groups);
        }

        [HttpGet("manure-groups/{id}")]
        [SwaggerOperation(Summary = "Retrieve manure group by ID", Description = "Fetches a specific manure group by its unique ID.")]
        [ProducesResponseType(typeof(ManureGroupDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ManureGroupDto?>> ManureGroups(int id)
        {
            var group = await _manureGroupService.FetchByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }

        [HttpGet("manure-types")]
        [SwaggerOperation(
            Summary = "Retrieve all manure types or filter by criteria",
            Description = "Fetches all manure types if no filters are provided. You can filter by optional parameters such as manureGroupId, manureTypeCategoryId, countryId, highReadilyAvailableNitrogen, and isLiquid."
        )]
        [ProducesResponseType(typeof(IEnumerable<ManureTypeDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ManureTypeDto>?>> ManureTypes(
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
                return NotFound("No manure types found matching the specified criteria.");
            }

            return Ok(manureTypes);
        }


        [HttpGet("manure-types/{id}")]
        [SwaggerOperation(Summary = "Retrieve manure type by ID", Description = "Fetches a specific manure type by its unique ID.")]
        [ProducesResponseType(typeof(ManureTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ManureTypeDto?>> ManureTypes(int id)
        {
            var type = await _manureTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                return NotFound($"Manure type with ID {id} not found.");
            }
            return Ok(type);
        }


        [HttpGet("manure-type-categories")]
        [SwaggerOperation(Summary = "Retrieve all manure type categories", Description = "Fetches a list of all manure type categories available.")]
        [ProducesResponseType(typeof(IEnumerable<ManureTypeCategoryDto>), 200)]
        public async Task<ActionResult<IEnumerable<ManureTypeCategoryDto>?>> ManureTypeCategories()
        {
            var categories = await _manureTypeCategoryService.FetchAllAsync();
            return Ok(categories);
        }

        [HttpGet("manure-type-categories/{id}")]
        [SwaggerOperation(Summary = "Retrieve manure type category by ID", Description = "Fetches a specific manure type category by its unique ID.")]
        [ProducesResponseType(typeof(ManureTypeCategoryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ManureTypeCategoryDto?>> ManureTypeCategories(int id)
        {
            var category = await _manureTypeCategoryService.FetchByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpGet("moisture-types")]
        [SwaggerOperation(Summary = "Retrieve all moisture types", Description = "Fetches a list of all moisture types available.")]
        [ProducesResponseType(typeof(IEnumerable<MoistureTypeDto>), 200)]
        public async Task<ActionResult<IEnumerable<MoistureTypeDto>?>> MoistureTypes()
        {
            var types = await _moistureTypeService.FetchAllAsync();
            return Ok(types);
        }

        [HttpGet("moisture-types/{id}")]
        [SwaggerOperation(Summary = "Retrieve moisture type by ID", Description = "Fetches a specific moisture type by its unique ID.")]
        [ProducesResponseType(typeof(MoistureTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MoistureTypeDto?>> MoistureTypes(int id)
        {
            var type = await _moistureTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                return NotFound();
            }
            return Ok(type);
        }

        [HttpGet("rain-types")]
        [SwaggerOperation(Summary = "Retrieve all rain types", Description = "Fetches a list of all rain types available.")]
        [ProducesResponseType(typeof(IEnumerable<RainTypeDto>), 200)]
        public async Task<ActionResult<IEnumerable<RainTypeDto>?>> RainTypes()
        {
            var types = await _rainTypeService.FetchAllAsync();
            return Ok(types);
        }

        [HttpGet("rain-types/{id}")]
        [SwaggerOperation(Summary = "Retrieve rain type by ID", Description = "Fetches a specific rain type by its unique ID.")]
        [ProducesResponseType(typeof(RainTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RainTypeDto?>> RainTypes(int id)
        {
            var type = await _rainTypeService.FetchByIdAsync(id);
            if (type == null)
            {
                return NotFound();
            }
            return Ok(type);
        }

        [HttpGet("sub-soils")]
        [SwaggerOperation(Summary = "Retrieve all sub-soils", Description = "Fetches a list of all sub-soils available.")]
        [ProducesResponseType(typeof(IEnumerable<SubSoilDto>), 200)]
        public async Task<ActionResult<IEnumerable<SubSoilDto>?>> SubSoils()
        {
            var soils = await _subSoilService.FetchAllAsync();
            return Ok(soils);
        }

        [HttpGet("sub-soils/{id}")]
        [SwaggerOperation(Summary = "Retrieve sub-soil by ID", Description = "Fetches a specific sub-soil by its unique ID.")]
        [ProducesResponseType(typeof(SubSoilDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SubSoilDto?>> SubSoils(int id)
        {
            var soil = await _subSoilService.FetchByIdAsync(id);
            if (soil == null)
            {
                return NotFound();
            }
            return Ok(soil);
        }

        [HttpGet("top-soils")]
        [SwaggerOperation(Summary = "Retrieve all top-soils", Description = "Fetches a list of all top-soils available.")]
        [ProducesResponseType(typeof(IEnumerable<TopSoilDto>), 200)]
        public async Task<ActionResult<IEnumerable<TopSoilDto>?>> TopSoils()
        {
            var soils = await _topSoilService.FetchAllAsync();
            return Ok(soils);
        }

        [HttpGet("top-soils/{id}")]
        [SwaggerOperation(Summary = "Retrieve top-soil by ID", Description = "Fetches a specific top-soil by its unique ID.")]
        [ProducesResponseType(typeof(TopSoilDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TopSoilDto?>> TopSoils(int id)
        {
            var soil = await _topSoilService.FetchByIdAsync(id);
            if (soil == null)
            {
                return NotFound();
            }
            return Ok(soil);
        }

        [HttpGet("windspeeds")]
        [SwaggerOperation(Summary = "Retrieve all windspeeds", Description = "Fetches a list of all windspeeds available.")]
        [ProducesResponseType(typeof(IEnumerable<WindspeedDto>), 200)]
        public async Task<ActionResult<IEnumerable<WindspeedDto>?>> Windspeeds()
        {
            var windspeeds = await _windspeedService.FetchAllAsync();
            return Ok(windspeeds);
        }

        [HttpGet("windspeeds/{id}")]
        [SwaggerOperation(Summary = "Retrieve windspeed by ID", Description = "Fetches a specific windspeed by its unique ID.")]
        [ProducesResponseType(typeof(WindspeedDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WindspeedDto?>> Windspeeds(int id)
        {
            var windspeed = await _windspeedService.FetchByIdAsync(id);
            if (windspeed == null)
            {
                return NotFound();
            }
            return Ok(windspeed);
        }








    }
}
