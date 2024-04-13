using HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCoreDeleteBenchmark.DataAccess;

public class SampleDbContext : DbContext
{
    private const string ConnectionString =
        "Server=localhost;Database=SampleDb;User Id=sa;Password=Password123;TrustServerCertificate=True;";
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
    
    public DbSet<Blog> Blogs { get; set; }
    
    public DbSet<Post> Posts { get; set; }
}