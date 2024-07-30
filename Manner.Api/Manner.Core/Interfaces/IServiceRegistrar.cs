using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Interfaces
{
    public interface IServiceRegistrar
    {
        void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}
