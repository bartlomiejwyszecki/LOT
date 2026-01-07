using Orders.Application.DTOs;
using Orders.Application.Interfaces;

namespace Orders.Application.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        
        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {command.OrderId} not found.");
        }

        order.UpdateStatus(command.NewStatus);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return OrderDto.FromEntity(order);
    }
}

