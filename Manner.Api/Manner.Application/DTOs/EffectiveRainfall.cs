namespace Manner.Application.DTOs;

public class RainfallPostApplication
{
    public RainfallPostApplication()
    {
        Value = 0;
        Unit = "mm";
    }
    public int Value { get; set; }

    public string Unit { get; set; }
}
