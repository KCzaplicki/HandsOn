using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about client vs server evaluation on
// https://learn.microsoft.com/en-us/ef/core/querying/client-eval
// functions mappings
// https://github.com/dotnet/EntityFramework.Docs/blob/main/entity-framework/core/providers/sql-server/functions.md

public class ClientVsServerEvaluationTests : QueryTestBase
{
    [Fact]
    public async Task ClientEvaluationIsUsedForCustomMethods()
    {
        var sqlQuery = "SELECT [b].[BlogId], [b].[Name] FROM [Blogs] AS [b]";
        
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);

        var blogs = await dbContext.Blogs
            .Select(x => new
            {
                x.BlogId,
                x.Name,
                Keywords = x.Name.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            })
            .ToListAsync();

        blogs.Should().NotBeEmpty();
        foreach (var blog in blogs) blog.Keywords.Should().NotBeEmpty();

        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task ServerEvaluationIsUsedForMethodsSupportedByDatabase()
    {
        var sqlQuery = "SELECT [b].[BlogId], [b].[Name], UPPER([b].[Name]) AS [UppercaseName] FROM [Blogs] AS [b]";
        
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        
        var blogs = await dbContext.Blogs
            .Select(x => new
            {
                x.BlogId,
                x.Name,
                UppercaseName = x.Name.ToUpper()
            })
            .ToListAsync();
        
        blogs.Should().NotBeEmpty();
        foreach (var blog in blogs) blog.UppercaseName.Should().NotBeEmpty();
        
        commandVerifier.VerifyCalled(sqlQuery);
    }

    [Fact]
    public async Task ServerEvaluationIsUsedForEFFunctions()
    {
        var sqlQuery = @"SELECT [b].[BlogId], [b].[Name], 
                         CASE WHEN [b].[Name] LIKE N'%seo%' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [HasSeoWords]
                         FROM [Blogs] AS [b]";
        
        await CreateDatabaseAsync();
        var commandVerifier = new CommandVerifier();
        var dbContext = new HandsOnQueriesDbContext(commandVerifier);
        
        var blogs = await dbContext.Blogs
            .Select(x => new
            {
                x.BlogId,
                x.Name,
                HasSeoWords = EF.Functions.Like(x.Name, "%seo%")
            })
            .ToListAsync();
        
        blogs.Should().NotBeEmpty();
        
        commandVerifier.VerifyCalled(sqlQuery);
    }
}