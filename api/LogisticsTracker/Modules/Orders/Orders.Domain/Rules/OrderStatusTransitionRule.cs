using Orders.Domain.ValueObjects;

namespace Orders.Domain.Rules;

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

    public static IReadOnlySet<OrderStatus> GetValidNextStatuses(OrderStatus currentStatus)
    {
        if (!ValidTransitions.ContainsKey(currentStatus))
            return new HashSet<OrderStatus>();

        return ValidTransitions[currentStatus];
    }

    public static bool IsTerminalState(OrderStatus status)
    {
        return ValidTransitions.ContainsKey(status) && 
               ValidTransitions[status].Count == 0;
    }
}

