using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess;

public class HandsOnQueriesDbContext : DbContext
{
    private const string ConnectionString =
        "Database=HandsOnQueriesDb;Server=localhost;User Id=SA;Password=3graFooy1KXVQ8kezMyH9RPj;Encrypt=False;";

    private static readonly Func<HandsOnQueriesDbContext, int, IEnumerable<Blog>> GetBlogsMinPostsQuery =
        EF.CompileQuery((HandsOnQueriesDbContext context, int postCount) =>
            context.Blogs.Where(b => b.Posts.Count >= postCount));

    private readonly IDbCommandInterceptor _interceptor;

    public HandsOnQueriesDbContext()
    {
    }

    public HandsOnQueriesDbContext(IDbCommandInterceptor interceptor)
    {
        _interceptor = interceptor;
    }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Metadata> Metadata { get; set; }

    public IEnumerable<Blog> GetBlogsWithMinPosts(int postCount)
    {
        return GetBlogsMinPostsQuery(this, postCount);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(ConnectionString)
            .AddInterceptors(_interceptor);
    }
}