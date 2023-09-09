using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HandsOn.Console.EFCore.Tests.Queries.DataAccess.Models.LazyLoaders;

public class Post
{
    private readonly ILazyLoader _lazyLoader;
    
    private ICollection<Comment> _comments;
    
    private Metadata _metadata;
    
    public Post()
    {
    }

    public Post(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
    
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime PublishedOn { get; set; }

    public Metadata Metadata
    {
        get => _lazyLoader.Load(this, ref _metadata);
        set => _metadata = value;
    }

    public ICollection<Comment> Comments
    {
        get => _lazyLoader.Load(this, ref _comments);
        set => _comments = value;
    }
}