using HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Proxies = HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.Proxies;

namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess;

public class HandsOnProxiesDbContext : DbContext
{
    private const string ConnectionString =
        "Database=HandsOnQueriesDb;Server=localhost;User Id=SA;Password=3graFooy1KXVQ8kezMyH9RPj;Encrypt=False;";

    private readonly IDbCommandInterceptor _interceptor;

    public HandsOnProxiesDbContext()
    {
    }
    
    public HandsOnProxiesDbContext(IDbCommandInterceptor interceptor)
    {
        _interceptor = interceptor;
    }
    
    public DbSet<Proxies.Blog> Blogs { get; set; }

    public DbSet<Proxies.Post> Posts { get; set; }

    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Metadata> Metadata { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlServer(ConnectionString)
            .AddInterceptors(_interceptor);
    }
}