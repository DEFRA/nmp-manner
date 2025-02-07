﻿using Manner.Application.DTOs;
using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Interfaces;
public interface IApplicationMethodService : IService<ApplicationMethodDto>
{
    Task<IEnumerable<ApplicationMethodDto>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null);
}
