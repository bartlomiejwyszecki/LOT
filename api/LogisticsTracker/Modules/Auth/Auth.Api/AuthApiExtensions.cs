using Auth.Application;
using Auth.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Api;

public static class AuthApiExtensions
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthApplication();
        services.AddAuthInfrastructure(configuration);

        return services;
    }
}