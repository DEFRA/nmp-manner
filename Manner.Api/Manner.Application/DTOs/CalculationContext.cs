namespace Manner.Application.DTOs;

public sealed class CalculationContext
{    
    public required ClimateDto Climate { get; set; }
    public required CropTypeDto CropType { get; set; }
    public required TopSoilDto TopSoil { get; set; }
    public required SubSoilDto SubSoil { get; set; }

    public List<ClimateTypeDto> ClimateTypes { get; set; } = new();

    public int RunType { get; set; }
}

