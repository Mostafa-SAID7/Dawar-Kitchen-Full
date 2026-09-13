using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Handler for CreateOrderCommand
/// Validates menu items, recomputes prices server-side, creates Order + OrderItems in a SINGLE transaction
/// ✅ Fixed: Consolidated to single SaveChangesAsync (atomic operation)
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // ✅ Step 1: Validate all menu items exist and fetch current prices from database
        var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();
        
        // Fetch all available items and filter in-memory (no IQueryable in Application)
        var allAvailableItems = await _unitOfWork.MenuItems.GetAllAsync(cancellationToken);
        var validatedMenuItems = allAvailableItems
            .Where(m => menuItemIds.Contains(m.Id) && m.IsAvailable)
            .ToList();

        if (validatedMenuItems.Count != request.Items.Count)
        {
            throw new InvalidOperationException("One or more menu items are not available or do not exist.");
        }

        // ✅ Step 2: Recompute prices server-side (do not trust client prices)
        var validatedItems = new List<(OrderItemRequest Request, MenuItem MenuItem, decimal ServerPrice)>();
        decimal orderTotal = 0;

        foreach (var requestItem in request.Items)
        {
            var menuItem = validatedMenuItems.FirstOrDefault(m => m.Id == requestItem.MenuItemId)
                ?? throw new InvalidOperationException($"Menu item {requestItem.MenuItemId} not found.");

            // Server-side price (ignore client-provided price)
            var serverPrice = menuItem.Price;
            var lineTotal = serverPrice * requestItem.Quantity;
            orderTotal += lineTotal;

            validatedItems.Add((requestItem, menuItem, serverPrice));
        }

        // ✅ Step 3: Create Order entity
        var order = new Order
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Type = Enum.Parse<OrderType>(request.Type, ignoreCase: true),
            DeliveryAddress = request.DeliveryAddress,
            Notes = request.Notes,
            TableReservationName = request.TableReservationName,
            TotalAmount = orderTotal,
            Status = OrderStatus.Pending
        };

        _unitOfWork.Orders.Add(order);

        // ✅ Step 4: Create OrderItems (one per menu item) — BEFORE SaveChangesAsync
        foreach (var (requestItem, menuItem, serverPrice) in validatedItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                MenuItemId = menuItem.Id,
                MenuItemName = menuItem.Name,
                UnitPrice = serverPrice,
                Quantity = requestItem.Quantity
            };

            _unitOfWork.OrderItems.Add(orderItem);
        }

        // ✅ Step 5: SINGLE SaveChangesAsync — atomic transaction (FIXED from two separate calls)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
