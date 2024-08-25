namespace Manner.Application.DTOs;

public class EffectiveRainfallResponse
{
    public int CropTypeId { get; set; }
    public string? CropType { get; set; }

    public DateOnly ApplicationDate { get; set; }

    public DateOnly EndOfDrainageDate { get; set; }

    public string ClimateDataPostcode { get; set; } = string.Empty;
}
