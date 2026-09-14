using FluentAssertions;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;
using NaarNoor.Domain.Exceptions;
using Xunit;

namespace NaarNoor.Domain.Tests.Aggregates;

/// <summary>
/// Domain unit tests for Reservation aggregate.
/// ✅ Tests domain-driven factory method, overlap detection, and state transitions
/// </summary>
public class ReservationAggregateTests
{
    #region Reservation.Create Factory Tests

    [Fact]
    public void Create_WithValidInputs_ReturnsReservationWithCorrectProperties()
    {
        // Arrange
        var customerName = "Jane Smith";
        var email = "jane@example.com";
        var phoneNumber = "555-5678";
        var reservationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
        var reservationTime = new TimeOnly(19, 0);
        var partySize = 4;
        var specialRequests = "Window seat";

        // Act
        var reservation = Reservation.Create(
            customerName, email, phoneNumber, reservationDate, reservationTime, partySize, specialRequests);

        // Assert
        reservation.CustomerName.Should().Be(customerName);
        reservation.Email.Should().Be(email.ToLowerInvariant());
        reservation.PhoneNumber.Should().Be(phoneNumber);
        reservation.ReservationDate.Should().Be(reservationDate);
        reservation.ReservationTime.Should().Be(reservationTime);
        reservation.PartySize.Should().Be(partySize);
        reservation.SpecialRequests.Should().Be(specialRequests);
        reservation.Id.Should().NotBe(Guid.Empty);
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyCustomerName_ThrowsReservationDomainException(string customerName)
    {
        // Act & Assert
        Assert.Throws<ReservationDomainException>(() =>
            Reservation.Create(customerName, "test@example.com", "555-1234", 
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ThrowsReservationDomainException(string email)
    {
        // Act & Assert
        Assert.Throws<ReservationDomainException>(() =>
            Reservation.Create("Jane", email, "555-1234", 
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-1)]
    public void Create_WithInvalidPartySize_ThrowsReservationDomainException(int partySize)
    {
        // Act & Assert
        Assert.Throws<ReservationDomainException>(() =>
            Reservation.Create("Jane", "jane@example.com", "555-1234", 
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), partySize, null));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Create_WithValidPartySize_Succeeds(int partySize)
    {
        // Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), partySize, null);

        // Assert
        reservation.PartySize.Should().Be(partySize);
    }

    #endregion

    #region Reservation Date/Time Validation Tests

    [Fact]
    public void Create_WithPastDate_ThrowsReservationDomainException()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

        // Act & Assert
        Assert.Throws<ReservationDomainException>(() =>
            Reservation.Create("Jane", "jane@example.com", "555-1234", 
                pastDate, new TimeOnly(19, 0), 2, null));
    }

    [Fact]
    public void Create_WithFutureDate_Succeeds()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5));

        // Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            futureDate, new TimeOnly(19, 0), 2, null);

        // Assert
        reservation.ReservationDate.Should().Be(futureDate);
    }

    #endregion

    #region TimeSlot Value Object Tests

    [Fact]
    public void GetTimeSlot_ReturnsTimeSlotWithReservationTime()
    {
        // Arrange
        var reservationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var reservationTime = new TimeOnly(19, 30);

        // Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            reservationDate, reservationTime, 2, null);
        var timeSlot = reservation.GetTimeSlot();

        // Assert
        timeSlot.Should().NotBeNull();
        timeSlot.StartTime.Should().Be(reservationTime);
        (timeSlot.EndTime > timeSlot.StartTime).Should().BeTrue(); // End time is 2 hours later
    }

    #endregion

    #region Overlap Detection Tests

    [Fact]
    public void OverlapsWith_WithDifferentDates_ReturnsFalse()
    {
        // Arrange
        var date1 = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var date2 = DateOnly.FromDateTime(DateTime.Now.AddDays(6));
        var time = new TimeOnly(19, 0);

        var reservation1 = Reservation.Create("Jane", "jane@example.com", "555-1234", date1, time, 2, null);
        var reservation2 = Reservation.Create("John", "john@example.com", "555-5678", date2, time, 2, null);

        // Act
        var overlaps = reservation1.OverlapsWith(reservation2);

        // Assert
        overlaps.Should().BeFalse();
    }

    [Fact]
    public void OverlapsWith_WithSameDateDifferentTimes_ConsidersTimeSlots()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var time1 = new TimeOnly(19, 0);
        var time2 = new TimeOnly(22, 0); // 3 hours later - outside 2-hour slot

        var reservation1 = Reservation.Create("Jane", "jane@example.com", "555-1234", date, time1, 2, null);
        var reservation2 = Reservation.Create("John", "john@example.com", "555-5678", date, time2, 2, null);

        // Act
        var overlaps = reservation1.OverlapsWith(reservation2);

        // Assert
        overlaps.Should().BeFalse();
    }

    #endregion

    #region State Machine Tests

    [Fact]
    public void NewReservation_HasPendingStatus()
    {
        // Arrange & Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public void Confirm_TransitionsFromPendingToConfirmed()
    {
        // Arrange
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);

        // Act
        reservation.Confirm();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Complete_TransitionsFromConfirmedToCompleted()
    {
        // Arrange
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);
        reservation.Confirm();

        // Act
        reservation.Complete();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Completed);
    }

    [Fact]
    public void Cancel_TransitionsFromPendingToCancelled()
    {
        // Arrange
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);

        // Act
        reservation.Cancel();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void IsTerminal_ReturnsTrueForCompletedOrCancelled()
    {
        // Arrange
        var reservation1 = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);
        var reservation2 = Reservation.Create("John", "john@example.com", "555-5678", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(2)), new TimeOnly(20, 0), 2, null);

        // Act
        reservation1.Complete();
        reservation1.Confirm(); // Confirm first
        reservation2.Cancel();

        // Assert
        reservation1.IsTerminal.Should().BeTrue();
        reservation2.IsTerminal.Should().BeTrue();
    }

    #endregion

    #region Special Requests Tests

    [Fact]
    public void Create_WithSpecialRequests_StoresRequest()
    {
        // Arrange
        var specialRequests = "Vegetarian menu, nut allergy";

        // Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, specialRequests);

        // Assert
        reservation.SpecialRequests.Should().Be(specialRequests);
    }

    [Fact]
    public void Create_WithoutSpecialRequests_HasNull()
    {
        // Act
        var reservation = Reservation.Create("Jane", "jane@example.com", "555-1234", 
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new TimeOnly(19, 0), 2, null);

        // Assert
        reservation.SpecialRequests.Should().BeNull();
    }

    #endregion
}
