using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about deferred query execution on 
// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/query-execution#deferred-query-execution

public class DeferredExecutionTests
{
    [Fact]
    public async Task ExecutionIsDeferredToCallToList()
    {
        const string querySql = "SELECT [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.ToListAsync();
        commandVerifier.VerifyCalled(querySql);
    }
    
    [Fact]
    public async Task ExecutionIsDeferredToCallToDictionary()
    {
        const string querySql = "SELECT [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.ToDictionaryAsync(b => b.BlogId, b => b.Name);
        commandVerifier.VerifyCalled(querySql);
    }

    [Fact]
    public async Task ExecutionIsDeferredToCallToArray()
    {
        const string querySql = "SELECT [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.ToArrayAsync();
        commandVerifier.VerifyCalled(querySql);
    }

    [Fact]
    public async Task ExecutionIsDeferredToCallFirst()
    {
        const string querySql = "SELECT TOP(1) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.FirstAsync();
        commandVerifier.VerifyCalled(querySql);
    }
    
    [Fact]
    public async Task ExecutionIsDeferredToCallSingle()
    {
        const string querySql = "SELECT TOP(2) [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.SingleAsync();
        commandVerifier.VerifyCalled(querySql);
    }
    
    [Fact]
    public async Task ExecutionIsDeferredToIterateWithLoop()
    {
        const string querySql = "SELECT [b].[BlogId], [b].[Description], [b].[Name] FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);

        var booksEnumerable = query.AsEnumerable();
        commandVerifier.VerifyNotCalled(querySql);

        foreach (var _ in booksEnumerable)
        {
            // Do nothing
        }
        
        commandVerifier.VerifyCalled(querySql);
    }
    
    [Fact]
    public async Task ExecutionIsDeferredToCallCount()
    {
        const string querySql = "SELECT COUNT(*) FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.CountAsync();
        commandVerifier.VerifyCalled(querySql);
    }
    
    [Fact]
    public async Task ExecutionIsDeferredToCallAggregateMethod()
    {
        const string querySql = "SELECT AVG(CAST([b].[BlogId] AS float)) FROM [Blogs] AS [b] WHERE [b].[Name] = N'My Blog'";
        
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        await CreateDatabaseAsync(dbContext);

        var query = dbContext.Blogs
            .Where(b => b.Name == "My Blog");
        commandVerifier.VerifyNotCalled(querySql);
        
        _ = await query.AverageAsync(b => b.BlogId);
        commandVerifier.VerifyCalled(querySql);
    }
    
    private static async Task CreateDatabaseAsync(HandsOnQueriesDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        await SeedDatabaseAsync(dbContext);
    }
    
    private static async Task SeedDatabaseAsync(HandsOnQueriesDbContext dbContext)
    {
        var blog = new Blog
        {
            Name = "My Blog",
            Description = "My Blog Description",
            Posts = new List<Post>
            {
                new()
                {
                    Title = "My First Post",
                    Content = "My First Post Content",
                    PublishedOn = DateTime.UtcNow,
                    Comments = new List<Comment>
                    {
                        new()
                        {
                            Message = "My First Comment",
                            PublishedOn = DateTime.UtcNow
                        },
                        new()
                        {
                            Message = "My Second Comment",
                            PublishedOn = DateTime.UtcNow
                        }
                    }
                }
            }
        };
        
        dbContext.Blogs.Add(blog);
        await dbContext.SaveChangesAsync();
    }
}