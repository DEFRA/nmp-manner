namespace Manner.Application.DTOs;

public class AutumnCropNitrogenUptakeResponse
{
    public AutumnCropNitrogenUptakeResponse()
    {
        NitrogenUptake = new();           
    }
    public int CropTypeId { get; set; }
    
    public string? CropType { get; set; }

    public NitrogenUptake NitrogenUptake {  get; set; }


}
