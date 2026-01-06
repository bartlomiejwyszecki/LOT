using Orders.Domain.ValueObjects;

namespace Orders.Application.DTOs;

public record UpdateOrderStatusDto
{
    public OrderStatus NewStatus { get; init; }
}

