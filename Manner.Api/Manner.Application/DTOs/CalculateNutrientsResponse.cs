namespace Manner.Application.DTOs;

public class CalculateNutrientsResponse
{
    public CalculateNutrientsResponse()
    {
        Results = new List<NutrientsResponse>();
    }
    public List<NutrientsResponse> Results { get; set; }
}

