using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;
using Orders.Infrastructure.Persistence.Repositories;
using Orders.Tests.Infrastructure.Fixtures;

namespace Orders.Tests.Infrastructure.Persistence;

public class OrderRepositoryTests : IAsyncLifetime
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
    public async Task GetByIdAsync_ShouldReturnOrderWithAllPropertiesMappedCorrectly()
    {
        // Arrange
        var builder = new TestDataBuilder()
            .WithOrderNumber("ORD-GET-001")
            .WithStreet("Kwiatowa 15")
            .WithCity("Katowice")
            .WithState("Śląskie")
            .WithPostalCode("40-850")
            .WithCountryCode("POL")
            .WithStatus(OrderStatus.Confirmed);
        var order = builder.Build();
        
        await _repository.AddAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Act
        var retrievedOrder = await _repository.GetByIdAsync(order.Id, CancellationToken.None);

        // Assert
        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.Id.Should().Be(order.Id);
        retrievedOrder.OrderNumber.Should().Be("ORD-GET-001");
        retrievedOrder.Status.Should().Be(OrderStatus.Confirmed);
        retrievedOrder.OrderDate.Should().Be(order.OrderDate);
        retrievedOrder.CreatedAt.Should().Be(order.CreatedAt);
        retrievedOrder.UpdatedAt.Should().NotBeNull();
        retrievedOrder.ShippingAddress.Should().NotBeNull();
        retrievedOrder.ShippingAddress.Street.Should().Be("Kwiatowa 15");
        retrievedOrder.ShippingAddress.City.Should().Be("Katowice");
        retrievedOrder.ShippingAddress.State.Should().Be("Śląskie");
        retrievedOrder.ShippingAddress.PostalCode.Should().Be("40-850");
        retrievedOrder.ShippingAddress.Country.Should().Be(CountryCode.POL);
    }

    [Fact]
    public async Task GetByOrderNumberAsync_ShouldReturnCorrectOrderWithCaseSensitiveValue()
    {
        // Arrange
        var order1 = new TestDataBuilder().WithOrderNumber("ORD-NUM-001").Build();
        var order2 = new TestDataBuilder().WithOrderNumber("ord-num-001").Build();
        
        await _repository.AddAsync(order1, CancellationToken.None);
        await _repository.AddAsync(order2, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Act
        var retrievedOrder = await _repository.GetByOrderNumberAsync("ORD-NUM-001", CancellationToken.None);

        // Assert
        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.OrderNumber.Should().Be("ORD-NUM-001");
        retrievedOrder!.Id.Should().Be(order1.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrdersInDatabasePreservingAllProperties()
    {
        // Arrange
        var order1 = new TestDataBuilder()
            .WithOrderNumber("ORD-ALL-001")
            .WithCity("Katowice")
            .WithStatus(OrderStatus.Pending)
            .Build();
        
        var order2 = new TestDataBuilder()
            .WithOrderNumber("ORD-ALL-002")
            .WithCity("Kraków")
            .WithStatus(OrderStatus.Confirmed)
            .Build();
        
        var order3 = new TestDataBuilder()
            .WithOrderNumber("ORD-ALL-003")
            .WithCity("Warszawa")
            .WithStatus(OrderStatus.Confirmed)
            .Build();

        await _repository.AddAsync(order1, CancellationToken.None);
        await _repository.AddAsync(order2, CancellationToken.None);
        await _repository.AddAsync(order3, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Act
        var orders = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        orders.Should().HaveCount(3);
        
        var retrievedOrder1 = orders.First(o => o.OrderNumber == "ORD-ALL-001");
        retrievedOrder1.Id.Should().Be(order1.Id);
        retrievedOrder1.Status.Should().Be(OrderStatus.Pending);
        retrievedOrder1.ShippingAddress.City.Should().Be("Katowice");
        retrievedOrder1.CreatedAt.Should().Be(order1.CreatedAt);
        
        var retrievedOrder2 = orders.First(o => o.OrderNumber == "ORD-ALL-002");
        retrievedOrder2.Id.Should().Be(order2.Id);
        retrievedOrder2.Status.Should().Be(OrderStatus.Confirmed);
        retrievedOrder2.ShippingAddress.City.Should().Be("Kraków");
        
        var retrievedOrder3 = orders.First(o => o.OrderNumber == "ORD-ALL-003");
        retrievedOrder3.Id.Should().Be(order3.Id);
        retrievedOrder3.Status.Should().Be(OrderStatus.Confirmed);
        retrievedOrder3.ShippingAddress.City.Should().Be("Warszawa");
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewOrderWithCreatedAtTimestamp()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var order = new TestDataBuilder().WithOrderNumber("ORD-ADD-001").Build();

        // Act
        await _repository.AddAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var afterCreation = DateTime.UtcNow;
        var savedOrder = await _repository.GetByIdAsync(order.Id, CancellationToken.None);
        
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderNumber.Should().Be("ORD-ADD-001");
        savedOrder.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        savedOrder.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrderPreservingCreatedAtAndUpdatingUpdatedAt()
    {
        // Arrange
        var order = new TestDataBuilder().WithOrderNumber("ORD-UPD-001").Build();

        await _repository.AddAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        var originalCreatedAt = order.CreatedAt;

        await Task.Delay(100);

        // Act
        var beforeUpdate = DateTime.UtcNow;

        order.UpdateStatus(OrderStatus.Confirmed);

        await _repository.UpdateAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        var updatedOrder = await _repository.GetByIdAsync(order.Id, CancellationToken.None);

        updatedOrder.Should().NotBeNull();
        updatedOrder!.CreatedAt.Should().Be(originalCreatedAt);
        updatedOrder.UpdatedAt.Should().NotBeNull();
        updatedOrder.UpdatedAt!.Value.Should().BeOnOrAfter(beforeUpdate);
        updatedOrder.UpdatedAt!.Value.Should().BeOnOrBefore(afterUpdate);
        updatedOrder.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveOrderAndMakeItNoLongerAvailable()
    {
        // Arrange
        var order = new TestDataBuilder().WithOrderNumber("ORD-DEL-001").Build();

        await _repository.AddAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        var orderId = order.Id;

        // Act
        await _repository.DeleteAsync(order, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var deletedOrder = await _repository.GetByIdAsync(orderId, CancellationToken.None);
        
        deletedOrder.Should().BeNull();
    }
}