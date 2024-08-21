namespace Manner.Application.DTOs
{
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

    public class EffectiveRainfallRequest
    {        
        public int CropTypeId { get; set; }

        public DateOnly ApplicationDate { get; set; }

        public DateOnly EndOfDrainageDate { get; set; }

        public string ClimateDataPostcode {  get; set; }= string.Empty;
    }

    public class EffectiveRainfallResponse
    {
        public int CropTypeId { get; set; }
        public string? CropType { get; set; }

        public DateOnly ApplicationDate { get; set; }

        public DateOnly EndOfDrainageDate { get; set; }

        public string ClimateDataPostcode { get; set; } = string.Empty;
    }

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
}
