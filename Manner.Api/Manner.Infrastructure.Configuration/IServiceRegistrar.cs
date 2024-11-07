using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Infrastructure.Configuration
{
    public interface IServiceRegistrar
    {
        void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}
