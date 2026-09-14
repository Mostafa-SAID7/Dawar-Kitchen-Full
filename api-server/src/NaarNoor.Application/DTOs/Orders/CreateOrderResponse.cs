namespace NaarNoor.Application.DTOs.Orders;

/// <summary>
/// Response DTO for create order operation.
/// ✅ Moved from API layer (OrdersController.cs) to Application layer
/// </summary>
public class CreateOrderResponse
{
    /// <summary>
    /// Order ID as string (GUID)
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
