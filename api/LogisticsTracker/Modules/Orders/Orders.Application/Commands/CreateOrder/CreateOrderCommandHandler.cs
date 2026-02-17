using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var existingOrder = await _orderRepository.GetByOrderNumberAsync(command.OrderNumber, cancellationToken);
        
        if (existingOrder is not null)
            throw new InvalidOperationException($"Order with number '{command.OrderNumber}' already exists.");

        var address = new Address(
            command.Street,
            command.City,
            command.State,
            command.PostalCode,
            command.Country
        );

        var order = Order.Create(command.OrderNumber, address);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return OrderDto.FromEntity(order);
    }
}

