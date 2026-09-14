using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.Reservations;

/// <summary>
/// Request DTO for updating a reservation.
/// ✅ Moved from API layer (ReservationsController.cs) to Application layer
/// ✅ Renamed from UpdateReservationBody to follow DTO naming convention
/// </summary>
public class UpdateReservationBodyRequest
{
    [MinLength(2, ErrorMessage = "CustomerName must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "CustomerName must not exceed 100 characters")]
    public string? CustomerName { get; set; }

    [Range(1, 20, ErrorMessage = "PartySize must be between 1 and 20")]
    public int? PartySize { get; set; }

    public DateTime? BookingTime { get; set; }

    public string? TableNumber { get; set; }

    [MaxLength(50, ErrorMessage = "Status must not exceed 50 characters")]
    public string? Status { get; set; }

    [EmailAddress(ErrorMessage = "CustomerEmail must be a valid email address")]
    public string? CustomerEmail { get; set; }

    [Phone(ErrorMessage = "CustomerPhone must be a valid phone number")]
    public string? CustomerPhone { get; set; }

    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "PhoneNumber must be a valid phone number")]
    public string? PhoneNumber { get; set; }

    [MaxLength(500, ErrorMessage = "SpecialRequests must not exceed 500 characters")]
    public string? SpecialRequests { get; set; }
}
