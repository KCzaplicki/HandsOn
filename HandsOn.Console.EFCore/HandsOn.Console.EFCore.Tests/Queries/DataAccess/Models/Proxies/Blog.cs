namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.Proxies;

public class Blog
{
    public int BlogId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Proxies.Post> Posts { get; set; }
}