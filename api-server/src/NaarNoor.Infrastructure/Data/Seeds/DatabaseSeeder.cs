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
            // Starters
            new() { Name = "Hawawshi", Description = "Handcrafted Egyptian crispy flatbread filled with spiced minced meat, served with tahini.", Price = 8.95m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 1 },
            new() { Name = "Veg Hawawshi", Description = "Handcrafted dumplings filled with seasoned cabbage, carrots and mushrooms, served with tomato chutney.", Price = 7.95m, Category = MenuCategory.Starters, IsVegetarian = true, IsVegan = true, SortOrder = 2 },
            new() { Name = "Kofta", Description = "Egyptian-style flame-grilled skewers of marinated minced meat, seasoned with traditional spices.", Price = 11.95m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 3 },
            new() { Name = "Ta'meya", Description = "Egyptian falafel made from fava beans, herbs, and spices, served with tahini sauce.", Price = 7.50m, Category = MenuCategory.Starters, IsVegetarian = true, IsVegan = true, SortOrder = 4 },
            new() { Name = "Kibbeh", Description = "Syrian bulgur wheat and lamb meatballs, deep-fried and served with tzatziki yogurt sauce.", Price = 9.95m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 5 },
            new() { Name = "Hummus with Lamb", Description = "Smooth chickpea puree topped with spiced ground lamb and pine nuts.", Price = 8.50m, Category = MenuCategory.Starters, IsVegetarian = false, SortOrder = 6 },
            new() { Name = "Baba Ghanoush", Description = "Roasted eggplant dip blended with tahini, lemon juice, and garlic.", Price = 6.95m, Category = MenuCategory.Starters, IsVegetarian = true, IsVegan = true, SortOrder = 7 },
            
            // Mains
            new() { Name = "Koshari", Description = "Egypt's national dish — lentil soup, steamed rice, seasonal vegetables and achaar (pickle).", Price = 14.95m, Category = MenuCategory.Mains, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Bamia with Lamb", Description = "Slow-braised Egyptian okra and lamb stew with aromatic spices, served with vermicelli rice.", Price = 18.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 2 },
            new() { Name = "Moussaka", Description = "Layered eggplant, minced lamb, and béchamel sauce baked until golden. Served with rice.", Price = 16.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 3 },
            new() { Name = "Shawarma (Chicken)", Description = "Marinated chicken cooked on a vertical spit, sliced thin and served with tahini sauce.", Price = 15.50m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 4 },
            new() { Name = "Shawarma (Lamb)", Description = "Seasoned lamb cooked on a vertical spit, sliced thin and served with lemon and garlic sauce.", Price = 16.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 5 },
            new() { Name = "Grilled Hammour (Sea Bass)", Description = "Fresh Gulf sea bass marinated in herbs and spices, grilled whole and served with rice.", Price = 22.95m, Category = MenuCategory.Mains, IsVegetarian = false, SortOrder = 6 },
            new() { Name = "Fattah", Description = "Crispy bread layered with rice, chickpeas, and spiced tomato-vinegar sauce, garnished with garlic.", Price = 13.95m, Category = MenuCategory.Mains, IsVegetarian = true, SortOrder = 7 },
            new() { Name = "Mahshi (Stuffed Vine Leaves)", Description = "Tender grape leaves stuffed with rice, herbs, and spices. Served with yogurt.", Price = 12.95m, Category = MenuCategory.Mains, IsVegetarian = true, IsVegan = true, SortOrder = 8 },
            
            // Breads
            new() { Name = "Pita Bread", Description = "Warm, fluffy Egyptian pita bread perfect for dipping or wrapping.", Price = 2.50m, Category = MenuCategory.Breads, IsVegetarian = true, IsVegan = true, SortOrder = 1 },
            new() { Name = "Manakish (Za'atar)", Description = "Flatbread topped with za'atar herb mix and olive oil.", Price = 4.50m, Category = MenuCategory.Breads, IsVegetarian = true, IsVegan = true, SortOrder = 2 },
            new() { Name = "Garlic Naan", Description = "Freshly baked leavened bread with roasted garlic and coriander.", Price = 3.95m, Category = MenuCategory.Breads, IsVegetarian = true, SortOrder = 3 },
            
            // Desserts
            new() { Name = "Om Ali", Description = "Creamy Egyptian bread pudding infused with cardamom, rose water and topped with mixed nuts and raisins.", Price = 6.95m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Basbousa", Description = "Coconut semolina cake soaked in sugar syrup and topped with an almond.", Price = 5.50m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 2 },
            new() { Name = "Kunafa", Description = "Shredded phyllo filled with nuts or cream, drizzled with honey and served warm.", Price = 7.95m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 3 },
            new() { Name = "Sikarni", Description = "Spiced strained yoghurt dessert with saffron, cardamom and dried fruits.", Price = 5.95m, Category = MenuCategory.Desserts, IsVegetarian = true, SortOrder = 4 },
            
            // Beverages
            new() { Name = "Masala Chai", Description = "Aromatic spiced tea with ginger, cardamom, cinnamon and steamed milk.", Price = 3.50m, Category = MenuCategory.Beverages, IsVegetarian = true, SortOrder = 1 },
            new() { Name = "Egyptian Hibiscus Tea (Karkade)", Description = "Refreshing cold hibiscus tea, naturally sweet with a tart finish.", Price = 3.00m, Category = MenuCategory.Beverages, IsVegetarian = true, IsVegan = true, SortOrder = 2 },
            new() { Name = "Fresh Mint Lemonade", Description = "Freshly squeezed lemon juice with mint leaves and a touch of honey.", Price = 3.75m, Category = MenuCategory.Beverages, IsVegetarian = true, IsVegan = true, SortOrder = 3 },
            
            // Specials
            new() { Name = "Chef's Egyptian Tasting Menu", Description = "A seven-course journey through the streets of Cairo. Ask your server for today's selection.", Price = 55.00m, Category = MenuCategory.Specials, IsVegetarian = false, SortOrder = 1 },
            new() { Name = "Chef's Syrian Feast", Description = "Five-course culinary adventure featuring the best of Syrian cuisine and traditions.", Price = 48.00m, Category = MenuCategory.Specials, IsVegetarian = false, SortOrder = 2 },
        };
        await context.MenuItems.AddRangeAsync(items);
    }

    private static async Task SeedChefsAsync(ApplicationDbContext context)
    {
        var chefs = new List<Chef>
        {
            new() { Name = "Chef Mahmoud", Title = "Executive Chef", Bio = "Born in Cairo, Mahmoud trained under master chefs across Egypt before bringing authentic Egyptian flavours to London. His philosophy: fire is the soul of every dish.", Specialty = "Egyptian Grills & Kofta", IsActive = true, SortOrder = 1 },
            new() { Name = "Chef Fatima", Title = "Head Pastry Chef", Bio = "A native of Alexandria, Fatima blends traditional Egyptian sweet-making techniques with modern patisserie to create desserts that tell stories of Mediterranean heritage.", Specialty = "Egyptian Desserts & Breads", IsActive = true, SortOrder = 2 },
            new() { Name = "Omar Tariq", Title = "Sous Chef", Bio = "From Damascus, Omar brings authentic Syrian recipes passed down through generations. He specializes in slow-cooked stews and grilled specialties that capture the essence of Levantine cuisine.", Specialty = "Syrian Mains & Shawarma", IsActive = true, SortOrder = 3 },
            new() { Name = "Chef Leila", Title = "Head Chef - Seafood", Bio = "With 15 years of experience in Gulf cuisine, Leila sources the freshest catch daily and prepares it with minimal spices to let the natural flavors shine.", Specialty = "Grilled Seafood & Gulf Specialties", IsActive = true, SortOrder = 4 },
            new() { Name = "Chef Karim", Title = "Grill Master", Bio = "A fourth-generation grill master from Cairo, Karim has mastered the art of open-flame cooking. Every kebab and shawarma that leaves his station is a masterpiece.", Specialty = "Kebabs, Shawarma & Open-Flame Grilling", IsActive = true, SortOrder = 5 },
            new() { Name = "Chef Amira", Title = "Vegetarian Specialist", Bio = "Amira believes vegetables deserve the same respect and skill as meat. She creates vibrant, nutritious dishes that prove vegetarian cuisine can be just as exciting.", Specialty = "Vegetarian & Vegan Creations", IsActive = true, SortOrder = 6 },
            new() { Name = "Chef Hassan", Title = "Sous Chef - Preparation", Bio = "Hassan is the backbone of our kitchen, ensuring every ingredient is sourced ethically and prepared to perfection. His attention to detail is unmatched.", Specialty = "Ingredient Sourcing & Mise en Place", IsActive = true, SortOrder = 7 },
            new() { Name = "Chef Zainab", Title = "Head Chef - Bread & Pastry", Bio = "Zainab learned the ancient art of bread-making from her grandmother in Aleppo. Each loaf and pastry is crafted with love and tradition.", Specialty = "Traditional Breads, Pastries & Manakish", IsActive = true, SortOrder = 8 },
        };
        await context.Chefs.AddRangeAsync(chefs);
    }
}
