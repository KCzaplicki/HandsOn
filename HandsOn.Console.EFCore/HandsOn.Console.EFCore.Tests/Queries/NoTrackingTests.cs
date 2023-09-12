using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about AsNoTracking on
// https://learn.microsoft.com/en-us/ef/core/querying/tracking

public class NoTrackingTests : QueryTestBase
{
    [Fact]
    public async Task AsNoTrackingCreatesNewObjectForTheSameEntities()
    {
        await CreateDatabaseAsync();
        var dbContext = new HandsOnQueriesDbContext();

        var posts = await dbContext.Posts
            .Include(x => x.Categories)
            .AsNoTracking()
            .ToListAsync();

        posts.Should().NotBeNull();
        posts[0].Categories.Should().NotBeNull();

        var firstPostCategory = posts[0].Categories.First();
        var secondPostCategory = posts[1].Categories.First();
        
        firstPostCategory.Should().NotBeSameAs(secondPostCategory);
        
        firstPostCategory.CategoryId.Should().Be(secondPostCategory.CategoryId);
        firstPostCategory.Name.Should().Be(secondPostCategory.Name);
    }
    
    [Fact]
    public async Task AsNoTrackingWithIdentityResolutionCreatesOneObjectForTheSameEntities()
    {
        await CreateDatabaseAsync();
        var dbContext = new HandsOnQueriesDbContext();

        var posts = await dbContext.Posts
            .Include(x => x.Categories)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

        posts.Should().NotBeNull();
        posts[0].Categories.Should().NotBeNull();

        var firstPostCategory = posts[0].Categories.First();
        var secondPostCategory = posts[1].Categories.First();
        
        firstPostCategory.Should().BeSameAs(secondPostCategory);
        
        firstPostCategory.CategoryId.Should().Be(secondPostCategory.CategoryId);
        firstPostCategory.Name.Should().Be(secondPostCategory.Name);
    }

    [Fact]
    public async Task TrackingCacheEntityInChangeTrackerByDefault()
    {
        await CreateDatabaseAsync();
        var dbContext = new HandsOnQueriesDbContext();

        dbContext.ChangeTracker.Entries<Blog>().Should().BeEmpty();
        
        var blog = await dbContext.Blogs.FirstAsync();
        
        dbContext.ChangeTracker.Entries<Blog>().Should().NotBeEmpty();
        dbContext.ChangeTracker.Entries<Blog>().Should().Contain(x => x.Entity.BlogId == blog.BlogId);
    }
    
    [Fact]
    public async Task AsNoTrackingDoesNotCacheEntityInChangeTracker()
    {
        await CreateDatabaseAsync();
        var dbContext = new HandsOnQueriesDbContext();

        dbContext.ChangeTracker.Entries<Blog>().Should().BeEmpty();
        
        _ = await dbContext.Blogs.AsNoTracking().FirstAsync();

        dbContext.ChangeTracker.Entries<Blog>().Should().BeEmpty();
    }
    
    [Fact]
    public async Task TrackingCacheEntitiesFromSelectInChangeTrackerByDefault()
    {
        await CreateDatabaseAsync();
        var dbContext = new HandsOnQueriesDbContext();

        dbContext.ChangeTracker.Entries<Category>().Should().BeEmpty();
        
        var posts = await dbContext.Posts.Select(x => new { x.PostId, x.Title, x.Categories }).FirstAsync();
        var firstCategory = posts.Categories.ElementAt(0);
        var secondCategory = posts.Categories.ElementAt(1);

        dbContext.ChangeTracker.Entries<Category>().Should().NotBeEmpty();
        dbContext.ChangeTracker.Entries<Category>().Should().Contain(x => x.Entity.CategoryId == firstCategory.CategoryId);
        dbContext.ChangeTracker.Entries<Category>().Should().Contain(x => x.Entity.CategoryId == secondCategory.CategoryId);
    }
}