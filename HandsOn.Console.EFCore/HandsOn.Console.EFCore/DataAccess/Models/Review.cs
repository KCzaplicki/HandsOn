namespace HandsOn.Console.EFCore.DataAccess.Models;

public class Review
{
    public int ReviewId { get; set; }

    public int Rating { get; set; }
    
    public int BookId { get; set; }
}