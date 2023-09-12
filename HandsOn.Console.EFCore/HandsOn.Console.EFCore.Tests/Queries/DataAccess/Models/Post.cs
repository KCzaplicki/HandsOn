namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

public class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime PublishedOn { get; set; }
    
    public Metadata Metadata { get; set; }
    
    public ICollection<Category> Categories { get; set; }

    public ICollection<Comment> Comments { get; set; }
}