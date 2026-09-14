using FluentAssertions;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;
using NaarNoor.Domain.Exceptions;
using Xunit;

namespace NaarNoor.Domain.Tests.Aggregates;

/// <summary>
/// Domain unit tests for Order aggregate.
/// ✅ Tests domain-driven factory method, invariants, and state transitions
/// </summary>
public class OrderAggregateTests
{
    #region Order.Create Factory Tests

    [Fact]
    public void Create_WithValidInputs_ReturnsOrderWithCorrectProperties()
    {
        // Arrange
        var customerName = "John Doe";
        var email = "john@example.com";
        var phoneNumber = "555-1234";
        var type = OrderType.Delivery;
        var deliveryAddress = "123 Main St";
        var notes = "No onions";

        // Act
        var order = Order.Create(customerName, email, phoneNumber, type, deliveryAddress, null, notes);

        // Assert
        order.CustomerName.Should().Be(customerName);
        order.Email.Should().Be(email.ToLowerInvariant());
        order.PhoneNumber.Should().Be(phoneNumber);
        order.Type.Should().Be(type);
        order.DeliveryAddress.Should().Be(deliveryAddress);
        order.Notes.Should().Be(notes);
        order.Id.Should().NotBe(Guid.Empty);
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(0);
        order.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyCustomerName_ThrowsOrderDomainException(string customerName)
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Order.Create(customerName, "test@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ThrowsOrderDomainException(string email)
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Order.Create("John", email, "555-1234", OrderType.Delivery, "123 Main St", null, null));
    }

    [Fact]
    public void Create_WithDeliveryTypeButNoAddress_ThrowsOrderDomainException()
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Order.Create("John", "test@example.com", "555-1234", OrderType.Delivery, null, null, null));
    }

    [Fact]
    public void Create_WithDineInTypeButNoTableName_ThrowsOrderDomainException()
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Order.Create("John", "test@example.com", "555-1234", OrderType.DineIn, null, null, null));
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public void AddItem_WithValidItem_AddsItemToOrder()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem 
        { 
            MenuItemId = Guid.NewGuid(),
            MenuItemName = "Koshari",
            Quantity = 2,
            UnitPrice = 10.50m
        };

        // Act
        order.AddItem(item);

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items[0].MenuItemName.Should().Be("Koshari");
        order.Items[0].Quantity.Should().Be(2);
        order.Items[0].UnitPrice.Should().Be(10.50m);
        order.TotalAmount.Should().Be(21m);
    }

    [Fact]
    public void AddItem_MultipleTimes_UpdatesOrderTotal()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);

        // Act
        var item1 = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item1", Quantity = 1, UnitPrice = 10m };
        var item2 = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item2", Quantity = 2, UnitPrice = 15m };
        order.AddItem(item1);
        order.AddItem(item2);

        // Assert
        order.Items.Should().HaveCount(2);
        order.TotalAmount.Should().Be(10m + (2 * 15m));
    }

    [Fact]
    public void AddItem_WithZeroQuantity_ThrowsOrderDomainException()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item", Quantity = 0, UnitPrice = 10m };

        // Act & Assert
        Assert.Throws<OrderDomainException>(() => order.AddItem(item));
    }

    [Fact]
    public void AddItem_WithNegativePrice_ThrowsOrderDomainException()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item", Quantity = 1, UnitPrice = -10m };

        // Act & Assert
        Assert.Throws<OrderDomainException>(() => order.AddItem(item));
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public void RemoveItem_WithValidIndex_RemovesItemFromOrder()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item1 = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item1", Quantity = 1, UnitPrice = 10m };
        var item2 = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item2", Quantity = 1, UnitPrice = 15m };
        order.AddItem(item1);
        order.AddItem(item2);

        // Act
        order.RemoveItem(0); // Remove first item

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items[0].MenuItemName.Should().Be("Item2");
        order.TotalAmount.Should().Be(15m);
    }

    [Fact]
    public void RemoveItem_WithInvalidIndex_ThrowsOrderDomainException()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item1", Quantity = 1, UnitPrice = 10m };
        order.AddItem(item);

        // Act & Assert
        Assert.Throws<OrderDomainException>(() => order.RemoveItem(5)); // Out of range
    }

    [Fact]
    public void RemoveItem_RemovingLastItem_ClearsOrderTotal()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item1", Quantity = 1, UnitPrice = 10m };
        order.AddItem(item);

        // Act
        order.RemoveItem(0);

        // Assert
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0m);
    }

    #endregion

    #region State Machine Tests

    [Fact]
    public void NewOrder_HasPendingStatus()
    {
        // Arrange & Act
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Confirm_TransitionsFromPendingToConfirmed()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = "Item", Quantity = 1, UnitPrice = 10m };
        order.AddItem(item);

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_WithNoItems_ThrowsOrderDomainException()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);

        // Act & Assert
        Assert.Throws<OrderDomainException>(() => order.Confirm());
    }

    [Fact]
    public void OrderWithItems_CalculatesTotalCorrectly()
    {
        // Arrange
        var order = Order.Create("John", "john@example.com", "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var expectedTotal = 0m;

        // Act & Assert - add items and verify running total
        for (int i = 0; i < 3; i++)
        {
            var price = 10m + i;
            var item = new OrderItem { MenuItemId = Guid.NewGuid(), MenuItemName = $"Item{i}", Quantity = 1, UnitPrice = price };
            order.AddItem(item);
            expectedTotal += price;
            order.TotalAmount.Should().Be(expectedTotal);
        }
    }

    #endregion

    #region Invariant Tests

    [Fact]
    public void Order_MaintainsIdempotency()
    {
        // Arrange
        var customerName = "John Doe";
        var email = "john@example.com";

        // Act
        var order1 = Order.Create(customerName, email, "555-1234", OrderType.Delivery, "123 Main St", null, null);
        var order2 = Order.Create(customerName, email, "555-1234", OrderType.Delivery, "123 Main St", null, null);

        // Assert - different instances, different IDs
        order1.Id.Should().NotBe(order2.Id);
        order1.CustomerName.Should().Be(order2.CustomerName);
    }

    #endregion
}
