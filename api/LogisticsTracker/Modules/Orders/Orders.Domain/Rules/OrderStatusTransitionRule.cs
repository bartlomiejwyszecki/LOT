using Order.Domain.ValueObjects;

namespace Order.Domain.Rules;

/// <summary>
/// Defines and validates order status transitions according to the order state machine.
/// </summary>
public static class OrderStatusTransitionRule
{
    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions = new()
    {
        { OrderStatus.Pending, new HashSet<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
        { OrderStatus.Confirmed, new HashSet<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
        { OrderStatus.Processing, new HashSet<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
        { OrderStatus.Shipped, new HashSet<OrderStatus> { OrderStatus.Delivered } },
        { OrderStatus.Delivered, new HashSet<OrderStatus>() },
        { OrderStatus.Cancelled, new HashSet<OrderStatus>() }
    };

    /// <summary>
    /// Validates if a status transition is allowed. Throws if the transition is invalid.
    /// </summary>
    /// <param name="currentStatus">The current order status</param>
    /// <param name="newStatus">The desired new order status</param>
    /// <exception cref="InvalidOperationException">Thrown when the transition is invalid</exception>
    public static void ValidateTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        if (currentStatus == newStatus)
            return;

        if (!ValidTransitions.ContainsKey(currentStatus))
        {
            throw new InvalidOperationException(
                $"Unknown current status: {currentStatus}. Cannot validate transition.");
        }

        if (!ValidTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {currentStatus} to {newStatus}. " +
                $"Valid transitions from {currentStatus} are: {string.Join(", ", ValidTransitions[currentStatus])}");
        }
    }

    /// <summary>
    /// Gets all valid statuses that can be transitioned to from the current status.
    /// </summary>
    /// <param name="currentStatus">The current order status</param>
    /// <returns>Collection of valid next statuses, or empty set if status is unknown</returns>
    public static IReadOnlySet<OrderStatus> GetValidNextStatuses(OrderStatus currentStatus)
    {
        if (!ValidTransitions.ContainsKey(currentStatus))
            return new HashSet<OrderStatus>();

        return ValidTransitions[currentStatus];
    }

    /// <summary>
    /// Determines if a status is terminal (cannot transition to any other state).
    /// </summary>
    /// <param name="status">The status to check</param>
    public static bool IsTerminalState(OrderStatus status)
    {
        return ValidTransitions.ContainsKey(status) && 
               ValidTransitions[status].Count == 0;
    }
}

