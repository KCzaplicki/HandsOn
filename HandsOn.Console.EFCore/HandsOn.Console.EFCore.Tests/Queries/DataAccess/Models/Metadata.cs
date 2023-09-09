namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models;

public class Metadata
{
    public int MetadataId { get; set; }
    
    public string SeoTitle { get; set; }
    
    public string SeoDescription { get; set; }
    
    public string SeoWords { get; set; }
    
    public int PostId { get; set; }
}