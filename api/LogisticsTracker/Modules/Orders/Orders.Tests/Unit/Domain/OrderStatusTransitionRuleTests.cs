using Orders.Domain.Rules;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Unit.Domain;

public class OrderStatusTransitionRuleTests
{
    #region ValidateTransition Tests

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Pending, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Processing)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Processing, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Processing, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered)]
    public void ValidateTransition_ShouldNotThrow_WhenTransitionIsValid(
        OrderStatus currentStatus, 
        OrderStatus newStatus)
    {
        // Act
        var action = () => OrderStatusTransitionRule.ValidateTransition(currentStatus, newStatus);

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Processing)]
    [InlineData(OrderStatus.Pending, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Pending, OrderStatus.Delivered)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Delivered)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Pending)]
    [InlineData(OrderStatus.Processing, OrderStatus.Pending)]
    [InlineData(OrderStatus.Processing, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Processing, OrderStatus.Delivered)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Pending)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled)]
    public void ValidateTransition_ShouldThrow_WhenTransitionIsInvalid(
        OrderStatus currentStatus, 
        OrderStatus newStatus)
    {
        // Act
        var action = () => OrderStatusTransitionRule.ValidateTransition(currentStatus, newStatus);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Invalid status transition from {currentStatus} to {newStatus}*");
    }

    [Theory]
    [InlineData(OrderStatus.Delivered, OrderStatus.Pending)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Processing)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Cancelled)]
    public void ValidateTransition_ShouldThrow_WhenTransitionFromDeliveredState(
        OrderStatus currentStatus, 
        OrderStatus newStatus)
    {
        // Act
        var action = () => OrderStatusTransitionRule.ValidateTransition(currentStatus, newStatus);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Invalid status transition from {currentStatus} to {newStatus}*");
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Pending)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Processing)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Delivered)]
    public void ValidateTransition_ShouldThrow_WhenTransitionFromCancelledState(
        OrderStatus currentStatus, 
        OrderStatus newStatus)
    {
        // Act
        var action = () => OrderStatusTransitionRule.ValidateTransition(currentStatus, newStatus);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Invalid status transition from {currentStatus} to {newStatus}*");
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void ValidateTransition_ShouldNotThrow_WhenTransitionToSameStatus(OrderStatus status)
    {
        // Act
        var action = () => OrderStatusTransitionRule.ValidateTransition(status, status);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region GetValidNextStatuses Tests

    [Fact]
    public void GetValidNextStatuses_ShouldReturnConfirmedAndCancelled_WhenPending()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Pending);

        // Assert
        validStatuses.Should().BeEquivalentTo(new[] { OrderStatus.Confirmed, OrderStatus.Cancelled });
    }

    [Fact]
    public void GetValidNextStatuses_ShouldReturnProcessingAndCancelled_WhenConfirmed()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Confirmed);

        // Assert
        validStatuses.Should().BeEquivalentTo(new[] { OrderStatus.Processing, OrderStatus.Cancelled });
    }

    [Fact]
    public void GetValidNextStatuses_ShouldReturnShippedAndCancelled_WhenProcessing()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Processing);

        // Assert
        validStatuses.Should().BeEquivalentTo(new[] { OrderStatus.Shipped, OrderStatus.Cancelled });
    }

    [Fact]
    public void GetValidNextStatuses_ShouldReturnOnlyDelivered_WhenShipped()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Shipped);

        // Assert
        validStatuses.Should().BeEquivalentTo(new[] { OrderStatus.Delivered });
    }

    [Fact]
    public void GetValidNextStatuses_ShouldReturnEmpty_WhenDelivered()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Delivered);

        // Assert
        validStatuses.Should().BeEmpty();
    }

    [Fact]
    public void GetValidNextStatuses_ShouldReturnEmpty_WhenCancelled()
    {
        // Act
        var validStatuses = OrderStatusTransitionRule.GetValidNextStatuses(OrderStatus.Cancelled);

        // Assert
        validStatuses.Should().BeEmpty();
    }

    #endregion

    #region IsTerminalState Tests

    [Theory]
    [InlineData(OrderStatus.Delivered, true)]
    [InlineData(OrderStatus.Cancelled, true)]
    [InlineData(OrderStatus.Pending, false)]
    [InlineData(OrderStatus.Confirmed, false)]
    [InlineData(OrderStatus.Processing, false)]
    [InlineData(OrderStatus.Shipped, false)]
    public void IsTerminalState_ShouldReturnCorrectValue(OrderStatus status, bool expectedIsTerminal)
    {
        // Act
        var isTerminal = OrderStatusTransitionRule.IsTerminalState(status);

        // Assert
        isTerminal.Should().Be(expectedIsTerminal);
    }

    #endregion
}
