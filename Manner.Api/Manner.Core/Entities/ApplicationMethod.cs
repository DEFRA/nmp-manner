using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class ApplicationMethod
{
    public ApplicationMethod()
    {
        Name = string.Empty;
    }

    public int ID { get; set; }
    public string Name { get; set; }
    public string? ApplicableForGrass { get; set; }
    public string? ApplicableForArableAndHorticulture { get; set; }
}
