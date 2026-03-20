using Auth.Application.Interfaces;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Persistence.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("AuthDatabase")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'AuthDatabase' or 'DefaultConnection' was not found.");

        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", AuthDbContext.SchemaName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

        services.AddSingleton<IJwtTokenService>(_ =>
            new JwtTokenService(
                configuration["Authentication:Jwt:SecretKey"]
                    ?? "dev-only-auth-secret-key-must-be-at-least-32-chars",
                configuration["Authentication:Jwt:Issuer"] ?? "LogisticsTracker",
                configuration["Authentication:Jwt:Audience"] ?? "LogisticsTracker",
                int.TryParse(configuration["Authentication:Jwt:ExpirationMinutes"], out var expirationMinutes)
                    ? expirationMinutes
                    : 15));

        return services;
    }
}