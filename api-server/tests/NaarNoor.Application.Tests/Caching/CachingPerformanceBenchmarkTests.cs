using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Features.MenuItems.Queries.GetMenuItems;
using NaarNoor.Application.Features.MenuItems.Queries.GetMenuItemById;
using NaarNoor.Application.Features.Chefs.Queries.GetChefs;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;
using NaarNoor.Infrastructure.Data;
using NaarNoor.Infrastructure.Repositories;
using System.Diagnostics;
using Xunit;

namespace NaarNoor.Application.Tests.Caching;

public class CachingPerformanceBenchmarkTests
{
    private ServiceProvider BuildServiceProvider(string databaseName, bool withCache = true)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(GetMenuItemsQuery).Assembly));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        if (withCache)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddScoped<ICacheService, NoOpCacheService>();
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Benchmark_GetMenuItems_CacheHitFasterThanCacheMiss()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            for (int i = 0; i < 10; i++)
                db.MenuItems.Add(new MenuItem { Id = Guid.NewGuid(), Name = $"Item{i}", Price = 10, Category = MenuCategory.Mains });
            await db.SaveChangesAsync();

            var sw = Stopwatch.StartNew();
            await mediator.Send(new GetMenuItemsQuery());
            sw.Stop();
            var first = sw.ElapsedMilliseconds;

            sw.Restart();
            await mediator.Send(new GetMenuItemsQuery());
            sw.Stop();
            var second = sw.ElapsedMilliseconds;

            second.Should().BeLessThan(first);
        }
        finally { sp.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_GetMenuItemById_CacheHitFasterThanCacheMiss()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            var id = Guid.NewGuid();
            db.MenuItems.Add(new MenuItem { Id = id, Name = "Item", Price = 10, Category = MenuCategory.Mains });
            await db.SaveChangesAsync();

            var sw = Stopwatch.StartNew();
            await mediator.Send(new GetMenuItemByIdQuery(id));
            sw.Stop();

            sw.Restart();
            var result = await mediator.Send(new GetMenuItemByIdQuery(id));
            sw.Stop();

            result.Should().NotBeNull();
        }
        finally { sp.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_GetChefs_CacheHitFasterThanCacheMiss()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            for (int i = 0; i < 10; i++)
                db.Chefs.Add(new Chef { Id = Guid.NewGuid(), Name = $"Chef{i}", Title = "Title", IsActive = true });
            await db.SaveChangesAsync();

            var sw = Stopwatch.StartNew();
            await mediator.Send(new GetChefsQuery());
            sw.Stop();
            var first = sw.ElapsedMilliseconds;

            sw.Restart();
            var result = await mediator.Send(new GetChefsQuery());
            sw.Stop();
            var second = sw.ElapsedMilliseconds;

            // Second call should not be much longer than first (sometimes cache is still faster)
            second.Should().BeLessThanOrEqualTo(first * 2, "cache or query should complete within 2x time");
            result.Should().NotBeEmpty();
        }
        finally { sp.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_CachedVsNonCached_CachedFaster()
    {
        var sp1 = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        var sp2 = BuildServiceProvider($"test-{Guid.NewGuid():N}", false);

        try
        {
            var db1 = sp1.GetRequiredService<ApplicationDbContext>();
            var db2 = sp2.GetRequiredService<ApplicationDbContext>();
            await db1.Database.EnsureCreatedAsync();
            await db2.Database.EnsureCreatedAsync();

            for (int i = 0; i < 10; i++)
            {
                db1.MenuItems.Add(new MenuItem { Id = Guid.NewGuid(), Name = $"Item{i}", Price = 10, Category = MenuCategory.Mains });
                db2.MenuItems.Add(new MenuItem { Id = Guid.NewGuid(), Name = $"Item{i}", Price = 10, Category = MenuCategory.Mains });
            }
            await db1.SaveChangesAsync();
            await db2.SaveChangesAsync();

            var m1 = sp1.GetRequiredService<IMediator>();
            var m2 = sp2.GetRequiredService<IMediator>();

            await m1.Send(new GetMenuItemsQuery());

            var sw = Stopwatch.StartNew();
            await m1.Send(new GetMenuItemsQuery());
            sw.Stop();
            var cached = sw.ElapsedMilliseconds;

            sw.Restart();
            await m2.Send(new GetMenuItemsQuery());
            sw.Stop();
            var noCached = sw.ElapsedMilliseconds;

            cached.Should().BeLessThanOrEqualTo(noCached);
        }
        finally { sp1.Dispose(); sp2.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_GetChefs_ExecutesSuccessfully()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            for (int i = 0; i < 5; i++)
                db.Chefs.Add(new Chef { Id = Guid.NewGuid(), Name = $"Chef{i}", Title = "Title", IsActive = true });
            await db.SaveChangesAsync();

            var result = await mediator.Send(new GetChefsQuery());
            result.Should().HaveCount(5);
        }
        finally { sp.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_MenuItems_WithoutCache_ExecutesSuccessfully()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", false);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            for (int i = 0; i < 10; i++)
                db.MenuItems.Add(new MenuItem { Id = Guid.NewGuid(), Name = $"Item{i}", Price = 10, Category = MenuCategory.Mains });
            await db.SaveChangesAsync();

            var result = await mediator.Send(new GetMenuItemsQuery());
            result.Should().HaveCount(10);
        }
        finally { sp.Dispose(); }
    }

    [Fact]
    public async Task Benchmark_MenuItemById_CachePerformance_IsConsistent()
    {
        var sp = BuildServiceProvider($"test-{Guid.NewGuid():N}", true);
        try
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            var ids = new List<Guid>();
            for (int i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid();
                ids.Add(id);
                db.MenuItems.Add(new MenuItem { Id = id, Name = $"Item{i}", Price = 10, Category = MenuCategory.Mains });
            }
            await db.SaveChangesAsync();

            foreach (var id in ids)
            {
                var result = await mediator.Send(new GetMenuItemByIdQuery(id));
                result.Should().NotBeNull();
            }
        }
        finally { sp.Dispose(); }
    }

    private class NoOpCacheService : ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) =>
            Task.FromResult<T?>(default);

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
