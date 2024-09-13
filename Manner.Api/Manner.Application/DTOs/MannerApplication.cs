namespace Manner.Application.DTOs;

public class MannerApplication(string postcode, int countryID)
{
    public ManureDetails ManureDetails { get; set; } = new ManureDetails();
    public DateOnly ApplicationDate { get; set; }
    public ApplicationRate ApplicationRate { get; set; } = new ApplicationRate();
    public int ApplicationMethodID { get; set; }
    public int IncorporationMethodID { get; set; }
    public int IncorporationDelayID { get; set; }
    public NitrogenUptake AutumnCropNitrogenUptake { get; set; } = new NitrogenUptake();
    public DateOnly EndOfDrainageDate { get; set; }
    public EffectiveRainfall? EffectiveRainfall
    {
        get
        {
            return new EffectiveRainfall
            {
                Value = Convert.ToInt32(this.RainfallTotal - this.EvapotranspirationTotal),
                Unit = "mm"
            };
        }
        set { }
    }
    public int WindspeedID { get; set; }
    public int RainTypeID { get; set; }
    public int TopsoilMoistureID { get; set; }
    public string Postcode { get; set; } = postcode;
    public int CountryID { get; set; } = countryID;

    public double RainfallTotal { get;  set; }

    public double EvapotranspirationTotal { get; set; }

    public int Easting {  get; private set; }
    public int Northing {  get; private set; }
}

