namespace HandsOn.Console.CosmosDb.EFCoreProvider.Models;

public class Blog
{
    public string BlogId { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Contact Contact { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, BlogId: {BlogId}, Name: {Name}, Description: {Description}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, Contact: {Contact}";
    }
}