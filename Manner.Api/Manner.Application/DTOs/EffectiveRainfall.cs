namespace Manner.Application.DTOs;

public class EffectiveRainfall
{
    public EffectiveRainfall()
    {
        Value = 0;
        Unit = "mm";
    }
    public int Value { get; set; }

    public string Unit { get; set; }
}
