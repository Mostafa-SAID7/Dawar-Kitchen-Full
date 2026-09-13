using NaarNoor.Domain.Common;
using NaarNoor.Domain.Enums;
using NaarNoor.Domain.Exceptions;
using NaarNoor.Domain.ValueObjects;

namespace NaarNoor.Domain.Entities;

/// <summary>
/// Reservation aggregate root representing a restaurant reservation.
/// This is a true aggregate with business logic and state management.
/// 
/// Key responsibilities:
/// - Enforce reservation business rules (valid party size, valid times, etc.)
/// - Manage state transitions using state machine pattern
/// - Validate time slots using TimeSlot value object
/// - Support full lifecycle (Pending → Confirmed → Completed or Cancelled)
/// </summary>
public class Reservation : BaseEntity
{
    // Constants for business rules
    private const int MinPartySize = 1;
    private const int MaxPartySize = 100;

    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? SpecialRequests { get; set; }

    private static readonly Dictionary<ReservationStatus, ReservationStatus[]> ValidTransitions = new()
    {
        [ReservationStatus.Pending] = new[] { ReservationStatus.Confirmed, ReservationStatus.Cancelled },
        [ReservationStatus.Confirmed] = new[] { ReservationStatus.Completed, ReservationStatus.Cancelled },
        [ReservationStatus.Cancelled] = Array.Empty<ReservationStatus>(),
        [ReservationStatus.Completed] = Array.Empty<ReservationStatus>(),
    };

    /// <summary>
    /// Factory method for creating a new Reservation.
    /// Validates all required fields and business rules.
    /// </summary>
    /// <exception cref="ReservationDomainException">Thrown when reservation data is invalid.</exception>
    public static Reservation Create(
        string customerName,
        string email,
        string phoneNumber,
        DateOnly reservationDate,
        TimeOnly reservationTime,
        int partySize,
        string? specialRequests = null)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ReservationDomainException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ReservationDomainException("Customer email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ReservationDomainException("Customer phone number is required.");

        // Validate party size
        if (partySize < MinPartySize || partySize > MaxPartySize)
            throw new ReservationDomainException($"Party size must be between {MinPartySize} and {MaxPartySize}.");

        // Validate reservation is in the future
        var reservationDateTime = new DateTime(reservationDate.Year, reservationDate.Month, reservationDate.Day)
            .Add(reservationTime.ToTimeSpan());

        if (reservationDateTime <= DateTime.UtcNow.AddMinutes(-5)) // Allow 5-minute grace period
            throw new ReservationDomainException("Reservation date and time must be in the future.");

        return new Reservation
        {
            CustomerName = customerName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber.Trim(),
            ReservationDate = reservationDate,
            ReservationTime = reservationTime,
            PartySize = partySize,
            Status = ReservationStatus.Pending,
            SpecialRequests = specialRequests?.Trim()
        };
    }

    /// <summary>
    /// Gets a TimeSlot value object representing the reservation time.
    /// Duration is fixed at 2 hours per reservation.
    /// </summary>
    public TimeSlot GetTimeSlot()
    {
        var endTime = ReservationTime.AddHours(2);
        if (endTime.Hour < ReservationTime.Hour) // Handle day boundary
            endTime = new TimeOnly(23, 59, 59);

        return new TimeSlot(ReservationTime, endTime);
    }

    /// <summary>
    /// Checks if the reservation overlaps with another reservation's time slot.
    /// </summary>
    public bool OverlapsWith(Reservation other)
    {
        if (other is null)
            return false;

        // Only check overlap if reservations are on the same date
        if (ReservationDate != other.ReservationDate)
            return false;

        // Both must be in non-terminal states to be considered overlapping
        if (IsTerminal || other.IsTerminal)
            return false;

        var thisSlot = GetTimeSlot();
        var otherSlot = other.GetTimeSlot();

        return thisSlot.Overlaps(otherSlot);
    }

    /// <summary>
    /// Transitions the reservation to a new status, enforcing the valid state machine rules.
    /// Pending -> Confirmed | Cancelled
    /// Confirmed -> Completed | Cancelled
    /// Cancelled and Completed are terminal states.
    /// </summary>
    /// <exception cref="ReservationDomainException">Thrown when the requested transition is not allowed.</exception>
    public void TransitionTo(ReservationStatus newStatus)
    {
        if (!ValidTransitions.TryGetValue(Status, out var allowedNextStates) ||
            !allowedNextStates.Contains(newStatus))
        {
            throw new ReservationDomainException(
                $"Cannot transition reservation from '{Status}' to '{newStatus}'.");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirms the reservation (transitions from Pending to Confirmed).
    /// </summary>
    public void Confirm()
    {
        TransitionTo(ReservationStatus.Confirmed);
    }

    /// <summary>
    /// Completes the reservation (transitions from Confirmed to Completed).
    /// </summary>
    public void Complete()
    {
        TransitionTo(ReservationStatus.Completed);
    }

    /// <summary>
    /// Cancels the reservation (can transition from Pending or Confirmed to Cancelled).
    /// </summary>
    public void Cancel()
    {
        TransitionTo(ReservationStatus.Cancelled);
    }

    /// <summary>
    /// Verifies that the reservation satisfies all business rule invariants:
    /// a positive party size, a valid (defined) status, and non-empty contact details.
    /// </summary>
    public bool IsInValidState()
    {
        return PartySize >= MinPartySize
            && PartySize <= MaxPartySize
            && Enum.IsDefined(typeof(ReservationStatus), Status)
            && !string.IsNullOrWhiteSpace(CustomerName)
            && !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(PhoneNumber);
    }

    /// <summary>
    /// Indicates whether the reservation is in a terminal state (Completed or Cancelled).
    /// </summary>
    public bool IsTerminal => Status == ReservationStatus.Completed || Status == ReservationStatus.Cancelled;
}
