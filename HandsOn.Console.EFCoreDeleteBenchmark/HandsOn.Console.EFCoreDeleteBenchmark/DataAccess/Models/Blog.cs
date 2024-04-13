namespace HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Models;

public class Blog
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public List<Post> Posts { get; set; } = null!;
}