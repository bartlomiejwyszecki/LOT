using Orders.Application.Interfaces;
using Orders.Application.Queries.GetOrderByOrderNumber;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Unit.Application.Queries;

public class GetOrderByOrderNumberQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly GetOrderByOrderNumberQueryHandler _handler;

    public GetOrderByOrderNumberQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _handler = new GetOrderByOrderNumberQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WhenOrderNumberExists()
    {
        // Arrange
        var orderNumber = "ORD-001";
        var query = new GetOrderByOrderNumberQuery(orderNumber);

        var address = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var order = Order.Create(orderNumber, address);

        _mockRepository
            .Setup(r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be(orderNumber);
        result.Status.Should().Be(OrderStatus.Pending);
        result.ShippingAddress.Street.Should().Be("Kwiatowa");
        result.ShippingAddress.City.Should().Be("Katowice");

        _mockRepository.Verify(
            r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenOrderNumberDoesNotExist()
    {
        // Arrange
        var orderNumber = "ORD-999";
        var query = new GetOrderByOrderNumberQuery(orderNumber);

        _mockRepository
            .Setup(r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(
            r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldConvertOrderToDto()
    {
        // Arrange
        var orderNumber = "ORD-002";
        var query = new GetOrderByOrderNumberQuery(orderNumber);

        var address = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var order = Order.Create(orderNumber, address);
        order.UpdateStatus(OrderStatus.Confirmed);

        _mockRepository
            .Setup(r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.OrderNumber.Should().Be(order.OrderNumber);
        result.Status.Should().Be(OrderStatus.Confirmed);
        result.OrderDate.Should().Be(order.OrderDate);
        result.CreatedAt.Should().Be(order.CreatedAt);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectOrderNumber()
    {
        // Arrange
        var orderNumber = "ORD-003";
        var query = new GetOrderByOrderNumberQuery(orderNumber);

        _mockRepository
            .Setup(r => r.GetByOrderNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        _mockRepository.Verify(
            r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldBeCaseSensitiveForOrderNumber()
    {
        // Arrange
        var orderNumber = "ORD-004";
        var query = new GetOrderByOrderNumberQuery(orderNumber);

        var address = new Address("Testowa", "Warszawa", "Mazovia", "00-001", "POL");
        var order = Order.Create(orderNumber, address);

        _mockRepository
            .Setup(r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();

        _mockRepository.Verify(
            r => r.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
