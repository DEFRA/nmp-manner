using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class DryMatterType
{
    public DryMatterType() 
    {
        Min = 0;
        Max = 90;
        Value = 0;
    }
    public double Min {  get; set; }
    public double Max { get; set; }
    public double Value { get; set; }
}
