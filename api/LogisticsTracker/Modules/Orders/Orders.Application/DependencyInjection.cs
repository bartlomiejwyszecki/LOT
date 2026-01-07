using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Orders.Application.Commands;
using Orders.Application.Queries;

namespace Orders.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type handlerInterface)
    {
        var handlerTypes = assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == handlerInterface))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == handlerInterface);

            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, handlerType);
            }
        }
    }
}