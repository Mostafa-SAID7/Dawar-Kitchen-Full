namespace NaarNoor.Application.DTOs;

/// <summary>
/// Data Transfer Object for Chef - matches exact Angular frontend contract
/// Returned by chef queries and endpoints
/// </summary>
public class ChefDto
{
    /// <summary>
    /// Unique identifier for the chef
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Chef's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Chef's title (e.g., "Executive Chef", "Sous Chef")
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Chef's biography and background
    /// </summary>
    public string Bio { get; set; } = string.Empty;

    /// <summary>
    /// Public image URL (nullable) - stored path, resolved by frontend via Supabase
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Chef's culinary specialty (e.g., "Charcoal Grill & Egyptian Mains")
    /// </summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>
    /// Display order in chef list (ascending)
    /// </summary>
    public int SortOrder { get; set; }
}
