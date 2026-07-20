namespace Manner.Application.DTOs;

public sealed class CalculationContext
{
    public ClimateDto Climate { get; set; } = default!;

    public CropTypeDto CropType { get; set; } = default!;

    public TopSoilDto TopSoil { get; set; } = default!;

    public SubSoilDto SubSoil { get; set; } = default!;

    public List<ClimateTypeDto> ClimateTypes { get; set; } = new();

    public int RunType { get; set; }
}

