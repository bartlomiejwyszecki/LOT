using Orders.Application.DTOs;
using Orders.Application.Interfaces;

namespace Orders.Application.Queries.GetOrderByOrderNumber;

public class GetOrderByOrderNumberQueryHandler : IQueryHandler<GetOrderByOrderNumberQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByOrderNumberQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> HandleAsync(GetOrderByOrderNumberQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(query.OrderNumber, cancellationToken);
        
        return order != null ? OrderDto.FromEntity(order) : null;
    }
}

