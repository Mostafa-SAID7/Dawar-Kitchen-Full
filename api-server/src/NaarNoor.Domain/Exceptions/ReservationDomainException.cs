namespace NaarNoor.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule violation occurs in the Reservation aggregate.
/// Examples: Invalid time slot, party size out of range, time slot already booked, etc.
/// </summary>
public class ReservationDomainException : DomainException
{
    /// <summary>
    /// Creates a new ReservationDomainException with the specified message.
    /// </summary>
    public ReservationDomainException(string message) : base(message) { }

    /// <summary>
    /// Creates a new ReservationDomainException with the specified message and inner exception.
    /// </summary>
    public ReservationDomainException(string message, Exception innerException) : base(message, innerException) { }
}
