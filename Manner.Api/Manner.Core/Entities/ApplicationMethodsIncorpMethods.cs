namespace Manner.Core.Entities;

public class ApplicationMethodsIncorpMethods
{
    public int ApplicationMethodID { get; set; }
    public int IncorporationMethodID { get; set; }

    public required ApplicationMethod ApplicationMethod { get; set; }
    public required IncorporationMethod IncorporationMethod { get; set; }
}
