using DH.Domain.Services.Seed;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DH.Database.MigrationUtility;

public static class ExtensionDI
{
    public static IServiceCollection LoadSeedServices(
        this IServiceCollection services)
    {
        Assembly.Load("DH.Application")!.GetTypesAssignableFrom<ISeedService>().ForEach((t) =>
        {
            services.AddScoped(typeof(ISeedService), t);
        });

        return services;
    }

    private static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
    {
        return assembly.GetTypesAssignableFrom(typeof(T));
    }

    private static List<Type> GetTypesAssignableFrom(this Assembly assembly, Type compareType)
    {
        List<Type> ret = new List<Type>();
        foreach (var type in assembly.DefinedTypes)
        {
            if (compareType.IsAssignableFrom(type) && compareType != type)
            {
                ret.Add(type);
            }
        }
        return ret;
    }
}
