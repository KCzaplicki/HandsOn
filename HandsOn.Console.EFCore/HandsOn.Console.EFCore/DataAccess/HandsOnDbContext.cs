namespace HandsOn.Console.EFCore.DataAccess;

public class HandsOnDbContext : DbContext
{
    private const string ConnectionString = "Database=HandsOnDb;Server=localhost;User Id=SA;Password=3graFooy1KXVQ8kezMyH9RPj;Encrypt=False;";

    public DbSet<Book> Books { get; set; }
    
    public DbSet<Author> Authors { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany();

        modelBuilder
            .Entity<Book>()
            .HasMany(b => b.Categories)
            .WithMany();
    }
}