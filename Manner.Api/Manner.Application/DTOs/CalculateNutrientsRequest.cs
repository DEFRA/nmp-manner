using Manner.Application.Interfaces;

namespace Manner.Application.DTOs;

public class CalculateNutrientsRequest 
{
    public CalculateNutrientsRequest()
    {
        Field= new FieldDetail();
        Applications = new List<MannerApplication>();
        Postcode= string.Empty;
    }

    public string Postcode {  get; set; }

    public FieldDetail Field { get; set; }

    public List<MannerApplication> Applications { get; set; }

    

}

