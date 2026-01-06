using Orders.Application.DTOs;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus
) : ICommand<OrderDto>;

