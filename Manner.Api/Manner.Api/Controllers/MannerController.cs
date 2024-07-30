using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace Manner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MannerController : ControllerBase
    {
        private readonly ILogger<MannerController> _logger;

        public MannerController(ILogger<MannerController> logger)
        {
            _logger = logger;
        }
                
        //Get Autumn Crop Nitrogen Uptake

        //[HttpPost(Name = "GetAutumnCropNitrogenUptake")]
        //[Authorize]
        //[Route("api/autumn-crop-nitrogen-uptake")]
        //public Task<int?> GetAutumnCropNitrogenUptake()
        //{
        //    return null;
        //    //var rng = new Random();
        //   // return 1;
        //}        
    }
}
