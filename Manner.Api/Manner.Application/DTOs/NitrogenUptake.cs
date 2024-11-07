namespace Manner.Application.DTOs
{
    public class NitrogenUptake
    {
        public NitrogenUptake()
        {
            Value = 0;
            Unit = "kg/ha";
        }
        public int Value { get; set; }

        public string Unit { get; set; }
    }
        
 }
