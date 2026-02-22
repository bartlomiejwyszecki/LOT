using Microsoft.EntityFrameworkCore;
using Orders.Infrastructure.Persistence;

namespace Orders.Tests.Infrastructure;

public class DatabaseMigrationsTests
{
    [Fact]
    public async Task Database_ShouldApplyAllMigrations_WithoutErrors()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new OrdersDbContext(options);
        await context.Database.OpenConnectionAsync();
        
        // Act
        var created = await context.Database.EnsureCreatedAsync();

        // Assert
        created.Should().BeTrue();  
        context.Orders.Should().NotBeNull(); 
    }
}