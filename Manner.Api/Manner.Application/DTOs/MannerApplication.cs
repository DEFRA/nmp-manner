namespace Manner.Application.DTOs;

public class ManureApplication()
{
    public ManureDetails ManureDetails { get; set; } = new ManureDetails();
    public DateOnly ApplicationDate { get; set; }
    public ApplicationRate ApplicationRate { get; set; } = new ApplicationRate();
    public int ApplicationMethodID { get; set; }
    public int IncorporationMethodID { get; set; }
    public int IncorporationDelayID { get; set; }
    public NitrogenUptake AutumnCropNitrogenUptake { get; set; } = new NitrogenUptake();
    public DateOnly EndOfDrainageDate { get; set; }
    public int RainfallPostApplication { get; set; }    
    public int CropNUptake {  get; set; }
    public int WindspeedID { get; set; }
    public int RainTypeID { get; set; }
    public int TopsoilMoistureID { get; set; }
    //public string Postcode { get; set; } = postcode;
    //public int CountryID { get; set; } = countryID;
    //public int Easting { get; private set; }
    //public int Northing { get; private set; }

    
}

