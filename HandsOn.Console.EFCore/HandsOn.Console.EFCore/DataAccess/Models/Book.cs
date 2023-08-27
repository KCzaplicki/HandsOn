namespace HandsOn.Console.EFCore.DataAccess.Models;

public class Book
{
    public int BookId { get; set; }
    
    public string Title { get; set; }
    
    public string Isbn { get; set; }

    public string Description { get; set; }
    
    public int Pages { get; set; }
    
    public DateTime PublishedOn { get; set; }
    
    public ICollection<Category> Categories { get; set; }

    public ICollection<Author> Authors { get; set; }
    
    public ICollection<Review> Reviews { get; set; }
}