namespace NaarNoor.Application.DTOs.Reservations;

/// <summary>
/// Data Transfer Object for Reservation - matches exact Angular frontend contract
/// Returned by reservation queries and endpoints
/// ✅ Moved from root DTOs folder to feature-organized structure
/// </summary>
public class ReservationDto
{
    /// <summary>
    /// Unique identifier for the reservation
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Customer's full name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Reservation date (YYYY-MM-DD format)
    /// </summary>
    public DateOnly ReservationDate { get; set; }

    /// <summary>
    /// Reservation time (HH:mm format)
    /// </summary>
    public string ReservationTime { get; set; } = string.Empty;

    /// <summary>
    /// Number of guests (1-20)
    /// </summary>
    public int PartySize { get; set; }

    /// <summary>
    /// Reservation status (Pending, Confirmed, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Optional special requests (e.g., dietary restrictions, seating preference)
    /// </summary>
    public string? SpecialRequests { get; set; }

    /// <summary>
    /// Timestamp when reservation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
