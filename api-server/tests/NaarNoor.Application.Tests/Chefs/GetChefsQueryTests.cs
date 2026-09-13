using FluentAssertions;
using NaarNoor.Application.Chefs.Queries.GetChefs;
using NaarNoor.Application.DTOs;
using Xunit;

namespace NaarNoor.Application.Tests.Chefs;

public class GetChefsQueryTests
{
    [Fact]
    public void GetChefsQuery_CanBeInstantiated()
    {
        var query = new GetChefsQuery();

        query.Should().NotBeNull();
    }

    [Fact]
    public void GetChefsQuery_TwoInstances_AreEqual()
    {
        var q1 = new GetChefsQuery();
        var q2 = new GetChefsQuery();

        q1.Should().Be(q2, "parameterless records with same type are equal");
    }

    [Fact]
    public void GetChefsQuery_GetHashCode_IsConsistent()
    {
        var query = new GetChefsQuery();

        query.GetHashCode().Should().Be(query.GetHashCode());
    }

    [Fact]
    public void GetChefsQuery_ToString_ReturnsNonEmpty()
    {
        var query = new GetChefsQuery();

        query.ToString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetChefsQuery_Equals_SameInstance_IsTrue()
    {
        var query = new GetChefsQuery();

        query.Equals(query).Should().BeTrue();
    }

    [Fact]
    public void GetChefsQuery_Equals_Null_IsFalse()
    {
        var query = new GetChefsQuery();

        query.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void ChefDto_CanBeConstructed_WithAllFields()
    {
        var id = Guid.NewGuid();

        var dto = new ChefDto 
        { 
            Id = id, 
            Name = "Mahmoud Hassan", 
            Title = "Executive Chef", 
            Bio = "Expert in Egyptian cuisine", 
            ImageUrl = null, 
            Specialty = "Grills", 
            SortOrder = 1 
        };

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Mahmoud Hassan");
        dto.Title.Should().Be("Executive Chef");
        dto.Bio.Should().Be("Expert in Egyptian cuisine");
        dto.ImageUrl.Should().BeNull();
        dto.Specialty.Should().Be("Grills");
        dto.SortOrder.Should().Be(1);
    }

    [Fact]
    public void ChefDto_ImageUrl_CanBeProvided()
    {
        var dto = new ChefDto 
        { 
            Id = Guid.NewGuid(), 
            Name = "Nisha", 
            Title = "Head Pastry Chef", 
            Bio = "Bio", 
            ImageUrl = "https://example.com/photo.jpg", 
            Specialty = "Desserts", 
            SortOrder = 2 
        };

        dto.ImageUrl.Should().Be("https://example.com/photo.jpg");
    }

    [Fact]
    public void ChefDto_TwoIdenticalInstances_AreEqual()
    {
        var id = Guid.NewGuid();

        var dto1 = new ChefDto 
        { 
            Id = id, 
            Name = "Rohan", 
            Title = "Sous Chef", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Middle Eastern", 
            SortOrder = 3 
        };
        var dto2 = new ChefDto 
        { 
            Id = id, 
            Name = "Rohan", 
            Title = "Sous Chef", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Middle Eastern", 
            SortOrder = 3 
        };

        dto1.Should().Be(dto2);
    }

    [Fact]
    public void ChefDto_DifferentSortOrders_AreNotEqual()
    {
        var id = Guid.NewGuid();

        var dto1 = new ChefDto 
        { 
            Id = id, 
            Name = "Chef", 
            Title = "Title", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Specialty", 
            SortOrder = 1 
        };
        var dto2 = new ChefDto 
        { 
            Id = id, 
            Name = "Chef", 
            Title = "Title", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Specialty", 
            SortOrder = 2 
        };

        dto1.Should().NotBe(dto2);
    }

    [Fact]
    public void ChefDto_ToString_ContainsTypeName()
    {
        var dto = new ChefDto 
        { 
            Id = Guid.NewGuid(), 
            Name = "Chef", 
            Title = "Title", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Specialty", 
            SortOrder = 1 
        };

        dto.ToString().Should().Contain("ChefDto");
    }

    [Fact]
    public void ChefDto_GetHashCode_IsConsistent()
    {
        var dto = new ChefDto 
        { 
            Id = Guid.NewGuid(), 
            Name = "Chef", 
            Title = "Title", 
            Bio = "Bio", 
            ImageUrl = null, 
            Specialty = "Specialty", 
            SortOrder = 1 
        };

        dto.GetHashCode().Should().Be(dto.GetHashCode());
    }
}
