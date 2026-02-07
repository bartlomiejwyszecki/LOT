using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Application.Queries.GetAllOrders;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Unit.Application.Queries;

public class GetAllOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly GetAllOrdersQueryHandler _handler;

    public GetAllOrdersQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _handler = new GetAllOrdersQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllOrders_WithDefaultPagination()
    {
        // Arrange
        var query = new GetAllOrdersQuery();

        var address1 = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var address2 = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var orders = new List<Order>
        {
            Order.Create("ORD-001", address1),
            Order.Create("ORD-002", address2)
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);

        _mockRepository.Verify(
            r => r.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldApplyPagination()
    {
        // Arrange
        var query = new GetAllOrdersQuery(PageNumber: 2, PageSize: 1);

        var address1 = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var address2 = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var orders = new List<Order>
        {
            Order.Create("ORD-001", address1),
            Order.Create("ORD-002", address2)
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().OrderNumber.Should().Be("ORD-002");
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByStatus()
    {
        // Arrange
        var query = new GetAllOrdersQuery(Status: OrderStatus.Confirmed);

        var address1 = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var address2 = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var order1 = Order.Create("ORD-001", address1);
        var order2 = Order.Create("ORD-002", address2);
        order2.UpdateStatus(OrderStatus.Confirmed);

        var orders = new List<Order> { order1, order2 };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be(OrderStatus.Confirmed);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByDateRange()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var query = new GetAllOrdersQuery(FromDate: now.AddDays(-1), ToDate: now.AddDays(1));

        var address = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var orders = new List<Order>
        {
            Order.Create("ORD-001", address)
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyResult_WhenNoOrdersMatch()
    {
        // Arrange
        var query = new GetAllOrdersQuery(Status: OrderStatus.Delivered);

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ShouldCombineStatusAndDateFilters()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var query = new GetAllOrdersQuery(
            Status: OrderStatus.Confirmed,
            FromDate: now.AddDays(-1),
            ToDate: now.AddDays(1)
        );

        var address1 = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
        var address2 = new Address("Glowna", "Krakow", "Malopolskie", "31-999", "POL");
        var order1 = Order.Create("ORD-001", address1);
        var order2 = Order.Create("ORD-002", address2);
        order2.UpdateStatus(OrderStatus.Confirmed);

        var orders = new List<Order> { order1, order2 };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be(OrderStatus.Confirmed);
        result.TotalCount.Should().Be(1);
    }
}
