using Manner.Core.Attributes;
using Manner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Manner.Infrastructure;

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    services.Add(new ServiceDescriptor(item, type, attribute.Lifetime));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                services.Add(new ServiceDescriptor(type, type, attribute.Lifetime));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MannerApiConnection")));
        
        return services;
    }
            
}
