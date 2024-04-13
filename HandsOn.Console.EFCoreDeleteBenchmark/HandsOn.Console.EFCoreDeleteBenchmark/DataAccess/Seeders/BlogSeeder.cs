using Bogus;
using HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Models;

namespace HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Seeders;

public static class BlogSeeder
{
    private static readonly Faker<Blog> BlogFaker = new Faker<Blog>()
        .Ignore(b => b.Id)
        .RuleFor(b => b.Name, f => f.Lorem.Word())
        .RuleFor(b => b.Url, f => f.Internet.Url())
        .RuleFor(b => b.Posts, f => PostFaker.Generate(5));
    
    private static readonly Faker<Post> PostFaker = new Faker<Post>()
        .Ignore(p => p.Id)
        .Ignore(p => p.BlogId)
        .RuleFor(p => p.Title, f => f.Lorem.Sentence())
        .RuleFor(p => p.Content, f => f.Lorem.Paragraph());
    
    public static async Task Seed(int count)
    {
        var blogs = BlogFaker.Generate(count);
        
        await using var context = new SampleDbContext();
        await context.Blogs.AddRangeAsync(blogs);
        await context.SaveChangesAsync();
    }
}