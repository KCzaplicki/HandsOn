using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about loading strategies on
// https://learn.microsoft.com/en-us/ef/core/querying/related-data/

public class LoadingStrategiesTests : QueryTestBase
{
    [Fact]
    public async Task LoadingReturnsCachedEntitiesWithNavigationalProperties()
    {
        var sqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]";

        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var blog = await dbContext.Blogs.FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().NotBeNull();

        foreach (var post in blog.Posts) post.Comments.Should().NotBeNull();

        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task EagerLoadingIncludeLoadsNavigationalProperties()
    {
        var sqlQuery =
            @"SELECT [t].[BlogId], [t].[Description], [t].[Name], [t0].[PostId], [t0].[BlogId], [t0].[Content], [t0].[PublishedOn], [t0].[Title], [t0].[CommentId], [t0].[Message], [t0].[PostId0], [t0].[PublishedOn0]
              FROM (SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]) AS [t]
              LEFT JOIN (
                  SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title], [c].[CommentId], [c].[Message], [c].[PostId] AS [PostId0], [c].[PublishedOn] AS [PublishedOn0] FROM [Posts] AS [p]
                  LEFT JOIN [Comments] AS [c] ON [p].[PostId] = [c].[PostId]
              ) AS [t0] ON [t].[BlogId] = [t0].[BlogId]
              ORDER BY [t].[BlogId], [t0].[PostId]";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs
            .Include(b => b.Posts)
            .ThenInclude(p => p.Comments)
            .FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().NotBeNull();

        foreach (var post in blog.Posts) post.Comments.Should().NotBeNull();

        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task EagerLoadingSetNavigationalPropertiesToNullByDefault()
    {
        var sqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs.FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().BeNull();
        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task ExplicitLoadingLoadsNavigationalPropertiesCollection()
    {
        var selectBlogSqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]";
        var loadPostsSqlQuery =
            "SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title] FROM [Posts] AS [p] WHERE [p].[BlogId] = @__p_0";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs.FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().BeNull();

        commandVerifier.VerifyCalled(selectBlogSqlQuery);

        await dbContext.Entry(blog)
            .Collection(x => x.Posts)
            .LoadAsync();

        blog.Posts.Should().NotBeNull();

        commandVerifier.VerifyCalled(loadPostsSqlQuery);
    }

    [Fact]
    public async Task ExplicitLoadingLoadsNavigationalPropertiesReference()
    {
        var selectPostSqlQuery =
            "SELECT TOP(1) [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title] FROM [Posts] AS [p]";
        var loadMetadataSqlQuery =
            "SELECT [m].[MetadataId], [m].[PostId], [m].[SeoDescription], [m].[SeoTitle], [m].[SeoWords] FROM [Metadata] AS [m] WHERE [m].[PostId] = @__p_0";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var post = await dbContext.Posts.FirstAsync();

        post.Should().NotBeNull();
        post.Metadata.Should().BeNull();

        commandVerifier.VerifyCalled(selectPostSqlQuery);

        await dbContext.Entry(post)
            .Reference(x => x.Metadata)
            .LoadAsync();

        post.Metadata.Should().NotBeNull();

        commandVerifier.VerifyCalled(loadMetadataSqlQuery);
    }

    [Fact]
    public async Task SelectLoadingMapsToAnonymousType()
    {
        var sqlQuery = @"SELECT [t].[BlogId], [t].[Name], [t].[Description], [p].[PostId], [p].[Title] 
                         FROM (SELECT TOP(1) [b].[BlogId], [b].[Name], [b].[Description] FROM [Blogs] AS [b]) AS [t] 
                         LEFT JOIN [Posts] AS [p] ON [t].[BlogId] = [p].[BlogId] ORDER BY [t].[BlogId]";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs
            .Select(x => new
            {
                x.BlogId,
                x.Name,
                x.Description,
                Posts = x.Posts.Select(p => new
                {
                    p.PostId,
                    p.Title
                })
            })
            .FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().NotBeNull();

        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task SelectLoadingMapsToDto()
    {
        var sqlQuery = @"SELECT [t].[BlogId], [t].[Name], [t].[Description], [p].[PostId], [p].[Title] 
                         FROM (SELECT TOP(1) [b].[BlogId], [b].[Name], [b].[Description] FROM [Blogs] AS [b]) AS [t] 
                         LEFT JOIN [Posts] AS [p] ON [t].[BlogId] = [p].[BlogId] ORDER BY [t].[BlogId]";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs
            .Select(x => new BlogDto
            {
                BlogId = x.BlogId,
                Name = x.Name,
                Description = x.Description,
                Posts = x.Posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title
                }).ToList()
            }).FirstAsync();

        blog.Should().NotBeNull();
        blog.Posts.Should().NotBeNull();

        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task LazyLoadingProxiesLoadsNavigationPropertiesOnCall()
    {
        var selectBlogSqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]";
        var loadPostsSqlQuery =
            "SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title] FROM [Posts] AS [p] WHERE [p].[BlogId] = @__p_0";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnProxiesDbContext(commandVerifier);

        var blog = await dbContext.Blogs.FirstAsync();

        blog.Should().NotBeNull();

        commandVerifier.VerifyCalled(selectBlogSqlQuery);

        _ = blog.Posts.ToList();

        blog.Posts.Should().NotBeNull();

        commandVerifier.VerifyCalled(loadPostsSqlQuery);
    }

    [Fact]
    public async Task LazyLoadingLazyLoaderLoadsNavigationalPropertiesOnCall()
    {
        var selectBlogSqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b]";
        var loadPostsSqlQuery =
            "SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title] FROM [Posts] AS [p] WHERE [p].[BlogId] = @__p_0";
        var loadMetadataSqlQuery =
            "SELECT [m].[MetadataId], [m].[PostId], [m].[SeoDescription], [m].[SeoTitle], [m].[SeoWords] FROM [Metadata] AS [m] WHERE [m].[PostId] = @__p_0";

        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnLazyLoaderDbContext(commandVerifier);

        var blog = await dbContext.Blogs.FirstAsync();

        blog.Should().NotBeNull();

        commandVerifier.VerifyCalled(selectBlogSqlQuery);

        _ = blog.Posts.ToList();

        blog.Posts.Should().NotBeNull();

        commandVerifier.VerifyCalled(loadPostsSqlQuery);

        var metadata = blog.Posts.First().Metadata;

        metadata.Should().NotBeNull();

        commandVerifier.VerifyCalled(loadMetadataSqlQuery);
    }
}