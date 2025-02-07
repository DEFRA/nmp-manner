﻿using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>?> FetchAllAsync();
        Task<T?> FetchByIdAsync(int id);
    }
}
