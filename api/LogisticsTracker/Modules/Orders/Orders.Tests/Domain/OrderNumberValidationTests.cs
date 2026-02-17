using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Domain;

public class OrderNumberValidationTests
{
    private static Address CreateValidAddress() =>
        new("Kwiatowa 15", "Katowice", "Śląskie", "40-850", "POL");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenOrderNumberIsNullOrWhitespace(string? invalidOrderNumber)
    {
        // Arrange & Act
        var action = () => Order.Create(invalidOrderNumber!, CreateValidAddress());

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("orderNumber")
            .WithMessage("Order number cannot be empty or whitespace.*");
    }

    [Fact]
    public void Create_ShouldSucceed_WithValidOrderNumber()
    {
        // Arrange & Act
        var order = Order.Create("ORD-12345", CreateValidAddress());

        // Assert
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be("ORD-12345");
    }
}