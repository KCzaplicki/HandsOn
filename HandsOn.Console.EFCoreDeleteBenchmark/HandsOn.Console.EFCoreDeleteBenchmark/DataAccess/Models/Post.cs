namespace HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Models;

public class Post
{
    public int Id { get; set; }
    
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int BlogId { get; set; }
}