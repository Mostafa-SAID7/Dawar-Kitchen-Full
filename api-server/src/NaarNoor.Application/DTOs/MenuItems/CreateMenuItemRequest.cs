using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.MenuItems;

/// <summary>
/// Request DTO for creating a new menu item.
/// ✅ Moved from API layer (MenuController.cs) to Application layer
/// </summary>
public class CreateMenuItemRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string? Name { get; set; }

    [MinLength(2, ErrorMessage = "English name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "English name must not exceed 100 characters")]
    public string? NameEn { get; set; }

    [MinLength(2, ErrorMessage = "Arabic name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Arabic name must not exceed 100 characters")]
    public string? NameAr { get; set; }

    [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    [MaxLength(500, ErrorMessage = "English description must not exceed 500 characters")]
    public string? DescriptionEn { get; set; }

    [MaxLength(500, ErrorMessage = "Arabic description must not exceed 500 characters")]
    public string? DescriptionAr { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = "Mains";

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal Price { get; set; }

    public bool IsVegetarian { get; set; }

    public bool IsVegan { get; set; }

    public bool IsGlutenFree { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Url(ErrorMessage = "ImageUrl must be a valid URL")]
    public string? ImageUrl { get; set; }

    [Range(0, 1000, ErrorMessage = "SortOrder must be between 0 and 1000")]
    public int SortOrder { get; set; }
}
