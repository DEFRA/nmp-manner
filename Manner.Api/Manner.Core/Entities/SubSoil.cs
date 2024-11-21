namespace Manner.Core.Entities;
public class SubSoil
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public int VolumetricMeasure { get; set; } // New property
    public int AvailableWaterCapacity { get; set; } // New property
}