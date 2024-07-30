using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Manner.Infrastructure
{  

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var typesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<RepositoryAttribute>() != null)
                .ToList();

            foreach (var type in typesWithAttribute)
            {
                var attribute = type.GetCustomAttribute<RepositoryAttribute>();
                var interfaces = type.GetInterfaces();

                if (interfaces.Length > 0)
                {
                    foreach (var item in interfaces)
                    {
                        services.Add(new ServiceDescriptor(item, type, attribute.Lifetime));
                    }
                }
                else
                {
                    services.Add(new ServiceDescriptor(type, type, attribute.Lifetime));
                }
            }

            //// Find the implementation of IServiceRegistrar
            //var registrarType = assembly.GetTypes()
            //    .FirstOrDefault(t => typeof(IServiceRegistrar).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            //if (registrarType != null)
            //{
            //    // Create an instance of the registrar and call RegisterServices
            //    var registrar = (IServiceRegistrar)Activator.CreateInstance(registrarType);
            //    registrar?.RegisterServices(services, configuration);
            //}

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MannerApiConnection")));
            
            return services;
        }
                
    }
}
