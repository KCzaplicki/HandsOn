namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.Proxies;

public class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime PublishedOn { get; set; }
    
    public virtual Metadata Metadata { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }
}