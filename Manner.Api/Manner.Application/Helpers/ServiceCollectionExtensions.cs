using FluentValidation;
using FluentValidation.AspNetCore;
using Manner.Application.Validators;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Manner.Api.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {            
            return AddServices(services,  configuration);
        }
               

        private static IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
        {

            Assembly assembly = Assembly.GetExecutingAssembly(); 

            var typesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<ServiceAttribute>() != null)
                .ToList();

            foreach (var type in typesWithAttribute)
            {
                var attribute = type.GetCustomAttribute<ServiceAttribute>();
                var interfaces = type.GetInterfaces();
                if (attribute != null)
                {
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
            }

            services.AddAutoMapper(assembly);

            // Register FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AutumnCropNitrogenUptakeValidator>();
            services.AddValidatorsFromAssemblyContaining<EffectiveRainfallRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<MannerApplicationValidator>();
            services.AddValidatorsFromAssemblyContaining<CalculateNutrientsRequestValidator>();

            return services;
        }

       
    }
}
