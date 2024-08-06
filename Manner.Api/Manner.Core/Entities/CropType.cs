using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class CropType
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Use { get; set; } = string.Empty;
    public int CropUptakeFactor { get; set; } = 0;

}
