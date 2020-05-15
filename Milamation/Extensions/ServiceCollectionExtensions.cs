using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Milamation.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x =>
            {
                return (x.GetInterfaces().Contains(typeof(T)) || x.BaseType == typeof(T)) && !x.IsAbstract;
            }));

            foreach (var type in typesFromAssemblies)
            {
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }

            return services;
        }
    }
}
