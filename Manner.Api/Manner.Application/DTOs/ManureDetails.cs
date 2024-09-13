namespace Manner.Application.DTOs;

public class ManureDetails()
{        
    public int ManureID { get; set; }
    public string Name { get; set; } = string.Empty;     
    
    public decimal? DryMatter { get; set; }
    public decimal? TotalN { get; set; }
    public decimal? NH4N { get; set; }
    public decimal? Uric { get; set; }
    public decimal? NO3N { get; set; }
    public decimal? P2O5 { get; set; }
    public decimal? K2O { get; set; }
    public decimal? SO3 { get; set; }
    public decimal? MgO { get; set; }

}