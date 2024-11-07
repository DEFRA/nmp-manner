using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Interfaces;
public interface IApplicationMethodRepository : IRepository<ApplicationMethod>
{
    Task<IEnumerable<ApplicationMethod>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null);
}
