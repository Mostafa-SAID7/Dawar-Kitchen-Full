using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.Orders;

/// <summary>
/// Request DTO for order items in CreateOrderRequest.
/// ✅ Moved from API layer (OrdersController.cs) to Application layer
/// </summary>
public class CreateOrderItemRequest
{
    [Required(ErrorMessage = "MenuItemId is required")]
    public Guid MenuItemId { get; set; }

    [Required(ErrorMessage = "MenuItemName is required")]
    [MinLength(2, ErrorMessage = "MenuItemName must be at least 2 characters")]
    public string MenuItemName { get; set; } = "";

    [Required(ErrorMessage = "UnitPrice is required")]
    [Range(0.01, 10000, ErrorMessage = "UnitPrice must be between 0.01 and 10000")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
    public int Quantity { get; set; }
}
