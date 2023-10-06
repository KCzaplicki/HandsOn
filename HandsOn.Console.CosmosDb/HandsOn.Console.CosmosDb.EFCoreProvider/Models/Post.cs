namespace HandsOn.Console.CosmosDb.EFCoreProvider.Models;

public class Post
{
    public string BlogId { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, BlogId: {BlogId}, Title: {Title}, Content: {Content}, PublishedOn: {PublishedOn}";
    }
}