using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Interfaces;
using Orders.Infrastructure;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Persistence.Repositories;

namespace Orders.Tests.Infrastructure;

public class DependencyInjectionTests
{
    [Fact]
    public void AddOrdersInfrastructure_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:OrdersDatabase", "Data Source=:memory:" }
            })
            .Build();

        // Act
        services.AddOrdersInfrastructure(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetRequiredService<OrdersDbContext>();
        dbContext.Should().NotBeNull();

        var repository = serviceProvider.GetRequiredService<IOrderRepository>();
        repository.Should().NotBeNull();
        repository.Should().BeOfType<OrderRepository>();
    }
}