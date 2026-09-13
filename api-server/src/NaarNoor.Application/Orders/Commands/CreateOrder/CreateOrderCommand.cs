using MediatR;

namespace NaarNoor.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Command to create a new order from checkout
/// </summary>
public record CreateOrderCommand(
    string CustomerName,
    string Email,
    string PhoneNumber,
    string Type,
    string? DeliveryAddress,
    string? Notes,
    string? TableReservationName,
    List<OrderItemRequest> Items
) : IRequest<Guid>;

/// <summary>
/// Order item request from frontend
/// Note: UnitPrice is provided by frontend but will be recomputed server-side for security
/// </summary>
public record OrderItemRequest(
    Guid MenuItemId,
    string MenuItemName,
    decimal UnitPrice,
    int Quantity
);
