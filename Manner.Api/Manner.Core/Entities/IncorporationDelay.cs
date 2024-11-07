using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class IncorporationDelay
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int? Hours { get; set; }
    public int? CumulativeHours { get; set; }
    public string? ApplicableFor { get; set; }
}
