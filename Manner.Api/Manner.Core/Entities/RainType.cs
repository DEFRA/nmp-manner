using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class RainType
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public int RainInMM { get; set; }
}
