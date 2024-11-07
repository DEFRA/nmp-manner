using Manner.Application.Interfaces;

namespace Manner.Application.DTOs;

public class CalculateNutrientsRequest 
{
    public CalculateNutrientsRequest()
    {
        Field= new FieldDetail();
        ManureApplications = new List<ManureApplication>();
        Postcode= string.Empty;
        CountryID = default(int);
    }
    /// <summary>
    /// MannerEngland = 1,
    /// MannerScotland = 2,
    /// PlanetEngland = 3,
    /// PlanetScotland = 4
    /// </summary>
    public int RunType {  get; set; }

    public string Postcode { get; set; }

    public int CountryID {  get; set; }
    public FieldDetail Field { get; set; }

    public List<ManureApplication> ManureApplications { get; set; }

    

}

