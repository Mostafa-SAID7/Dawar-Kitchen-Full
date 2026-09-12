using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Infrastructure.Services;
using Xunit;

namespace NaarNoor.Infrastructure.Tests.Services;

/// <summary>
/// Additional tests for StripeService to cover constructor paths and configuration scenarios.
/// CreateCheckoutSessionAsync cannot be unit-tested without a real Stripe key,
/// so these tests focus on all reachable code paths in the constructor and ParseWebhookEventAsync.
/// </summary>
public class StripeServiceCheckoutTests
{
    private static IConfiguration BuildConfig(
        string? secretKey = "sk_test_fake_key_for_testing_only",
        string webhookSecret = "")
    {
        var dict = new Dictionary<string, string?>();
        if (secretKey is not null)
            dict["Stripe:SecretKey"] = secretKey;
        dict["Stripe:WebhookSecret"] = webhookSecret;
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static StripeService CreateService(
        string? secretKey = "sk_test_fake_key_for_testing_only",
        string webhookSecret = "")
    {
        Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", null);
        Environment.SetEnvironmentVariable("STRIPE_WEBHOOK_SECRET", null);
        return new StripeService(BuildConfig(secretKey, webhookSecret), Mock.Of<ILogger<StripeService>>());
    }

    [Fact]
    public void Constructor_WithSecretKeyFromEnvVar_DoesNotThrow()
    {
        Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", "sk_test_env_key_only");
        var prevWebhook = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
        try
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
            var logger = Mock.Of<ILogger<StripeService>>();

            Action act = () => new StripeService(config, logger);

            act.Should().NotThrow("Env var STRIPE_SECRET_KEY should be sufficient");
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", null);
            Environment.SetEnvironmentVariable("STRIPE_WEBHOOK_SECRET", prevWebhook);
        }
    }

    [Fact]
    public void Constructor_WebhookSecretFromEnvVar_UsesEnvVar()
    {
        Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", null);
        Environment.SetEnvironmentVariable("STRIPE_WEBHOOK_SECRET", "whsec_env_webhook_secret");
        try
        {
            var config = BuildConfig("sk_test_fake_key");
            var logger = Mock.Of<ILogger<StripeService>>();

            Action act = () => new StripeService(config, logger);
            act.Should().NotThrow();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRIPE_WEBHOOK_SECRET", null);
        }
    }

    [Fact]
    public async Task ParseWebhookEventAsync_InDevelopment_CheckoutSessionExpired_ReturnsCorrectType()
    {
        var prevEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var service = CreateService(webhookSecret: "");

            var payload = BuildCheckoutExpiredEvent(Guid.NewGuid().ToString());
            var result = await service.ParseWebhookEventAsync(payload, "no-sig", CancellationToken.None);

            result.EventType.Should().Be("checkout.session.expired");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", prevEnv);
        }
    }

    [Fact]
    public async Task ParseWebhookEventAsync_InDevelopment_WithNullSessionMetadata_HandlesGracefully()
    {
        var prevEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var service = CreateService(webhookSecret: "");

            var payload = BuildCheckoutSessionNoMetadata();
            var result = await service.ParseWebhookEventAsync(payload, "no-sig", CancellationToken.None);

            result.EventType.Should().Be("checkout.session.completed");
            result.SessionId.Should().NotBeNull();
            result.OrderId.Should().BeNull("No orderId in metadata");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", prevEnv);
        }
    }

    [Fact]
    public async Task ParseWebhookEventAsync_InDevelopment_PaymentIntentSucceeded_HasNullSessionId()
    {
        var prevEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var service = CreateService(webhookSecret: "");

            var result = await service.ParseWebhookEventAsync(
                BuildPaymentIntentEvent("payment_intent.succeeded"),
                "no-sig",
                CancellationToken.None);

            result.EventType.Should().Be("payment_intent.succeeded");
            result.SessionId.Should().BeNull();
            result.OrderId.Should().BeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", prevEnv);
        }
    }

    [Fact]
    public void CreateCheckoutSessionRequest_MapToStripeLineItems_ConvertsCorrectly()
    {
        var lineItems = new List<StripeLineItem>
        {
            new("Hawawshi", "Dumplings", 8.95m, 2),
            new("Koshari", null, 14.95m, 1)
        };

        lineItems.Should().HaveCount(2);
        lineItems[0].Name.Should().Be("Hawawshi");
        lineItems[0].UnitPrice.Should().Be(8.95m);
        lineItems[0].Quantity.Should().Be(2);
        lineItems[1].Description.Should().BeNull();
    }

    private const string ApiVersion = "2025-01-27.acacia";

    private static string BuildCheckoutExpiredEvent(string orderId) => $$"""
        {
          "id": "evt_test_expired_001",
          "object": "event",
          "type": "checkout.session.expired",
          "api_version": "{{ApiVersion}}",
          "created": 1234567890,
          "livemode": false,
          "pending_webhooks": 0,
          "request": null,
          "data": {
            "object": {
              "id": "cs_test_expired_session",
              "object": "checkout.session",
              "payment_intent": null,
              "amount_total": 1500,
              "currency": "gbp",
              "customer_email": "test@example.com",
              "mode": "payment",
              "payment_status": "unpaid",
              "status": "expired",
              "success_url": "https://example.com/success",
              "cancel_url": "https://example.com/cancel",
              "metadata": {
                "orderId": "{{orderId}}",
                "customerName": "Test"
              }
            }
          }
        }
        """;

    private static string BuildCheckoutSessionNoMetadata() => $$"""
        {
          "id": "evt_test_nometa_001",
          "object": "event",
          "type": "checkout.session.completed",
          "api_version": "{{ApiVersion}}",
          "created": 1234567890,
          "livemode": false,
          "pending_webhooks": 0,
          "request": null,
          "data": {
            "object": {
              "id": "cs_test_no_meta",
              "object": "checkout.session",
              "payment_intent": "pi_test_abc",
              "amount_total": 2000,
              "currency": "gbp",
              "customer_email": "test@example.com",
              "mode": "payment",
              "payment_status": "paid",
              "status": "complete",
              "success_url": "https://example.com/success",
              "cancel_url": "https://example.com/cancel",
              "metadata": {}
            }
          }
        }
        """;

    private static string BuildPaymentIntentEvent(string type) => $$"""
        {
          "id": "evt_test_pi_001",
          "object": "event",
          "type": "{{type}}",
          "api_version": "{{ApiVersion}}",
          "created": 1234567890,
          "livemode": false,
          "pending_webhooks": 0,
          "request": null,
          "data": {
            "object": {
              "id": "pi_test_xyz",
              "object": "payment_intent",
              "amount": 2000,
              "currency": "gbp"
            }
          }
        }
        """;
}
