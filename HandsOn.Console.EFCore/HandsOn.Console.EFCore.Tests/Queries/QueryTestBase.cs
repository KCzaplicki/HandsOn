using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

namespace HandsOn.Console.EFCore.Tests.Queries;

public abstract class QueryTestBase
{
    protected async Task CreateDatabaseAsync(HandsOnQueriesDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        await SeedDatabaseAsync(dbContext);
    }
    
    protected virtual async Task SeedDatabaseAsync(HandsOnQueriesDbContext dbContext)
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