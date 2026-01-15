using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application;
using Orders.Infrastructure;

namespace Orders.Api;

public static class OrdersApiExtensions
{
    public static IServiceCollection AddOrdersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOrdersApplication();
        services.AddOrdersInfrastructure(configuration);

        return services;
    }
}
