using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Application.DTOs;

public record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public AddressDto ShippingAddress { get; init; } = null!;
    public DateTime OrderDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public static OrderDto FromEntity(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            ShippingAddress = AddressDto.FromEntity(order.ShippingAddress),
            OrderDate = order.OrderDate,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}

