namespace Manner.Application.DTOs;

public class ApplicationRate
{
    public ApplicationRate()
    {
        Value = 0;
        Unit = "t/ha";
    }

    public decimal Value { get; set; }
    public string Unit { get; set; } 
}

