using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.Orders;

/// <summary>
/// Request DTO for creating an order.
/// ✅ Moved from API layer (OrdersController.cs) to Application layer
/// </summary>
public class CreateOrderRequest
{
    [Required(ErrorMessage = "CustomerName is required")]
    [MinLength(2, ErrorMessage = "CustomerName must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "CustomerName must not exceed 100 characters")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PhoneNumber is required")]
    [Phone(ErrorMessage = "PhoneNumber must be a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; } = "Delivery";

    [MaxLength(250, ErrorMessage = "DeliveryAddress must not exceed 250 characters")]
    public string? DeliveryAddress { get; set; }

    [MaxLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }

    public string? TableReservationName { get; set; }

    [Required(ErrorMessage = "Items are required")]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
