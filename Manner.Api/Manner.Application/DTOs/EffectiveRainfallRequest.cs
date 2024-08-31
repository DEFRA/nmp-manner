namespace Manner.Application.DTOs;

public class EffectiveRainfallRequest
{        
    public int CropTypeId { get; set; }

    public DateOnly ApplicationDate { get; set; }

    public DateOnly EndOfSoilDrainageDate { get; set; }

    public string Postcode {  get; set; }= string.Empty;
}
