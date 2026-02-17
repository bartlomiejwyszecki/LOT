using Microsoft.EntityFrameworkCore;
using Orders.Infrastructure.Persistence.Repositories;
using Orders.Tests.Infrastructure.Fixtures;

namespace Orders.Tests.Infrastructure.Persistence;

public class UniqueOrderNumberConstraintTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;
    private OrderRepository _repository = null!;

    public async Task InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.InitializeAsync();
        _repository = new OrderRepository(_fixture.DbContext);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldThrowDbUpdateException_WhenDuplicateOrderNumberExists()
    {
        // Arrange
        var order1 = new TestDataBuilder().WithOrderNumber("ORD-UNIQUE-001").Build();
        var order2 = new TestDataBuilder().WithOrderNumber("ORD-UNIQUE-001").Build();

        await _repository.AddAsync(order1, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Act
        await _repository.AddAsync(order2, CancellationToken.None);
        var action = () => _repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSucceed_WhenMultipleOrdersHaveDifferentOrderNumbers()
    {
        // Arrange
        var order1 = new TestDataBuilder().WithOrderNumber("ORD-DIFF-001").Build();
        var order2 = new TestDataBuilder().WithOrderNumber("ORD-DIFF-002").Build();
        var order3 = new TestDataBuilder().WithOrderNumber("ORD-DIFF-003").Build();

        // Act
        await _repository.AddAsync(order1, CancellationToken.None);
        await _repository.AddAsync(order2, CancellationToken.None);
        await _repository.AddAsync(order3, CancellationToken.None);
        
        var action = () => _repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        await action.Should().NotThrowAsync();
        
        var orders = await _repository.GetAllAsync(CancellationToken.None);
        orders.Should().HaveCount(3);
    }
}