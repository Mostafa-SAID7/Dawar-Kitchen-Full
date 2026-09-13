using NaarNoor.Domain.Common;

namespace NaarNoor.Domain.Entities;

/// <summary>
/// Customer review entity for menu items and dining experience feedback
/// </summary>
public class Review : BaseEntity
{
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = false;
}
