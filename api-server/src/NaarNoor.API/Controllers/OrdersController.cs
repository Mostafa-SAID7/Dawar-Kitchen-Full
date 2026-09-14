using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.DTOs.Orders;
using NaarNoor.Application.Features.Orders.Commands.CreateOrder;

namespace NaarNoor.API.Controllers;

/// <summary>
/// Endpoints for managing customer orders
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new order from checkout
    /// </summary>
    /// <remarks>
    /// Accepts online order with items, validates menu items, recomputes total server-side.
    /// Returns 201 Created with order ID.
    /// </remarks>
    /// <param name="request">Order creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>201 Created with order ID</returns>
    /// <response code="201">Order created successfully, returns { id: string }</response>
    /// <response code="400">Validation failed (invalid input)</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        // Map request to command
        var command = new CreateOrderCommand(
            CustomerName: request.CustomerName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber,
            Type: request.Type,
            DeliveryAddress: request.DeliveryAddress,
            Notes: request.Notes,
            TableReservationName: request.TableReservationName,
            Items: request.Items
                .Select(i => new OrderItemRequest(
                    MenuItemId: i.MenuItemId,
                    MenuItemName: i.MenuItemName,
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity))
                .ToList()
        );

        // Dispatch command via MediatR
        var orderId = await _mediator.Send(command, cancellationToken);

        // Return 201 Created with order ID
        return CreatedAtAction(nameof(CreateOrder), new { Id = orderId.ToString() });
    }
}

