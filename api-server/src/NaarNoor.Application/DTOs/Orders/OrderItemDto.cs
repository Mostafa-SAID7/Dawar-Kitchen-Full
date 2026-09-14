namespace NaarNoor.Application.DTOs.Orders;

/// <summary>
/// Data Transfer Object for OrderItem - individual item in an order
/// ✅ Moved from root DTOs folder to feature-organized structure
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Unique order item identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Menu item ID
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Menu item name (snapshot at order time)
    /// </summary>
    public string MenuItemName { get; set; } = string.Empty;

    /// <summary>
    /// Unit price at time of order (server-validated)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Line total (UnitPrice * Quantity)
    /// </summary>
    public decimal LineTotal { get; set; }
}
