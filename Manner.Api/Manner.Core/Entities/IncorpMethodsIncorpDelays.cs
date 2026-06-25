namespace Manner.Core.Entities;

public class IncorpMethodsIncorpDelays
{
    public int IncorporationMethodID { get; set; }
    public int IncorporationDelayID { get; set; }

    public required IncorporationMethod IncorporationMethod { get; set; }
    public required IncorporationDelay IncorporationDelay { get; set; }
}
