namespace Manner.Application.DTOs;

public class RainfallPostApplicationRequest
{ 
    public DateOnly ApplicationDate { get; set; }

    public DateOnly EndOfSoilDrainageDate { get; set; }

    public string ClimateDataPostcode {  get; set; }= string.Empty;
}
