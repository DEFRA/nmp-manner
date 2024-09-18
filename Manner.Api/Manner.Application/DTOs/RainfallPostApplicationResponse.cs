namespace Manner.Application.DTOs;

public class RainfallPostApplicationResponse
{
    public Rainfall RainfallPostApplication { get; set; }

    public RainfallPostApplicationResponse()
    {
        RainfallPostApplication = new Rainfall();
    }
}
