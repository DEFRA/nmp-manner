using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Interfaces
{
    public interface IService<T> where  T: class
    {
        Task<IEnumerable<T>?> FetchAllAsync();
        Task<T?> FetchByIdAsync(int id);
    }
}
