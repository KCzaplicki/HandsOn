using HandsOn.Console.CosmosDb.EFCoreProvider;
using ConfigurationProvider = HandsOn.Console.CosmosDb.EFCoreProvider.ConfigurationProvider;

Console.WriteLine("Console application with CosmosDB database and EF Core provider");

var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
Console.WriteLine($"Application environment: {environmentName}\r\n");

var configurationProvider = new ConfigurationProvider();
await using var dbContext = new CosmosDbContext(configurationProvider.GetConnectionString());

var dbCreated = await dbContext.Database.EnsureCreatedAsync();
Console.WriteLine($"Database '{dbContext.DatabaseName}' {(dbCreated ? "created" : "already exists")}");

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
Console.WriteLine("Post items added\r\n");

var dbBlogs = await dbContext.Blogs.ToListAsync();
Console.WriteLine($"Total blogs: {dbBlogs.Count}");
Console.WriteLine("Blogs: \r\n");

foreach (var blog in dbBlogs)
{
    Console.WriteLine(blog);
}

var blogPosts = await dbContext.Posts
    .WithPartitionKey(dbBlogs[0].BlogId)
    .ToListAsync();
    
Console.WriteLine($"\r\nTotal posts for blog '{dbBlogs[0].BlogId}': {blogPosts.Count}");
Console.WriteLine("Posts: \r\n");

foreach (var post in blogPosts)
{
    Console.WriteLine(post);
}

var updatedBlog = dbBlogs[0];
updatedBlog.Name = "Updated name";
dbContext.Blogs.Update(updatedBlog);
await dbContext.SaveChangesAsync();
Console.WriteLine($"\r\nBlog '{updatedBlog.BlogId}' updated\r\n");

var deletedBlog = dbBlogs[1];
dbContext.Blogs.Remove(deletedBlog);
await dbContext.SaveChangesAsync();
Console.WriteLine($"Blog '{deletedBlog.BlogId}' deleted");

