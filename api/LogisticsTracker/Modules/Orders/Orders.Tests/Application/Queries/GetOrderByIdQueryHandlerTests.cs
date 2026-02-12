using Orders.Application.Interfaces;
using Orders.Application.Queries.GetOrderById;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Application.Queries;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var address = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var order = Order.Create("ORD-001", address);

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be("ORD-001");
        result.Status.Should().Be(OrderStatus.Pending);
        result.ShippingAddress.Street.Should().Be("Kwiatowa");
        result.ShippingAddress.City.Should().Be("Katowice");

        _mockRepository.Verify(
            r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(
            r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldConvertOrderToDto()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var address = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var order = Order.Create("ORD-002", address);
        order.UpdateStatus(OrderStatus.Confirmed);

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.OrderNumber.Should().Be(order.OrderNumber);
        result.Status.Should().Be(order.Status);
        result.OrderDate.Should().Be(order.OrderDate);
        result.CreatedAt.Should().Be(order.CreatedAt);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        _mockRepository.Verify(
            r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}