using FluentAssertions;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Orders.Commands.CreateStripeCheckoutSession;
using Xunit;

namespace NaarNoor.Application.Tests.Orders;

public class StripeInterfaceRecordTests
{
    // ──────────────────────────────────────────────────────────
    // StripeLineItem
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void StripeLineItem_CanBeConstructed_WithAllFields()
    {
        var item = new StripeLineItem("Hawawshi", "Egyptian dumplings", 8.95m, 2);

        item.Name.Should().Be("Hawawshi");
        item.Description.Should().Be("Egyptian dumplings");
        item.UnitPrice.Should().Be(8.95m);
        item.Quantity.Should().Be(2);
    }

    [Fact]
    public void StripeLineItem_Description_CanBeNull()
    {
        var item = new StripeLineItem("Koshari", null, 14.95m, 1);

        item.Description.Should().BeNull();
        item.Name.Should().Be("Koshari");
        item.UnitPrice.Should().Be(14.95m);
        item.Quantity.Should().Be(1);
    }

    [Fact]
    public void StripeLineItem_TwoIdenticalInstances_AreEqual()
    {
        var item1 = new StripeLineItem("Lamb", "Rogan Josh", 18.95m, 1);
        var item2 = new StripeLineItem("Lamb", "Rogan Josh", 18.95m, 1);

        item1.Should().Be(item2);
    }

    [Fact]
    public void StripeLineItem_DifferentQuantities_AreNotEqual()
    {
        var item1 = new StripeLineItem("Hawawshi", null, 8.95m, 1);
        var item2 = new StripeLineItem("Hawawshi", null, 8.95m, 3);

        item1.Should().NotBe(item2);
    }

    [Fact]
    public void StripeLineItem_GetHashCode_IsConsistent()
    {
        var item = new StripeLineItem("Om Ali", "Rice pudding", 6.95m, 2);

        item.GetHashCode().Should().Be(item.GetHashCode());
    }

    [Fact]
    public void StripeLineItem_ToString_ContainsTypeName()
    {
        var item = new StripeLineItem("Sel Roti", null, 4.50m, 1);

        item.ToString().Should().Contain("StripeLineItem");
    }

    // ──────────────────────────────────────────────────────────
    // StripeCheckoutRequest
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void StripeCheckoutRequest_CanBeConstructed_WithAllFields()
    {
        var orderId = Guid.NewGuid();
        var lineItems = new List<StripeLineItem> { new("Hawawshi", null, 8.95m, 2) };

        var request = new StripeCheckoutRequest(
            orderId,
            "customer@example.com",
            "John Doe",
            lineItems,
            "https://example.com/success",
            "https://example.com/cancel"
        );

        request.OrderId.Should().Be(orderId);
        request.CustomerEmail.Should().Be("customer@example.com");
        request.CustomerName.Should().Be("John Doe");
        request.LineItems.Should().HaveCount(1);
        request.SuccessUrl.Should().Be("https://example.com/success");
        request.CancelUrl.Should().Be("https://example.com/cancel");
    }

    [Fact]
    public void StripeCheckoutRequest_TwoIdenticalInstances_AreEqual()
    {
        var orderId = Guid.NewGuid();
        var lineItems = new List<StripeLineItem> { new("Item", null, 10m, 1) };

        var r1 = new StripeCheckoutRequest(orderId, "a@b.com", "Alice", lineItems, "https://s.com", "https://c.com");
        var r2 = new StripeCheckoutRequest(orderId, "a@b.com", "Alice", lineItems, "https://s.com", "https://c.com");

        r1.Should().Be(r2);
    }

    [Fact]
    public void StripeCheckoutRequest_DifferentEmails_AreNotEqual()
    {
        var orderId = Guid.NewGuid();
        var lineItems = new List<StripeLineItem>();

        var r1 = new StripeCheckoutRequest(orderId, "a@b.com", "Alice", lineItems, "https://s.com", "https://c.com");
        var r2 = new StripeCheckoutRequest(orderId, "x@y.com", "Alice", lineItems, "https://s.com", "https://c.com");

        r1.Should().NotBe(r2);
    }

    [Fact]
    public void StripeCheckoutRequest_GetHashCode_IsConsistent()
    {
        var req = new StripeCheckoutRequest(Guid.NewGuid(), "e@mail.com", "Name",
            new List<StripeLineItem>(), "https://s.com", "https://c.com");

        req.GetHashCode().Should().Be(req.GetHashCode());
    }

    [Fact]
    public void StripeCheckoutRequest_ToString_ContainsTypeName()
    {
        var req = new StripeCheckoutRequest(Guid.NewGuid(), "e@mail.com", "Name",
            new List<StripeLineItem>(), "https://s.com", "https://c.com");

        req.ToString().Should().Contain("StripeCheckoutRequest");
    }

    // ──────────────────────────────────────────────────────────
    // StripeWebhookEvent
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void StripeWebhookEvent_CanBeConstructed_WithAllFields()
    {
        var evt = new StripeWebhookEvent(
            "checkout.session.completed",
            "cs_test_abc",
            Guid.NewGuid().ToString(),
            "pi_test_xyz"
        );

        evt.EventType.Should().Be("checkout.session.completed");
        evt.SessionId.Should().Be("cs_test_abc");
        evt.OrderId.Should().NotBeNull();
        evt.PaymentIntentId.Should().Be("pi_test_xyz");
    }

    [Fact]
    public void StripeWebhookEvent_NullableFields_CanBeNull()
    {
        var evt = new StripeWebhookEvent("payment_intent.created", null, null, null);

        evt.EventType.Should().Be("payment_intent.created");
        evt.SessionId.Should().BeNull();
        evt.OrderId.Should().BeNull();
        evt.PaymentIntentId.Should().BeNull();
    }

    [Fact]
    public void StripeWebhookEvent_TwoIdenticalInstances_AreEqual()
    {
        var orderId = Guid.NewGuid().ToString();
        var e1 = new StripeWebhookEvent("checkout.session.completed", "cs_abc", orderId, "pi_xyz");
        var e2 = new StripeWebhookEvent("checkout.session.completed", "cs_abc", orderId, "pi_xyz");

        e1.Should().Be(e2);
    }

    [Fact]
    public void StripeWebhookEvent_DifferentEventTypes_AreNotEqual()
    {
        var e1 = new StripeWebhookEvent("checkout.session.completed", null, null, null);
        var e2 = new StripeWebhookEvent("checkout.session.expired", null, null, null);

        e1.Should().NotBe(e2);
    }

    [Fact]
    public void StripeWebhookEvent_GetHashCode_IsConsistent()
    {
        var evt = new StripeWebhookEvent("payment_intent.succeeded", "cs_1", "ord_1", "pi_1");

        evt.GetHashCode().Should().Be(evt.GetHashCode());
    }

    [Fact]
    public void StripeWebhookEvent_ToString_ContainsTypeName()
    {
        var evt = new StripeWebhookEvent("event.type", null, null, null);

        evt.ToString().Should().Contain("StripeWebhookEvent");
    }

    // ──────────────────────────────────────────────────────────
    // CheckoutOrderItemRequest
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void CheckoutOrderItemRequest_CanBeConstructed_WithAllFields()
    {
        var menuItemId = Guid.NewGuid();

        var item = new CheckoutOrderItemRequest(menuItemId, "Koshari", 14.95m, 2);

        item.MenuItemId.Should().Be(menuItemId);
        item.MenuItemName.Should().Be("Koshari");
        item.UnitPrice.Should().Be(14.95m);
        item.Quantity.Should().Be(2);
    }

    [Fact]
    public void CheckoutOrderItemRequest_TwoIdenticalInstances_AreEqual()
    {
        var id = Guid.NewGuid();
        var r1 = new CheckoutOrderItemRequest(id, "Hawawshi", 8.95m, 3);
        var r2 = new CheckoutOrderItemRequest(id, "Hawawshi", 8.95m, 3);

        r1.Should().Be(r2);
    }

    [Fact]
    public void CheckoutOrderItemRequest_DifferentPrices_AreNotEqual()
    {
        var id = Guid.NewGuid();
        var r1 = new CheckoutOrderItemRequest(id, "Om Ali", 6.95m, 1);
        var r2 = new CheckoutOrderItemRequest(id, "Om Ali", 7.95m, 1);

        r1.Should().NotBe(r2);
    }

    [Fact]
    public void CheckoutOrderItemRequest_GetHashCode_IsConsistent()
    {
        var item = new CheckoutOrderItemRequest(Guid.NewGuid(), "Sel Roti", 4.50m, 1);

        item.GetHashCode().Should().Be(item.GetHashCode());
    }

    [Fact]
    public void CheckoutOrderItemRequest_ToString_ContainsTypeName()
    {
        var item = new CheckoutOrderItemRequest(Guid.NewGuid(), "Item", 9.99m, 1);

        item.ToString().Should().Contain("CheckoutOrderItemRequest");
    }

    // ──────────────────────────────────────────────────────────
    // StripeCheckoutResult
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void StripeCheckoutResult_CanBeConstructed()
    {
        var result = new StripeCheckoutResult("cs_test_session_123", "https://checkout.stripe.com/pay/cs_test_session_123");

        result.SessionId.Should().Be("cs_test_session_123");
        result.SessionUrl.Should().Be("https://checkout.stripe.com/pay/cs_test_session_123");
    }

    [Fact]
    public void StripeCheckoutResult_TwoIdenticalInstances_AreEqual()
    {
        var r1 = new StripeCheckoutResult("cs_abc", "https://stripe.com/pay");
        var r2 = new StripeCheckoutResult("cs_abc", "https://stripe.com/pay");

        r1.Should().Be(r2);
    }

    [Fact]
    public void StripeCheckoutResult_ToString_ContainsTypeName()
    {
        var result = new StripeCheckoutResult("cs_abc", "https://stripe.com/pay");

        result.ToString().Should().Contain("StripeCheckoutResult");
    }

    // ──────────────────────────────────────────────────────────
    // CreateStripeCheckoutSessionResponse
    // ──────────────────────────────────────────────────────────

    [Fact]
    public void CreateStripeCheckoutSessionResponse_CanBeConstructed()
    {
        var orderId = Guid.NewGuid();
        var response = new CreateStripeCheckoutSessionResponse(orderId, "https://checkout.stripe.com/pay/session");

        response.OrderId.Should().Be(orderId);
        response.SessionUrl.Should().Be("https://checkout.stripe.com/pay/session");
    }

    [Fact]
    public void CreateStripeCheckoutSessionResponse_TwoIdenticalInstances_AreEqual()
    {
        var orderId = Guid.NewGuid();
        var r1 = new CreateStripeCheckoutSessionResponse(orderId, "https://stripe.com");
        var r2 = new CreateStripeCheckoutSessionResponse(orderId, "https://stripe.com");

        r1.Should().Be(r2);
    }
}
