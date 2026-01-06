using Orders.Application.DTOs;

namespace Orders.Application.Queries.GetOrderByOrderNumber;

public record GetOrderByOrderNumberQuery(string OrderNumber) : IQuery<OrderDto?>;

