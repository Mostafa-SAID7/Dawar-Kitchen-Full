using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        return CreatedAtAction(nameof(CreateOrder), new CreateOrderResponse { Id = orderId.ToString() });
    }
}

/// <summary>
/// Create order request from frontend checkout
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Customer's full name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Order type: collection, delivery, or dine-in
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Delivery address (required when type=delivery)
    /// </summary>
    public string? DeliveryAddress { get; set; }

    /// <summary>
    /// Optional notes or special requests
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Optional table reservation name for dine-in orders
    /// </summary>
    public string? TableReservationName { get; set; }

    /// <summary>
    /// Items to order
    /// </summary>
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Order item in create order request
/// Note: UnitPrice is provided by frontend for display but will be recomputed server-side
/// </summary>
public class CreateOrderItemRequest
{
    /// <summary>
    /// Menu item ID
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Menu item name
    /// </summary>
    public string MenuItemName { get; set; } = string.Empty;

    /// <summary>
    /// Unit price provided by frontend (will be ignored, server will recompute)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantity to order
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Create order response
/// </summary>
public class CreateOrderResponse
{
    /// <summary>
    /// Order ID as string (GUID)
    /// </summary>
    public string Id { get; set; } = string.Empty;
}

