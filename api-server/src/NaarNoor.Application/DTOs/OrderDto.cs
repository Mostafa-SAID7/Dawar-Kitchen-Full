namespace NaarNoor.Application.DTOs;

/// <summary>
/// Data Transfer Object for Order - represents a customer order
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Unique order identifier
    /// </summary>
    public Guid Id { get; set; }

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
    /// Total amount in USD (computed server-side)
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Order status (Pending, Confirmed, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Order items (menu items + quantities)
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Timestamp when order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
