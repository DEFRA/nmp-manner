using Manner.Application.DTOs;
using Manner.Core.Entities;

namespace Manner.Application.Calculators;

public sealed class MannerCalculatorInput
{
    public required FieldDetail Field { get; init; }
    public required ClimateDto Climate { get; init; }
    public required CropTypeDto CropType { get; init; }
    public required ManureApplication ManureApplication { get; init; }
    public required ManureTypeDto ManureType { get; init; }
    public IncorporationDelayDto? IncorporationDelay { get; init; }
    public required TopSoilDto TopSoil { get; init; }
    public required SubSoilDto SubSoil { get; init; }
    public required List<ClimateTypeDto> ClimateTypes { get; init; }
    public required int RunType { get; init; }
}
