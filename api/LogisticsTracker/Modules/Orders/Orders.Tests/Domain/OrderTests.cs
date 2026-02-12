using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Domain;

public class OrderTests
{
    private static Address CreateValidAddress() =>
        new("Kwiatowa 15", "Katowice", "Śląskie", "40-850", "POL");

    #region Create - Business Rules

    [Fact]
    public void Create_ShouldSetInitialStatusToPending()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        var order = Order.Create("ORD-001", address);

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Create_ShouldSetOrderDateToCurrentUtcTime()
    {
        // Arrange
        var address = CreateValidAddress();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var order = Order.Create("ORD-001", address);

        // Assert
        var afterCreation = DateTime.UtcNow;
        order.OrderDate.Should().BeOnOrAfter(beforeCreation);
        order.OrderDate.Should().BeOnOrBefore(afterCreation);
    }

    #endregion

    #region UpdateStatus - Valid Transitions

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled)]
    public void UpdateStatus_ShouldUpdateStatus_WhenTransitionFromPendingIsValid(OrderStatus newStatus)
    {
        // Arrange
        var order = Order.Create("ORD-001", CreateValidAddress());

        // Act
        order.UpdateStatus(newStatus);

        // Assert
        order.Status.Should().Be(newStatus);
    }

    [Fact]
    public void UpdateStatus_ShouldSetUpdatedAtTimestamp()
    {
        // Arrange
        var order = Order.Create("ORD-001", CreateValidAddress());
        var beforeUpdate = DateTime.UtcNow;

        // Act
        order.UpdateStatus(OrderStatus.Confirmed);

        // Assert
        var afterUpdate = DateTime.UtcNow;
        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt!.Value.Should().BeOnOrAfter(beforeUpdate);
        order.UpdatedAt!.Value.Should().BeOnOrBefore(afterUpdate);
    }

    #endregion

    #region UpdateStatus - Invalid Transitions

    [Theory]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    public void UpdateStatus_ShouldThrow_WhenTransitionFromPendingIsInvalid(OrderStatus invalidStatus)
    {
        // Arrange
        var order = Order.Create("ORD-001", CreateValidAddress());

        // Act
        var action = () => order.UpdateStatus(invalidStatus);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Invalid status transition from Pending to {invalidStatus}*");
    }

    [Fact]
    public void UpdateStatus_ShouldThrow_WhenOrderIsInTerminalState()
    {
        // Arrange
        var order = Order.Create("ORD-001", CreateValidAddress());
        order.UpdateStatus(OrderStatus.Cancelled); // Move to terminal state

        // Act
        var action = () => order.UpdateStatus(OrderStatus.Confirmed);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Invalid status transition from Cancelled to Confirmed*");
    }

    #endregion

    #region UpdateStatus - Full Lifecycle

    [Fact]
    public void Order_ShouldCompleteFullLifecycle_WhenFollowingValidTransitions()
    {
        // Arrange
        var order = Order.Create("ORD-001", CreateValidAddress());

        // Act & Assert - Full happy path
        order.Status.Should().Be(OrderStatus.Pending);

        order.UpdateStatus(OrderStatus.Confirmed);
        order.Status.Should().Be(OrderStatus.Confirmed);

        order.UpdateStatus(OrderStatus.Processing);
        order.Status.Should().Be(OrderStatus.Processing);

        order.UpdateStatus(OrderStatus.Shipped);
        order.Status.Should().Be(OrderStatus.Shipped);

        order.UpdateStatus(OrderStatus.Delivered);
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    #endregion
}