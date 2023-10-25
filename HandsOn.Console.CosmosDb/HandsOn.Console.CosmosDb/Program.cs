using HandsOn.Console.CosmosDb;
using HandsOn.Console.CosmosDb.Models;

Console.WriteLine("Console application with CosmosDB database");

var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
Console.WriteLine($"Application environment: {environmentName}\r\n");

var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", false, true)
                 .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                 .Build();

var connectionString = configuration["ConnectionStrings:DefaultConnection"];
var cosmosClient = new CosmosClient(connectionString);

var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("HandsOn");
var database = databaseResponse.Database;
Console.WriteLine($"Database {databaseResponse.Database.Id} created, request charge: {databaseResponse.RequestCharge}");

var containerResponse = await database.CreateContainerIfNotExistsAsync("Blogs", "/blogId");
var container = containerResponse.Container;
Console.WriteLine($"Container {containerResponse.Container.Id} created, request charge: {containerResponse.RequestCharge}\r\n");

var dataFactory = new DataFactory();
 var blogs = Enumerable.Range(0, 5).Select(i => dataFactory.GenerateBlog()).ToList();

 var requestChargeSum = 0.0;

 foreach (var blog in blogs)
 {
     var createItemResponse = await container.CreateItemAsync(blog, new PartitionKey(blog.blogId));
     Console.WriteLine($"Item '{createItemResponse.Resource.blogId}' created, request charge: {createItemResponse.RequestCharge}");
     requestChargeSum += createItemResponse.RequestCharge;
 }

 Console.WriteLine($"{blogs.Count} items created, Total request charge: {requestChargeSum:F}\r\n");

var blogsQuery = $@"SELECT * FROM c WHERE c.type = ""{nameof(Blog)}""";
var blogsIterator = container.GetItemQueryIterator<Blog>(blogsQuery);
var cosmosBlogs = new List<Blog>();
while (blogsIterator.HasMoreResults)
{
    var feedResponse = await blogsIterator.ReadNextAsync();
    Console.WriteLine($"Items retrieved, request charge: {feedResponse.RequestCharge}");

    foreach (var blog in feedResponse)
    {
        Console.WriteLine($"Item: {blog}");
    }
    
    cosmosBlogs.AddRange(feedResponse);
}

var blogsCountQuery = $@"SELECT VALUE COUNT(1) FROM c WHERE c.type = ""{nameof(Blog)}""";
var blogsCountResponse = await container.GetItemQueryIterator<int>(blogsCountQuery).ReadNextAsync();
var blogsCount = blogsCountResponse.Resource.First();
Console.WriteLine($"\r\nTotal blogs: {blogsCount}, request charge: {blogsCountResponse.RequestCharge}\r\n");

Console.WriteLine("Query filter by createdAt");
var blogsCreatedAtFilterQuery = $@"SELECT * FROM c WHERE c.type = ""{nameof(Blog)}"" AND c.createdAt > ""2021-01-01T00:00:00Z""";
var blogsCreatedAtFilterResponse = container.GetItemQueryIterator<Blog>(blogsCreatedAtFilterQuery);
while (blogsCreatedAtFilterResponse.HasMoreResults)
{
    var feedResponse = await blogsCreatedAtFilterResponse.ReadNextAsync();
    Console.WriteLine($"Items retrieved, request charge: {feedResponse.RequestCharge}");

    foreach (var blog in feedResponse.Resource)
    {
        Console.WriteLine($"Item: {blog}");
    }
}

var updatedBlog = cosmosBlogs[0];
updatedBlog.description = "Updated description";
var updateResponse = await container.ReplaceItemAsync(updatedBlog, updatedBlog.id, new PartitionKey(updatedBlog.blogId));
Console.WriteLine($"\r\nItem {updateResponse.Resource.blogId} updated, request charge: {updateResponse.RequestCharge}");

updatedBlog.name = "Updated name";
var patchResponse = await container.PatchItemAsync<Blog>(updatedBlog.id, new PartitionKey(updatedBlog.blogId), new PatchOperation[]
{
    PatchOperation.Replace("/name", updatedBlog.name)
});
Console.WriteLine($"Item {patchResponse.Resource.blogId} patched, request charge: {patchResponse.RequestCharge}");

var deleteBlog = cosmosBlogs[0];
var deleteResponse = await container.DeleteItemAsync<Blog>(deleteBlog.id, new PartitionKey(deleteBlog.blogId));
Console.WriteLine($"Item {deleteBlog.blogId} deleted, request charge: {deleteResponse.RequestCharge}");