using Microsoft.Extensions.DependencyInjection;

namespace Manner.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RepositoryAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public RepositoryAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
    }
}
