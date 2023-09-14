using HandsOn.Console.EFCore.Tests.Queries.DataAccess;
using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

namespace HandsOn.Console.EFCore.Tests.Queries;

public abstract class QueryTestBase
{
    protected async Task CreateDatabaseAsync(HandsOnQueriesDbContext? dbContext = null)
    {
        dbContext ??= new HandsOnQueriesDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await SeedDatabaseAsync(dbContext);
    }

    protected virtual async Task SeedDatabaseAsync(HandsOnQueriesDbContext dbContext)
    {
        var categories = new Category[]
        {
            new()
            {
                Name = "My First Category"
            },
            new()
            {
                Name = "My Second Category"
            }
        };
        
        var blogs = new Blog[]
        {
            new()
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
                        Metadata = new Metadata
                        {
                            SeoTitle = "My First Post Seo Title",
                            SeoDescription = "My First Post Seo Description",
                            SeoWords = "Post, Blog, First"
                        },
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
                        },
                        Categories = categories
                    },
                    new ()
                    {
                        Title = "My Second Post",
                        Content = "My Second Post Content",
                        PublishedOn = DateTime.UtcNow,
                        Metadata = new Metadata
                        {
                            SeoTitle = "My Second Post Seo Title",
                            SeoDescription = "My Second Post Seo Description",
                            SeoWords = "Post, Blog, Second"
                        },
                        Categories = categories,
                        Comments = new List<Comment>
                        {
                            new()
                            {
                                Message = "My First Comment",
                                PublishedOn = DateTime.UtcNow
                            }
                        }
                    }
                }
            }
        };

        await dbContext.Blogs.AddRangeAsync(blogs);
        await dbContext.SaveChangesAsync();
    }
}