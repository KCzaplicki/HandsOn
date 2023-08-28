namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

public class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime PublishedOn { get; set; }

    public ICollection<Comment> Comments { get; set; }
}