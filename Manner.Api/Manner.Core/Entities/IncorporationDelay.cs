namespace Manner.Core.Entities;
public class IncorporationDelay
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public int? Hours { get; set; }
    public int? CumulativeHours { get; set; }
    public string? ApplicableFor { get; set; }
}
