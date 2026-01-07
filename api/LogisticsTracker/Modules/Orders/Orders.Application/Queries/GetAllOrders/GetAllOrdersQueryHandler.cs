using System.Linq;
using Orders.Application.DTOs;
using Orders.Application.Interfaces;

namespace Orders.Application.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDto>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Orders.Domain.Entities.Order> filteredOrders;

        var allOrders = await _orderRepository.GetAllAsync(cancellationToken);

        filteredOrders = allOrders.AsQueryable();
  

        if (query.Status.HasValue)
        {
            filteredOrders = filteredOrders.Where(o => o.Status == query.Status.Value);
        }

        if (query.FromDate.HasValue)
        {
            filteredOrders = filteredOrders.Where(o => o.OrderDate >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            filteredOrders = filteredOrders.Where(o => o.OrderDate <= query.ToDate.Value);
        }

        var totalCount = filteredOrders.Count();
        var pagedOrders = filteredOrders
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();
        var orderDtos = pagedOrders.Select(OrderDto.FromEntity);

        return new PagedResult<OrderDto>
        {
            Items = orderDtos,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}

