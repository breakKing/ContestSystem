using System.Reflection;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        var mappers = assembly.DefinedTypes
            .Where(t => 
                t.ImplementedInterfaces.Any(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>))
                && t.IsClass)
            .ToList();
        
        foreach (var mapper in mappers)
        {
            var mapperInterface = mapper.ImplementedInterfaces.First(
                i => i.GetGenericTypeDefinition() == typeof(IMapper<,>));

            services.AddSingleton(mapperInterface, mapper);
        }

        return services;
    }
}
