using Bogus;
using HandsOn.Console.CosmosDb.EFCoreProvider.Models;

namespace HandsOn.Console.CosmosDb.EFCoreProvider;

public class DataFactory
{
    private Faker<Blog> _blogFaker;
    private Faker<Post> _postFaker;

    public DataFactory()
    {
        SetUpBlogFaker();
        SetUpPostFaker();
    }

    public Blog GenerateBlog()
    {
        return _blogFaker.Generate();
    }

    public Post GeneratePost(string blogId)
    {
        var post = _postFaker.Generate();
        post.BlogId = blogId;

        return post;
    }

    private void SetUpPostFaker()
    {
        _postFaker = new Faker<Post>()
            .RuleFor(p => p.Id, f => f.Random.Guid().ToString())
            .RuleFor(p => p.Title, f => f.Lorem.Sentence())
            .RuleFor(p => p.Content, f => f.Lorem.Paragraphs())
            .RuleFor(p => p.PublishedOn, f => f.Date.Past());
    }

    private void SetUpBlogFaker()
    {
        _blogFaker = new Faker<Blog>()
            .RuleFor(b => b.Id, f => f.Random.Guid().ToString())
            .RuleFor(b => b.Name, f => $"{f.Name.FullName()} Blog")
            .RuleFor(b => b.BlogId, (_, b) => b.Name.Replace(" ", "-").ToLower())
            .RuleFor(b => b.Description, f => f.Lorem.Sentence())
            .RuleFor(b => b.CreatedAt, f => f.Date.Past())
            .RuleFor(b => b.UpdatedAt, f => f.Date.Recent())
            .RuleFor(b => b.Contact, f => new Contact
            {
                Email = f.Internet.Email(),
                Phone = f.Phone.PhoneNumber()
            });
    }
}