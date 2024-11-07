namespace Manner.Application.DTOs;

public class Rainfall
{
    public Rainfall()
    {
        Value = 0;
        Unit = "mm";
    }
    public int Value { get; set; }

    public string Unit { get; set; }
}
