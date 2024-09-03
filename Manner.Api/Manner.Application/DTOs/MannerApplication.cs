namespace Manner.Application.DTOs;

public class MannerApplication
{
    public MannerApplication()
    {
        ManureDetails = new ManureDetails();
        ApplicationRate = new ApplicationRate();
        AutumnCropNitrogenUptake = new NitrogenUptake();
        EffectiveRainfall = new EffectiveRainfall();
    }

    
    public ManureDetails ManureDetails { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public ApplicationRate ApplicationRate { get; set; }
    public int ApplicationMethodID { get; set; }
    public int IncorporationMethodID { get; set; }
    public int IncorporationDelayID { get; set; }
    public NitrogenUptake AutumnCropNitrogenUptake { get; set; }
    public DateOnly EndOfDrainageDate { get; set; }
    public EffectiveRainfall? EffectiveRainfall { get; set; }
    public int WindspeedID { get; set; }
    public int RainTypeID { get; set; }
    public int TopsoilMoistureID { get; set; }

    public int CountryCode {  get; set; }
}

