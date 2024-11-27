using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class ManureType
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ManureGroupID { get; set; }
    public int ManureTypeCategoryID { get; set; }
    public int CountryID { get; set; }
    public bool HighReadilyAvailableNitrogen { get; set; }
    public bool IsLiquid { get; set; }
    public decimal DryMatter { get; set; }
    public decimal TotalN { get; set; }
    public decimal NH4N { get; set; }
    public decimal Uric { get; set; }
    public decimal NO3N { get; set; }
    public decimal P2O5 { get; set; }
    public decimal K2O { get; set; }
    public decimal SO3 { get; set; }
    public decimal MgO { get; set; }
    public int P2O5Available { get; set; }
    public int K2OAvailable { get; set; }
    public decimal NMaxConstant { get; set; }
    public int ApplicationRateArable { get; set; }
    public int ApplicationRateGrass { get; set; }        
    public decimal SO3AvaiableAutumnOther { get; set; }
    public decimal SO3AvaiableAutumnOsrGrass { get; set; }
    public decimal SO3AvailableSpring { get; set; }
}
