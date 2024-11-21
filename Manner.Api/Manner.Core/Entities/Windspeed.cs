namespace Manner.Core.Entities;
public class Windspeed
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public int FromScale { get; set; }
    public int ToScale { get; set; }
}
