﻿using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Interfaces;
public interface IClimateRepository : IRepository<Climate>
{
    Task<Climate?> FetchByPostcodeAsync(string postcode);
}
