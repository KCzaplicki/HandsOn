using System.Diagnostics;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Xunit.Abstractions;

namespace HandsOn.Console.EFCore.Tests.Queries;

// More about compiled query on
// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/compiled-queries-linq-to-entities

public class CompiledQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CompiledQueryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task CompiledQueriesAreFasterThanRegularQueries()
    {
        const int minPosts = 5, attempts = 5;
        
        var regularQueryContext = new HandsOnQueriesDbContext();
        await CreateDatabaseAsync(regularQueryContext);

        var stopWatch = new Stopwatch();
        var queryTimes = new List<TimeSpan>();
        
        for (var i = 0; i < attempts; i++)
        {
            stopWatch.Restart();
            _ = regularQueryContext.Blogs.Where(b => b.Posts.Count > minPosts).ToList();
            stopWatch.Stop();
            queryTimes.Add(stopWatch.Elapsed);
            _testOutputHelper.WriteLine("Regular query elapsed time: {0}", stopWatch.Elapsed);
        }

        var avgRegularQueryElapsedTime = TimeSpan.FromMilliseconds(queryTimes.Average(t => t.TotalMilliseconds));
        _testOutputHelper.WriteLine("Average regular query elapsed time: {0}", avgRegularQueryElapsedTime);

        var compiledQueryContext = new HandsOnQueriesDbContext();
        await CreateDatabaseAsync(compiledQueryContext);

        queryTimes = new List<TimeSpan>();
        for (var i = 0; i < attempts; i++)
        {
            stopWatch.Restart();
            _ = compiledQueryContext.GetBlogsWithMinPosts(minPosts).ToList();
            stopWatch.Stop();
            queryTimes.Add(stopWatch.Elapsed);
            _testOutputHelper.WriteLine("Compiled query elapsed time: {0}", stopWatch.Elapsed);
        }
        
        var avgCompiledQueryElapsedTime = TimeSpan.FromMilliseconds(queryTimes.Average(t => t.TotalMilliseconds));
        _testOutputHelper.WriteLine("Average compiled query elapsed time: {0}", avgCompiledQueryElapsedTime);

        avgCompiledQueryElapsedTime.Should().BeLessThan(avgRegularQueryElapsedTime);
    }

    protected override async Task SeedDatabaseAsync(HandsOnQueriesDbContext dbContext)
    {
        var random = new Random();
        
        for (var i = 0; i < 10_000; i++)
        {
            var posts = new List<Post>();
            for (var j = 0; j < random.Next(20); j++)
            {
                posts.Add(new Post
                {
                    Title = $"Post no {j}",
                    Content = "Content",
                    PublishedOn = DateTime.UtcNow
                });
            }
            
            var blog = new Blog
            {
                Name = $"My Blog {i}",
                Description = "My Blog Description",
                Posts = posts
            };
            
            await dbContext.Blogs.AddAsync(blog);
        }
        
        await dbContext.SaveChangesAsync();
    }
}