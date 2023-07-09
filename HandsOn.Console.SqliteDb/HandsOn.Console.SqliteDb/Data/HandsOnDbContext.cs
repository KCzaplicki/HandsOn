using System.Data;

namespace HandsOn.Console.SqliteDb.Data;

public sealed class HandsOnDbContext : DbContext
{
    private const string DbName = $"{nameof(HandsOn)}.db";

    public DbSet<Organization> Organizations { get; set; }
    
    public DbSet<Department> Departments { get; set; }
    
    public DbSet<Employee> Employees { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options) => 
        options.UseSqlite($"Data Source={DbName}");
}