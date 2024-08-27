namespace Manner.Application.DTOs;

public class EffectiveRainfallRequest
{        
    public string Postcode { get; set; }

    public DateOnly ApplicationDate { get; set; }

    public DateOnly EndSoilDrainageDate { get; set; }

}
