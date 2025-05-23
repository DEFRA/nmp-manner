﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities;
public class IncorporationMethod
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public string? ApplicableForGrass { get; set; }
    public string? ApplicableForArableAndHorticulture { get; set; }
    
    public int SortOrder { get; set; }

}
