using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Core.Interfaces
{
    public interface IServiceRegistrar
    {
        void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}
