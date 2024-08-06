using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace Manner.Api.Controllers
{
    [ApiController]
    [Route("api/manner")]
    //[Authorize]
    public class MannerController : ControllerBase
    {
        private readonly ILogger<MannerController> _logger;
        private readonly IClimateService _climateService;
        private readonly IApplicationMethodService _applicationMethodService;
        private readonly ICropTypeService _cropTypeService;


        public MannerController(ILogger<MannerController> logger, 
            IClimateService climateService, 
            IApplicationMethodService applicationMethodService,
            ICropTypeService cropTypeService)
        {
            _logger = logger;
            _climateService = climateService;
            _applicationMethodService = applicationMethodService;
            _cropTypeService = cropTypeService;
        }
         
        [HttpGet("climates/{postcode}")]
        public async Task<ActionResult<ClimateDto?>> Climates(string postcode)
        {           
            return Ok(await _climateService.FetchByPostcodeAsync(postcode));
        }

        [HttpGet("application-methods")]
        public async Task<ActionResult<IEnumerable<ApplicationMethodDto>?>> ApplicationMethods()
        {            
            return Ok(await _applicationMethodService.FetchAllAsync());
        }

        [HttpGet("crop-types")]
        public async Task<ActionResult<IEnumerable<CropTypeDto>?>> CropTypes()
        {
            return Ok(await _cropTypeService.FetchAllAsync());
        }

        //Get Autumn Crop Nitrogen Uptake
        [HttpPost("autumn-crop-nitrogen-uptake")]
        public async Task<ActionResult<int>> GetAutumnCropNitrogenUptake(AutumnCropNitrogenUptakeRequest autumnCropNitrogenUptakeRequest)
        {
            return Ok(await _cropTypeService.FetchCropUptakeFactorDefault(autumnCropNitrogenUptakeRequest));
        }
    }
}
