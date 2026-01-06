using Orders.Application.DTOs;

namespace Orders.Application.Commands.CreateOrder;

public record CreateOrderCommand(
    string OrderNumber,
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country
) : ICommand<OrderDto>;

