namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

public class Comment
{
    public int CommentId { get; set; }

    public string Message { get; set; }
    
    public DateTime PublishedOn { get; set; }
}