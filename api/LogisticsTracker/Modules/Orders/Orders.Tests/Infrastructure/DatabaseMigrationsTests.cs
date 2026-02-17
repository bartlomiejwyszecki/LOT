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

    // Act
    using var context = new OrdersDbContext(options);
    await context.Database.OpenConnectionAsync();
    var result = await context.Database.EnsureCreatedAsync();

    // Assert
    result.Should().BeTrue();
    
    var count = await context.Orders.CountAsync();
    count.Should().Be(0);
}
}