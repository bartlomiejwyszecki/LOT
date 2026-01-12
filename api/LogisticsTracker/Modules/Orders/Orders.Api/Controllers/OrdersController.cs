using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.Commands.UpdateOrderStatus;
using Orders.Application.DTOs;
using Orders.Application.Queries;
using Orders.Application.Queries.GetAllOrders;
using Orders.Application.Queries.GetOrderById;
using Orders.Application.Queries.GetOrderByOrderNumber;
using Orders.Domain.ValueObjects;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICommandHandler<CreateOrderCommand, OrderDto> _createOrderHandler;
    private readonly ICommandHandler<UpdateOrderStatusCommand, OrderDto> _updateOrderStatusHandler;
    private readonly IQueryHandler<GetAllOrdersQuery, PagedResult<OrderDto>> _getAllOrdersHandler;
    private readonly IQueryHandler<GetOrderByIdQuery, OrderDto?> _getOrderByIdHandler;
    private readonly IQueryHandler<GetOrderByOrderNumberQuery, OrderDto?> _getOrderByOrderNumberHandler;

    public OrdersController(
        ICommandHandler<CreateOrderCommand, OrderDto> createOrderHandler,
        ICommandHandler<UpdateOrderStatusCommand, OrderDto> updateOrderStatusHandler,
        IQueryHandler<GetAllOrdersQuery, PagedResult<OrderDto>> getAllOrdersHandler,
        IQueryHandler<GetOrderByIdQuery, OrderDto?> getOrderByIdHandler,
        IQueryHandler<GetOrderByOrderNumberQuery, OrderDto?> getOrderByOrderNumberHandler)
    {
        _createOrderHandler = createOrderHandler;
        _updateOrderStatusHandler = updateOrderStatusHandler;
        _getAllOrdersHandler = getAllOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _getOrderByOrderNumberHandler = getOrderByOrderNumberHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            dto.OrderNumber,
            dto.Street,
            dto.City,
            dto.State,
            dto.PostalCode,
            dto.Country);

        var result = await _createOrderHandler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = result.Id },
            result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllOrdersQuery(pageNumber, pageSize, status, fromDate, toDate);
        var result = await _getAllOrdersHandler.HandleAsync(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _getOrderByIdHandler.HandleAsync(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("by-order-number/{orderNumber}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderByOrderNumber(
        [FromRoute] string orderNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByOrderNumberQuery(orderNumber);
        var result = await _getOrderByOrderNumberHandler.HandleAsync(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(id, dto.NewStatus);
        var result = await _updateOrderStatusHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }
}
