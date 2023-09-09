using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.LazyLoaders;

public class Blog
{
    private readonly ILazyLoader _lazyLoader;
    
    private ICollection<Post> _posts;

    public Blog()
    {
    }
    
    public Blog(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public int BlogId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Post> Posts
    {
        get => _lazyLoader.Load(this, ref _posts);
        set => _posts = value;
    }
}