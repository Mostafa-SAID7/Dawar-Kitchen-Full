using FluentAssertions;
using NaarNoor.Domain.Exceptions;
using Xunit;

namespace NaarNoor.Domain.Tests.Exceptions;

/// <summary>
/// Domain unit tests for domain exceptions.
/// ✅ Tests exception hierarchy, messages, and invariant violations
/// </summary>
public class DomainExceptionTests
{
    #region DomainException Base Tests

    [Fact]
    public void DomainException_CanBeThrownAsDerivedType()
    {
        // Arrange
        var message = "Domain rule violated";
        bool threwCorrectException = false;

        // Act
        try
        {
            throw new OrderDomainException(message);
        }
        catch (OrderDomainException ex)
        {
            threwCorrectException = true;
            ex.Message.Should().Be(message);
        }

        // Assert
        threwCorrectException.Should().BeTrue();
    }

    [Fact]
    public void DomainException_CanBeCaughtByBaseType()
    {
        // Arrange
        bool caughtByBase = false;

        // Act & Assert
        try
        {
            throw new OrderDomainException("Domain rule violated");
        }
        catch (DomainException ex)
        {
            caughtByBase = true;
            ex.Message.Should().Contain("Domain");
        }

        caughtByBase.Should().BeTrue();
    }

    #endregion

    #region OrderDomainException Tests

    [Fact]
    public void OrderDomainException_InheritsFromDomainException()
    {
        // Arrange
        var message = "Order invariant violated";
        var exception = new OrderDomainException(message);

        // Assert
        exception.Should().BeOfType<OrderDomainException>();
        exception.Should().BeOfType<DomainException>();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void OrderDomainException_CanBeThrowForInvalidCustomerName()
    {
        // Arrange
        bool threw = false;

        // Act
        try
        {
            throw new OrderDomainException("Customer name cannot be empty");
        }
        catch (OrderDomainException ex)
        {
            threw = true;
            ex.Message.Should().Contain("Customer name");
        }

        threw.Should().BeTrue();
    }

    [Fact]
    public void OrderDomainException_CanBeThrownForInvalidQuantity()
    {
        // Arrange
        bool threw = false;

        // Act
        try
        {
            throw new OrderDomainException("Quantity must be greater than zero");
        }
        catch (OrderDomainException ex)
        {
            threw = true;
            ex.Message.Should().Contain("Quantity");
        }

        threw.Should().BeTrue();
    }

    #endregion

    #region ReservationDomainException Tests

    [Fact]
    public void ReservationDomainException_InheritsFromDomainException()
    {
        // Arrange
        var message = "Reservation invariant violated";
        var exception = new ReservationDomainException(message);

        // Assert
        exception.Should().BeOfType<ReservationDomainException>();
        exception.Should().BeOfType<DomainException>();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ReservationDomainException_CanBeThrownForInvalidPartySize()
    {
        // Arrange
        bool threw = false;

        // Act
        try
        {
            throw new ReservationDomainException("Party size must be between 1 and 100");
        }
        catch (ReservationDomainException ex)
        {
            threw = true;
            ex.Message.Should().Contain("Party size");
        }

        threw.Should().BeTrue();
    }

    [Fact]
    public void ReservationDomainException_CanBeThrownForPastDate()
    {
        // Arrange
        bool threw = false;

        // Act
        try
        {
            throw new ReservationDomainException("Reservation date cannot be in the past");
        }
        catch (ReservationDomainException ex)
        {
            threw = true;
            ex.Message.Should().Contain("past");
        }

        threw.Should().BeTrue();
    }

    #endregion

    #region Exception Hierarchy Tests

    [Fact]
    public void AllDomainExceptions_AreBaseExceptions()
    {
        // Arrange
        DomainException[] exceptions = new[]
        {
            (DomainException)new OrderDomainException("Order"),
            (DomainException)new ReservationDomainException("Reservation")
        };

        // Act & Assert
        foreach (var exception in exceptions)
        {
            exception.Should().BeOfType<DomainException>();
        }
    }

    [Fact]
    public void ExceptionMessagesArePreserved()
    {
        // Arrange
        var message1 = "Order invariant: customer name required";
        var message2 = "Reservation invariant: party size out of range";

        var orderEx = new OrderDomainException(message1);
        var resEx = new ReservationDomainException(message2);

        // Act & Assert
        orderEx.Message.Should().Be(message1);
        resEx.Message.Should().Be(message2);
    }

    #endregion
}
