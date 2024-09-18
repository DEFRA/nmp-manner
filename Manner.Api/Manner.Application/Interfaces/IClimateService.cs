using Manner.Application.DTOs;
using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Interfaces;
public interface IClimateService : IService<ClimateDto>
{
    Task<ClimateDto?> FetchByPostcodeAsync(string postcode);

    Task<Rainfall?> FetchAverageAnualRainfall(string postcode);
    Task<RainfallPostApplicationResponse> FetchRainfallPostApplication(RainfallPostApplicationRequest rainfallPostApplicationRequest);
}
