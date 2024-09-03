namespace Manner.Application.DTOs;

public class ManureDetails()
{        
    public int ManureID { get; set; }
    public string Name { get; set; } = string.Empty;     
    
    public decimal? DryMatter { get; set; }
    public decimal? TotalN { get; set; }
    public decimal? NH4N { get; set; }
    public decimal? Uric { get; set; }
    public decimal NO3N { get; set; }
    public decimal TotalP2O5 { get; set; }
    public decimal TotalK2O { get; set; }
    public decimal TotalSO3 { get; set; }
    public decimal TotalMgO { get; set; }

}