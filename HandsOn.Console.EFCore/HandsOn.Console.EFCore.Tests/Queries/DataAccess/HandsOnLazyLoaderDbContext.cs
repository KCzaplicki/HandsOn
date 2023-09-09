using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using LazyLoaders = HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.LazyLoaders;

namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess;

public class HandsOnLazyLoaderDbContext : DbContext
{
    private const string ConnectionString =
        "Database=HandsOnQueriesDb;Server=localhost;User Id=SA;Password=3graFooy1KXVQ8kezMyH9RPj;Encrypt=False;";

    private readonly IDbCommandInterceptor _interceptor;

    public HandsOnLazyLoaderDbContext()
    {
    }

    public HandsOnLazyLoaderDbContext(IDbCommandInterceptor interceptor)
    {
        _interceptor = interceptor;
    }
    
    public DbSet<LazyLoaders.Blog> Blogs { get; set; }

    public DbSet<LazyLoaders.Post> Posts { get; set; }

    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Metadata> Metadata { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(ConnectionString)
            .AddInterceptors(_interceptor);
    }
}