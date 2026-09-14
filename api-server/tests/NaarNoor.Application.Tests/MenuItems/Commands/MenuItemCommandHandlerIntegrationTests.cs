using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Features.MenuItems.Commands.CreateMenuItem;
using NaarNoor.Application.Features.MenuItems.Commands.UpdateMenuItem;
using NaarNoor.Application.Features.MenuItems.Commands.DeleteMenuItem;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;
using NaarNoor.Infrastructure.Data;
using NaarNoor.Infrastructure.Repositories;
using Xunit;

namespace NaarNoor.Application.Tests.MenuItems.Commands;

/// <summary>
/// Integration tests for MenuItem command handlers.
/// ✅ Tests complete vertical slices: CreateMenuItem → UpdateMenuItem → DeleteMenuItem
/// Uses in-memory DbContext with full MediatR + DI integration.
/// Each test creates its own isolated DbContext and ServiceProvider for test independence.
/// </summary>
public class MenuItemCommandHandlerIntegrationTests
{
    private ServiceProvider BuildServiceProvider(string databaseName)
    {
        var services = new ServiceCollection();
        
        // Add DbContext (in-memory for testing)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        // Add MediatR
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(CreateMenuItemCommand).Assembly));

        // Add Infrastructure services (repositories, UoW)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add distributed memory cache for cache service
        services.AddDistributedMemoryCache();
        services.AddScoped<ICacheService, DistributedCacheService>();

        return services.BuildServiceProvider();
    }

    #region Create MenuItem Tests

    [Fact]
    public async Task CreateMenuItem_WithValidData_SucceedsAndReturnsMenuItemId()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var createCommand = new CreateMenuItemCommand(
                Name: "Koshari",
                Description: "Traditional Egyptian pasta dish with tomato sauce and chickpeas",
                Price: 25.50m,
                Category: MenuCategory.Mains.ToString(),
                IsVegetarian: true,
                IsVegan: true,
                IsGlutenFree: false,
                IsAvailable: true,
                ImageUrl: "https://example.com/koshari.jpg",
                SortOrder: 1
            );

            // Act
            var menuItemId = await mediator.Send(createCommand);

            // Assert
            menuItemId.Should().NotBe(Guid.Empty);
            var menuItem = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItem.Should().NotBeNull();
            menuItem!.Name.Should().Be("Koshari");
            menuItem.Price.Should().Be(25.50m);
            menuItem.Category.Should().Be(MenuCategory.Mains);
            menuItem.IsVegetarian.Should().BeTrue();
            menuItem.IsVegan.Should().BeTrue();
            menuItem.IsAvailable.Should().BeTrue();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateMenuItem_WithAllCategories_Succeeds()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var categories = new[]
            {
                ("Hummus", MenuCategory.Starters.ToString()),
                ("Falafel Wrap", MenuCategory.Mains.ToString()),
                ("Pita Bread", MenuCategory.Breads.ToString()),
                ("Baklava", MenuCategory.Desserts.ToString()),
                ("Mint Tea", MenuCategory.Beverages.ToString()),
                ("Seasonal Special", MenuCategory.Specials.ToString()),
            };

            var ids = new List<Guid>();
            foreach (var (name, category) in categories)
            {
                var createCommand = new CreateMenuItemCommand(
                    name,
                    $"Description for {name}",
                    10.00m + ids.Count,
                    category,
                    true,
                    true,
                    false,
                    true,
                    null,
                    ids.Count
                );
                ids.Add(await mediator.Send(createCommand));
            }

            // Assert
            var allItems = await dbContext.MenuItems.ToListAsync();
            allItems.Should().HaveCount(6);
            
            allItems.Should().Contain(m => m.Category == MenuCategory.Starters && m.Name == "Hummus");
            allItems.Should().Contain(m => m.Category == MenuCategory.Mains && m.Name == "Falafel Wrap");
            allItems.Should().Contain(m => m.Category == MenuCategory.Breads && m.Name == "Pita Bread");
            allItems.Should().Contain(m => m.Category == MenuCategory.Desserts && m.Name == "Baklava");
            allItems.Should().Contain(m => m.Category == MenuCategory.Beverages && m.Name == "Mint Tea");
            allItems.Should().Contain(m => m.Category == MenuCategory.Specials && m.Name == "Seasonal Special");
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateMenuItem_WithInvalidCategory_DefaultsToMains()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var createCommand = new CreateMenuItemCommand(
                "Invalid Category Item",
                "This item has an invalid category",
                15.00m,
                "InvalidCategory", // Invalid category
                false,
                false,
                false,
                true,
                null,
                1
            );

            // Act
            var menuItemId = await mediator.Send(createCommand);

            // Assert
            var menuItem = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItem!.Category.Should().Be(MenuCategory.Mains); // Should default to Mains
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateMenuItem_WithNullDescription_UsesEmptyString()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var createCommand = new CreateMenuItemCommand(
                "Simple Item",
                null, // Null description
                20.00m,
                MenuCategory.Starters.ToString(),
                false,
                false,
                false,
                true,
                null,
                1
            );

            // Act
            var menuItemId = await mediator.Send(createCommand);

            // Assert
            var menuItem = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItem!.Description.Should().Be(string.Empty);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Update MenuItem Tests

    [Fact]
    public async Task UpdateMenuItem_WithValidData_SucceedsAndUpdatesFields()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            // Create a menu item
            var createCommand = new CreateMenuItemCommand(
                "Original Name",
                "Original Description",
                10.00m,
                MenuCategory.Mains.ToString(),
                true,
                false,
                false,
                true,
                null,
                1
            );
            var menuItemId = await mediator.Send(createCommand);

            // Act: Update the menu item
            var updateCommand = new UpdateMenuItemCommand(
                Id: menuItemId,
                Name: "Updated Name",
                Description: "Updated Description",
                Price: 15.00m,
                Category: MenuCategory.Desserts.ToString(),
                IsVegetarian: false,
                IsVegan: null,
                IsGlutenFree: true,
                IsAvailable: false,
                ImageUrl: "https://example.com/updated.jpg"
            );
            var result = await mediator.Send(updateCommand);

            // Assert
            result.Should().BeTrue();
            var menuItem = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItem!.Name.Should().Be("Updated Name");
            menuItem.Description.Should().Be("Updated Description");
            menuItem.Price.Should().Be(15.00m);
            menuItem.Category.Should().Be(MenuCategory.Desserts);
            menuItem.IsVegetarian.Should().BeFalse();
            menuItem.IsGlutenFree.Should().BeTrue();
            menuItem.IsAvailable.Should().BeFalse();
            menuItem.ImageUrl.Should().Be("https://example.com/updated.jpg");
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task UpdateMenuItem_WithPartialData_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            // Create a menu item
            var createCommand = new CreateMenuItemCommand(
                "Original Name",
                "Original Description",
                10.00m,
                MenuCategory.Mains.ToString(),
                true,
                false,
                false,
                true,
                "https://example.com/original.jpg",
                1
            );
            var menuItemId = await mediator.Send(createCommand);

            var menuItemBefore = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItemBefore!.IsVegetarian.Should().BeTrue();

            // Act: Update only name and price (other fields should remain unchanged)
            var updateCommand = new UpdateMenuItemCommand(
                Id: menuItemId,
                Name: "Updated Name",
                Description: null, // Not updating
                Price: 15.00m,
                Category: null, // Not updating
                IsVegetarian: null, // Not updating
                IsVegan: null,
                IsGlutenFree: null,
                IsAvailable: null,
                ImageUrl: null // Not updating
            );
            var result = await mediator.Send(updateCommand);

            // Assert
            result.Should().BeTrue();
            var menuItem = await dbContext.MenuItems.FindAsync(menuItemId);
            menuItem!.Name.Should().Be("Updated Name");
            menuItem.Price.Should().Be(15.00m);
            menuItem.Description.Should().Be("Original Description"); // Unchanged
            menuItem.Category.Should().Be(MenuCategory.Mains); // Unchanged
            menuItem.IsVegetarian.Should().BeTrue(); // Unchanged
            menuItem.ImageUrl.Should().Be("https://example.com/original.jpg"); // Unchanged
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task UpdateMenuItem_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var nonExistentId = Guid.NewGuid();

            // Act
            var updateCommand = new UpdateMenuItemCommand(
                Id: nonExistentId,
                Name: "Updated Name",
                Description: null,
                Price: 15.00m,
                Category: null,
                IsVegetarian: null,
                IsVegan: null,
                IsGlutenFree: null,
                IsAvailable: null,
                ImageUrl: null
            );
            var result = await mediator.Send(updateCommand);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Delete MenuItem Tests

    [Fact]
    public async Task DeleteMenuItem_WithValidId_SucceedsAndRemovesMenuItem()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            // Create a menu item
            var createCommand = new CreateMenuItemCommand(
                "Delete Test",
                "This will be deleted",
                10.00m,
                MenuCategory.Mains.ToString(),
                true,
                false,
                false,
                true,
                null,
                1
            );
            var menuItemId = await mediator.Send(createCommand);

            // Verify it was created
            var createdItem = await dbContext.MenuItems.FindAsync(menuItemId);
            createdItem.Should().NotBeNull();

            // Act
            var deleteCommand = new DeleteMenuItemCommand(menuItemId);
            var result = await mediator.Send(deleteCommand);

            // Assert
            result.Should().BeTrue();
            var deletedItem = await dbContext.MenuItems.FindAsync(menuItemId);
            deletedItem.Should().BeNull();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task DeleteMenuItem_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var nonExistentId = Guid.NewGuid();

            // Act
            var deleteCommand = new DeleteMenuItemCommand(nonExistentId);
            var result = await mediator.Send(deleteCommand);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Data Persistence Tests

    [Fact]
    public async Task MenuItemData_IsPersisted_AndCanBeRetrieved()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var createCommand = new CreateMenuItemCommand(
                "Persistence Test Item",
                "This item tests data persistence",
                30.00m,
                MenuCategory.Starters.ToString(),
                true,
                true,
                true,
                true,
                "https://example.com/item.jpg",
                1
            );

            // Act
            var menuItemId = await mediator.Send(createCommand);

            // Query the item from the database
            var retrieved = await dbContext.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(menuItemId);
            retrieved.Name.Should().Be("Persistence Test Item");
            retrieved.Description.Should().Be("This item tests data persistence");
            retrieved.Price.Should().Be(30.00m);
            retrieved.Category.Should().Be(MenuCategory.Starters);
            retrieved.IsVegetarian.Should().BeTrue();
            retrieved.IsVegan.Should().BeTrue();
            retrieved.IsGlutenFree.Should().BeTrue();
            retrieved.IsAvailable.Should().BeTrue();
            retrieved.ImageUrl.Should().Be("https://example.com/item.jpg");
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task MultipleMenuItems_CanBeCreatedAndUpdatedIndependently()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var items = new[]
            {
                ("Item1", 10.00m, MenuCategory.Mains.ToString()),
                ("Item2", 15.00m, MenuCategory.Starters.ToString()),
                ("Item3", 20.00m, MenuCategory.Desserts.ToString()),
            };

            var ids = new List<Guid>();

            // Act: Create items
            foreach (var (name, price, category) in items)
            {
                var createCommand = new CreateMenuItemCommand(
                    name,
                    $"Description for {name}",
                    price,
                    category,
                    true,
                    true,
                    false,
                    true,
                    null,
                    ids.Count
                );
                ids.Add(await mediator.Send(createCommand));
            }

            // Update first item
            await mediator.Send(new UpdateMenuItemCommand(
                ids[0],
                "Item1 Updated",
                null,
                12.00m,
                null,
                null,
                null,
                null,
                null,
                null
            ));

            // Assert
            var allItems = await dbContext.MenuItems.ToListAsync();
            allItems.Should().HaveCount(3);
            
            allItems.Should().Contain(m => m.Id == ids[0] && m.Name == "Item1 Updated" && m.Price == 12.00m);
            allItems.Should().Contain(m => m.Id == ids[1] && m.Name == "Item2" && m.Price == 15.00m);
            allItems.Should().Contain(m => m.Id == ids[2] && m.Name == "Item3" && m.Price == 20.00m);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion
}
