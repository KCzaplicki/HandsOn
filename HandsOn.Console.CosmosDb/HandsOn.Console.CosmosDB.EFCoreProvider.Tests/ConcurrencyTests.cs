using HandsOn.Console.CosmosDb.EFCoreProvider;
using HandsOn.Console.CosmosDb.EFCoreProvider.Models;

namespace HandsOn.Console.CosmosDB.EFCoreProvider.Tests;

public class ConcurrencyTests
{
    private readonly string _connectionString;

    public ConcurrencyTests()
    {
        var configurationProvider = new ConfigurationProvider();
        _connectionString = configurationProvider.GetConnectionString();
    }
    
    [Fact]
    public async Task TransactionByDefaultSolveConcurrencyConflictWithLastWriteWins()
    {
        var blogId = await AddBlogItem();
        await using var primaryContext = new CosmosDbContext(_connectionString);
        await using var secondaryContext = new CosmosDbContext(_connectionString);

        var primaryBlog = await primaryContext.Blogs.FindAsync(blogId);
        var secondaryBlog = await secondaryContext.Blogs.FindAsync(blogId);
        
        primaryBlog.Name = "Primary Blog";
        secondaryBlog.Name = "Secondary Blog";

        await primaryContext.SaveChangesAsync();
        await secondaryContext.SaveChangesAsync();
        
        await using var assertContext = new CosmosDbContext(_connectionString);
        var assertBlog = await assertContext.Blogs.FindAsync(blogId);

        assertBlog.Should().NotBeNull();
        assertBlog.Name.Should().Be(secondaryBlog.Name);
    }

    [Fact]
    public async Task TransactionConcurrencyWithETagSolveConflictWithThrowsDbUpdateConcurrencyException()
    {
        var postId = await AddPostItem();
        await using var primaryContext = new CosmosDbContext(_connectionString);
        await using var secondaryContext = new CosmosDbContext(_connectionString);

        var primaryBlog = await primaryContext.Posts.FindAsync(postId);
        var secondaryBlog = await secondaryContext.Posts.FindAsync(postId);
        
        primaryBlog.Title = "Primary Post";
        secondaryBlog.Title = "Secondary Post";
        
        await primaryContext.SaveChangesAsync();
        var secondSaveChanges = async () => await secondaryContext.SaveChangesAsync();

        await secondSaveChanges.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    private async Task<string> AddBlogItem()
    {
        await using var context = new CosmosDbContext(_connectionString);

        var blog = new Blog
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Concurrency blog",
            BlogId = "concurrency-blog",
            Description = "Concurrency blog description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await context.AddAsync(blog);
        await context.SaveChangesAsync();

        return blog.Id;
    }
    
    private async Task<string> AddPostItem()
    {
        await using var context = new CosmosDbContext(_connectionString);

        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            BlogId = "concurrency-blog",
            Title = "Concurrency post",
            Content = "Concurrency post content",
            PublishedOn = DateTime.UtcNow,
        };

        await context.AddAsync(post);
        await context.SaveChangesAsync();

        return post.Id;
    }
}