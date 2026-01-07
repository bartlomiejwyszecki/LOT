using Orders.Application.DTOs;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    OrderStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IQuery<PagedResult<OrderDto>>;

