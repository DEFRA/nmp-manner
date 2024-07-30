using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Configuration
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDependencies(configuration);
        }
    }
}
