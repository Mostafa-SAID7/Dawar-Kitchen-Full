namespace NaarNoor.Application.DTOs.MenuItems;

/// <summary>
/// Data Transfer Object for MenuItem - matches exact Angular frontend contract
/// Returned by menu queries and endpoints
/// ✅ Moved from root DTOs folder to feature-organized structure
/// </summary>
public class MenuItemDto
{
    /// <summary>
    /// Unique identifier for the menu item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Item name (e.g., "Koshari", "Charcoal Kofta")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Item description with preparation details
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Item price in USD
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Category (e.g., "Egyptian Dishes", "Grills & Meat", "Sides & Rice")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Dietary flag: contains vegetable protein, no meat
    /// </summary>
    public bool IsVegetarian { get; set; }

    /// <summary>
    /// Dietary flag: no animal products
    /// </summary>
    public bool IsVegan { get; set; }

    /// <summary>
    /// Dietary flag: no gluten-containing ingredients
    /// </summary>
    public bool IsGlutenFree { get; set; }

    /// <summary>
    /// Availability flag: item can be ordered
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Public image URL (nullable) - stored path, resolved by frontend via Supabase
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Display order in menu (ascending)
    /// </summary>
    public int SortOrder { get; set; }
}
