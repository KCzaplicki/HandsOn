using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about split queries on
// https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries

public class QuerySplitTests : QueryTestBase
{
    [Fact]
    public async Task AsSplitQueryCreatesSeparateQueryForNavigationalProperties()
    {
        var blogsSqlQuery = "SELECT [b].[BlogId] ,[b].[Description] ,[b].[Name] FROM [Blogs] AS [b] ORDER BY [b].[BlogId]";
        var postsSqlQuery = @"SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title], [b].[BlogId] FROM [Blogs] AS [b] 
                              INNER JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId] ORDER BY [b].[BlogId]";
        
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blogs = await dbContext.Blogs
            .Include(x => x.Posts)
            .AsSplitQuery()
            .ToListAsync();

        blogs.Should().NotBeEmpty();
        
        commandVerifier.VerifyCalled(blogsSqlQuery);
        commandVerifier.VerifyCalled(postsSqlQuery);
    }

    [Fact]
    public async Task AsSplitQueryCreatesSeparateQueryForSingleEntityWithNavigationalProperties()
    {
        var blogSqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] ORDER BY [b].[BlogId]";
        var postsSqlQuery = @"SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title], [t].[BlogId]
                              FROM (SELECT TOP(1) [b].[BlogId] FROM [Blogs] AS [b]) AS [t] 
                              INNER JOIN [Posts] AS [p] ON [t].[BlogId] = [p].[BlogId]
                              ORDER BY [t].[BlogId]";
            
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        
        var blog = await dbContext.Blogs
            .Include(x => x.Posts)
            .AsSplitQuery()
            .FirstAsync();

        blog.Should().NotBeNull();
        
        commandVerifier.VerifyCalled(blogSqlQuery);
        commandVerifier.VerifyCalled(postsSqlQuery);
    }

    [Fact]
    public async Task AsSplitQueryCreatesSeparateQueryForSelectLoadingWithProperties()
    {
        var blogSqlQuery = "SELECT TOP(1) [b].[BlogId], [b].[Name] FROM [Blogs] AS [b] ORDER BY [b].[BlogId]";
        var postsSqlQuery = @"SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[PublishedOn], [p].[Title], [t].[BlogId]
                              FROM (SELECT TOP(1) [b].[BlogId] FROM [Blogs] AS [b]) AS [t] 
                              INNER JOIN [Posts] AS [p] ON [t].[BlogId] = [p].[BlogId]
                              ORDER BY [t].[BlogId]";
        
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blog = await dbContext.Blogs
            .Select(x => new
            {
                x.BlogId,
                x.Name,
                x.Posts
            })
            .AsSplitQuery()
            .FirstAsync();
        
        blog.Should().NotBeNull();
        
        commandVerifier.VerifyCalled(blogSqlQuery);
        commandVerifier.VerifyCalled(postsSqlQuery);
    }
}