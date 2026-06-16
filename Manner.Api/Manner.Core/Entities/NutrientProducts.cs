namespace Manner.Core.Entities
{
    public class NutrientProduct
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NutrientID { get; set; }
        public decimal NutrientPercentage { get; set; } = 0.0m;
        public bool IsNutrientDefaultProduct { get; set; } = false;        
        public string MeasurementUnit { get; set; } = string.Empty;
    }
}
