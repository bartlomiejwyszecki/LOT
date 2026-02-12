using Orders.Application.Commands.CreateOrder;
using Orders.Application.Interfaces;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Application.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrderAndReturnDto()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-001",
            Street: "Kwiatowa 15",
            City: "Katowice",
            State: "Śląskie",
            PostalCode: "40-850",
            Country: "POL"
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.OrderNumber.Should().Be("ORD-001");
        result.Status.Should().Be(OrderStatus.Pending);
        result.ShippingAddress.Street.Should().Be("Kwiatowa 15");
        result.ShippingAddress.City.Should().Be("Katowice");
        result.ShippingAddress.Country.Should().Be(CountryCode.POL);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryAddAsync()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-002",
            Street: "Testowa 1",
            City: "Warszawa",
            State: "Mazowieckie",
            PostalCode: "00-001",
            Country: "POL"
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockRepository.Verify(
            r => r.AddAsync(It.Is<Order>(o => o.OrderNumber == "ORD-002"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositorySaveChangesAsync()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-003",
            Street: "Główna 10",
            City: "Kraków",
            State: "Małopolskie",
            PostalCode: "30-001",
            Country: "POL"
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenAddressValidationFails()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-004",
            Street: "",
            City: "Katowice",
            State: "Śląskie",
            PostalCode: "40-850",
            Country: "POL"
        );

        // Act
        var action = async () => await _handler.HandleAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Street cannot be empty*");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenCityIsEmpty()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-005",
            Street: "Kwiatowa 15",
            City: "", 
            State: "Śląskie",
            PostalCode: "40-850",
            Country: "POL"
        );

        // Act
        var action = async () => await _handler.HandleAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("City cannot be empty*");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenCountryIsEmpty()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-006",
            Street: "Kwiatowa 15",
            City: "Katowice",
            State: "Śląskie",
            PostalCode: "40-850",
            Country: "" 
        );

        // Act
        var action = async () => await _handler.HandleAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Country code cannot be empty. Must be a valid ISO 3166-1 alpha-3 country code (e.g., POL, DEU, FRA). (Parameter 'countryCode')");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenCountryIsInInvalidFormat()
    {
        // Arrange
        var command = new CreateOrderCommand(
            OrderNumber: "ORD-006",
            Street: "Kwiatowa 15",
            City: "Katowice",
            State: "Śląskie",
            PostalCode: "40-850",
            Country: "Polska" 
        );

        // Act
        var action = async () => await _handler.HandleAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Country code '{command.Country}' is invalid. Must be a valid ISO 3166-1 alpha-3 country code (e.g., POL, DEU, FRA). (Parameter 'countryCode')");
    }
}