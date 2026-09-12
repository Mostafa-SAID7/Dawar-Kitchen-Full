using Microsoft.EntityFrameworkCore;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Infrastructure.Data.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
        await SeedDataAsync(context);
    }

    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        if (!await context.MenuItems.AnyAsync())
            await SeedMenuItemsAsync(context);

        if (!await context.Chefs.AnyAsync())
            await SeedChefsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedMenuItemsAsync(ApplicationDbContext context)
    {
        var items = new List<MenuItem>
        {
            new() { Name = "Hawawshi", Description = "Handcrafted Egyptian crispy flatbread filled with spiced minced meat, served with tahini.", Price = 8.95m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 1 },
            new() { Name = "Veg Hawawshi", Description = "Handcrafted dumplings filled with seasoned cabbage, carrots and mushrooms, served with tomato chutney.", Price = 7.95m, Category = MenuCategory.Starters, IsVegetarian = true, IsVegan = true, SortOrder = 2 },
            new() { Name = "Kofta", Description = "Egyptian-style flame-grilled skewers of marinated minced meat, seasoned with traditional spices.", Price = 11.95m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 3 },
            new() { Name = "Aloo Tama", Description = "Traditional Coptic curry with bamboo shoots, black-eyed peas and potatoes in a tangy sauce.", Price = 12.95m, Category = MenuCategory.Mains, IsVegetarian = true, IsVegan = true, SortOrder = 1 },
            new() { Name = "Koshari", Description = "Egypt's national dish — lentil soup, steamed rice, seasonal vegetables and achaar (pickle).", Price = 14.95m, Category = MenuCategory.Mains, IsVegetarian = true, SortOrder = 2 },
            new() { Name = "Bamia with Lamb", Description = "Slow-braised Egyptian okra and lamb stew with aromatic spices, served with vermicelli rice.", Price = 18.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 3 },
            new() { Name = "Butter Chicken", Description = "Tender chicken in a rich, creamy tomato-based sauce with fenugreek and cardamom.", Price = 16.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 4 },
            new() { Name = "Sel Roti", Description = "Traditional Coptic rice bread, crispy on the outside, soft inside. Served with yoghurt.", Price = 4.50m, Category = MenuCategory.Breads, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Garlic Naan", Description = "Freshly baked leavened bread with roasted garlic and coriander from our tandoor.", Price = 3.95m, Category = MenuCategory.Breads, IsVegetarian = true, SortOrder = 2 },
            new() { Name = "Om Ali", Description = "Creamy Egyptian bread pudding infused with cardamom, rose water and topped with mixed nuts and raisins.", Price = 6.95m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Sikarni", Description = "Spiced strained yoghurt dessert with saffron, cardamom and dried fruits.", Price = 5.95m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 2 },
            new() { Name = "Butter Tea (Po Cha)", Description = "Traditional Middle Eastern butter tea brewed with Pu-erh tea, yak butter and salt.", Price = 3.95m, Category = MenuCategory.Beverages, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Masala Chai", Description = "Aromatic spiced tea with ginger, cardamom, cinnamon and steamed milk.", Price = 3.50m, Category = MenuCategory.Beverages, IsVegetarian = true, SortOrder = 2 },
            new() { Name = "Chef's Egyptian Tasting Menu", Description = "A seven-course journey through the streets of Cairo. Ask your server for today's selection.", Price = 55.00m, Category = MenuCategory.Specials, IsVegetarian = false, SortOrder = 1 },
        };
        await context.MenuItems.AddRangeAsync(items);
    }

    private static async Task SeedChefsAsync(ApplicationDbContext context)
    {
        var chefs = new List<Chef>
        {
            new() { Name = "Chef Mahmoud", Title = "Executive Chef", Bio = "Born in Cairo, Mahmoud trained under master chefs across Egypt before bringing authentic Egyptian flavours to London. His philosophy: fire is the soul of every dish.", Specialty = "Egyptian Grills & Kofta", IsActive = true, SortOrder = 1 },
            new() { Name = "Chef Fatima", Title = "Head Pastry Chef", Bio = "A native of Alexandria, Fatima blends traditional Egyptian sweet-making techniques with modern patisserie to create desserts that tell stories.", Specialty = "Egyptian Desserts & Breads", IsActive = true, SortOrder = 2 },
            new() { Name = "Omar Tariq", Title = "Sous Chef", Bio = "Rohan's deep knowledge of Middle Eastern and Sherpa cuisines brings the high-altitude flavours of the mountain communities to every plate.", Specialty = "Middle Eastern Cuisine & Noodles", IsActive = true, SortOrder = 3 },
        };
        await context.Chefs.AddRangeAsync(chefs);
    }
}
