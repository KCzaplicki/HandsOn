using Bogus;
using HandsOn.Console.CosmosDb.Models;

namespace HandsOn.Console.CosmosDb;

public class DataFactory
{
    private Faker<Blog> _blogFaker;

    public DataFactory()
    {
        SetUpBlogFaker();
    }

    public Blog GenerateBlog() => _blogFaker.Generate();

    private void SetUpBlogFaker()
    {
        _blogFaker = new Faker<Blog>()
            .RuleFor(b => b.id, f => f.Random.Guid().ToString())
            .RuleFor(b => b.name, f => $"{f.Name.FullName()} Blog")
            .RuleFor(b => b.blogId, (_, b) => b.name.Replace(" ", "-").ToLower())
            .RuleFor(b => b.description, f => f.Lorem.Sentence())
            .RuleFor(b => b.createdAt, f => f.Date.Past())
            .RuleFor(b => b.updatedAt, f => f.Date.Recent())
            .RuleFor(b => b.contact, f => new Contact
            {
                email = f.Internet.Email(),
                phone = f.Phone.PhoneNumber()
            });
    }
}