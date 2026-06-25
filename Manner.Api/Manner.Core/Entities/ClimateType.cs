namespace Manner.Core.Entities;

public class ClimateType
{
    public int MonthNumber { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int AE { get; set; }
    public int Rain { get; set; }
    public decimal MinTemp { get; set; }
    public decimal MaxTemp { get; set; }
    public decimal Sunshine { get; set; }
    public decimal WindSpeed { get; set; }
    public decimal MeanTemp { get; set; }
    public decimal RainDays { get; set; }
    public decimal HER { get; set; }
    public decimal SM { get; set; }
    public decimal PETG { get; set; }    
}
