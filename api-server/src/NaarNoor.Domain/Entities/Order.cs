using NaarNoor.Domain.Common;
using NaarNoor.Domain.Enums;
using NaarNoor.Domain.Exceptions;

namespace NaarNoor.Domain.Entities;

/// <summary>
/// Order aggregate root representing a customer order.
/// This is a true aggregate root with business logic and state management.
/// 
/// Key responsibilities:
/// - Enforce order business rules (validate items, manage status transitions)
/// - Manage order items collection (add/remove items)
/// - Calculate totals and maintain consistency
/// - Support state machine transitions (Pending → Confirmed → Preparing → Ready → Completed)
/// </summary>
public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public OrderType Type { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? TableReservationName { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? StripeSessionId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// Factory method for creating a new Order.
    /// Validates all required fields and initializes the order in Pending status.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when order data is invalid.</exception>
    public static Order Create(
        string customerName,
        string email,
        string phoneNumber,
        OrderType type,
        string? deliveryAddress = null,
        string? tableReservationName = null,
        string? notes = null)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(customerName))
            throw new OrderDomainException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new OrderDomainException("Customer email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new OrderDomainException("Customer phone number is required.");

        // Validate delivery address for delivery orders
        if (type == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
            throw new OrderDomainException("Delivery address is required for delivery orders.");

        // Validate table reservation name for dine-in orders
        if (type == OrderType.DineIn && string.IsNullOrWhiteSpace(tableReservationName))
            throw new OrderDomainException("Table reservation name is required for dine-in orders.");

        return new Order
        {
            CustomerName = customerName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber.Trim(),
            Type = type,
            DeliveryAddress = deliveryAddress?.Trim(),
            TableReservationName = tableReservationName?.Trim(),
            Notes = notes?.Trim(),
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            TotalAmount = 0,
            Items = new()
        };
    }

    /// <summary>
    /// Adds an item to the order.
    /// Validates item data and updates order total.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when item data is invalid.</exception>
    public void AddItem(OrderItem item)
    {
        if (item is null)
            throw new OrderDomainException("Order item cannot be null.");

        if (item.Quantity <= 0)
            throw new OrderDomainException("Order item quantity must be greater than zero.");

        if (item.UnitPrice < 0)
            throw new OrderDomainException("Order item unit price cannot be negative.");

        if (string.IsNullOrWhiteSpace(item.MenuItemName))
            throw new OrderDomainException("Order item menu item name is required.");

        Items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Removes an item from the order by its index.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when index is out of range.</exception>
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Items.Count)
            throw new OrderDomainException("Item index is out of range.");

        Items.RemoveAt(index);
        RecalculateTotal();
    }

    /// <summary>
    /// Recalculates the total order amount based on all items.
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(item => item.UnitPrice * item.Quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirms the order (transitions from Pending to Confirmed).
    /// Can only be called when order is in Pending status and has at least one item.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when status transition is invalid or order has no items.</exception>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException($"Cannot confirm order in {Status} status. Order must be in Pending status.");

        if (Items.Count == 0)
            throw new OrderDomainException("Cannot confirm order with no items.");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as being prepared (transitions to Preparing status).
    /// Can only be called when order is in Confirmed status.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when status transition is invalid.</exception>
    public void MarkPreparing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new OrderDomainException($"Cannot mark order as preparing in {Status} status. Order must be in Confirmed status.");

        Status = OrderStatus.Preparing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as ready (transitions to Ready status).
    /// Can only be called when order is in Preparing status.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when status transition is invalid.</exception>
    public void MarkReady()
    {
        if (Status != OrderStatus.Preparing)
            throw new OrderDomainException($"Cannot mark order as ready in {Status} status. Order must be in Preparing status.");

        Status = OrderStatus.Ready;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Completes the order (transitions to Completed status).
    /// Can only be called when order is in Ready status.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when status transition is invalid.</exception>
    public void Complete()
    {
        if (Status != OrderStatus.Ready)
            throw new OrderDomainException($"Cannot complete order in {Status} status. Order must be in Ready status.");

        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the order (can only cancel from Pending or Confirmed states).
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when order cannot be cancelled from current status.</exception>
    public void Cancel()
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new OrderDomainException($"Cannot cancel order in {Status} status. Only Pending or Confirmed orders can be cancelled.");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates payment status and sets Stripe session ID.
    /// </summary>
    public void SetStripeSessionId(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new OrderDomainException("Stripe session ID cannot be null or empty.");

        StripeSessionId = sessionId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks payment as completed (sets status to Paid).
    /// </summary>
    public void MarkPaymentComplete()
    {
        PaymentStatus = PaymentStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the number of items in the order.
    /// </summary>
    public int ItemCount => Items.Count;

    /// <summary>
    /// Indicates whether the order has any items.
    /// </summary>
    public bool HasItems => Items.Count > 0;

    /// <summary>
    /// Indicates whether the order is in a terminal state (Completed or Cancelled).
    /// </summary>
    public bool IsTerminal => Status == OrderStatus.Completed || Status == OrderStatus.Cancelled;
}
