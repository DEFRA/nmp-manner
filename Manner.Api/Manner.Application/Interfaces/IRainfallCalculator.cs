using Manner.Application.DTOs;

namespace Manner.Application.Interfaces
{
    public interface IRainfallCalculator
    {
        decimal CalculateRainfallPostApplication(ClimateDto climate, DateOnly applicationDate, DateOnly endSoilDrainageDate);
    }
}