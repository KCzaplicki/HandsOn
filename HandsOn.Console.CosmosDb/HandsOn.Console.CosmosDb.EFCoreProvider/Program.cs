using HandsOn.Console.CosmosDb.EFCoreProvider;
using HandsOn.Console.CosmosDb.EFCoreProvider.Models;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Console application with CosmosDB database and EF Core provider");

var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
Console.WriteLine($"Application environment: {environmentName}\r\n");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .Build();
var connectionString = configuration["ConnectionStrings:DefaultConnection"];

var dbContext = new CosmosDbContext(connectionString);

await dbContext.Database.EnsureCreatedAsync();
Console.WriteLine($"Database '{dbContext.DatabaseName}' created");

var dataFactory = new DataFactory();
var blogs = Enumerable.Range(0, 5).Select(_ => dataFactory.GenerateBlog()).ToList();

await dbContext.Blogs.AddRangeAsync(blogs);
await dbContext.SaveChangesAsync();
Console.WriteLine("Blog items added\r\n");

foreach (var blog in blogs)
{
    var posts = Enumerable.Range(0, 10).Select(_ => dataFactory.GeneratePost(blog.BlogId)).ToList();
    dbContext.Posts.AddRange(posts);
}

await dbContext.SaveChangesAsync();
Console.WriteLine("Post items added");

