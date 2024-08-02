using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace Manner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class MannerController : ControllerBase
    {
        private readonly ILogger<MannerController> _logger;
        private readonly IClimateService _climateService;
        private readonly IApplicationMethodService _applicationMethodService;


        public MannerController(ILogger<MannerController> logger, 
            IClimateService climateService, 
            IApplicationMethodService applicationMethodService)
        {
            _logger = logger;
            _climateService = climateService;
            _applicationMethodService = applicationMethodService;
        }

        //Get Autumn Crop Nitrogen Uptake

        [HttpPost("autumn-crop-nitrogen-uptake")]        
        //[Route("api/autumn-crop-nitrogen-uptake")]
        public async Task<ActionResult<int?>> GetAutumnCropNitrogenUptake()
        {
            return 10;            
        }


        [HttpGet("Climates")]
        public async Task<ActionResult<IEnumerable<ClimateDto>?>> Climates()
        {           
            return Ok(await _climateService.FetchAllAsync());
        }

        [HttpGet("ApplicationMethods")]
        public async Task<ActionResult<IEnumerable<ApplicationMethodDto>?>> ApplicationMethods()
        {            
            return Ok(await _applicationMethodService.FetchAllAsync());
        }
    }
}
