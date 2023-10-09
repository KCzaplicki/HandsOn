using HandsOn.Console.CosmosDb.EFCoreProvider;
using HandsOn.Console.CosmosDb.EFCoreProvider.Models;
using Microsoft.Azure.Cosmos;

namespace HandsOn.Console.CosmosDB.EFCoreProvider.Tests;

public class ChangeFeedTests
{
    private readonly string _connectionString;
    private TaskCompletionSource<Blog>? _changeFeedProcessorTcs;

    public ChangeFeedTests()
    {
        var configurationProvider = new ConfigurationProvider();
        _connectionString = configurationProvider.GetConnectionString();
    }

    [Fact]
    public async Task ChangeFeedProcessorReturnsUpdatesOfContainerItems()
    {
        await using var context = new CosmosDbContext(_connectionString);
        var blog = await AddBlogItem(context);
        
        var cosmosClient = context.Database.GetCosmosClient();
        var database = cosmosClient.GetDatabase(context.DatabaseName);
        await database.CreateContainerIfNotExistsAsync("Blogs-lease", "/id");
        var leaseContainer = database.GetContainer("Blogs-lease");
        var changeFeedProcessor = cosmosClient.GetContainer(context.DatabaseName, "Blogs")
            .GetChangeFeedProcessorBuilder<Blog>(processorName: "changeFeedProcessor", onChangesDelegate: HandleBlogChangesAsync)
            .WithInstanceName("changeFeedProcessorInstance")
            .WithLeaseContainer(leaseContainer)
            .Build();

        _changeFeedProcessorTcs = new TaskCompletionSource<Blog>();
        await changeFeedProcessor.StartAsync();
        
        blog.Name = "Updated blog name";
        blog.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        var blogFromChangeFeed = await _changeFeedProcessorTcs.Task;
        
        await changeFeedProcessor.StopAsync();
        
        blogFromChangeFeed.Name.Should().Be(blog.Name);
    }

    [Fact]
    public async Task ChangeFeedPullModelReturnsUpdatesOfContainerItems()
    {
        await using var context = new CosmosDbContext(_connectionString);
        var blog = await AddBlogItem(context);
        
        var cosmosClient = context.Database.GetCosmosClient();
        var container = cosmosClient.GetContainer(context.DatabaseName, "Blogs");
        var feedIterator = container.GetChangeFeedIterator<Blog>(ChangeFeedStartFrom.Now(), ChangeFeedMode.Incremental);
        _ = await feedIterator.ReadNextAsync();
        
        blog.Name = "Updated blog name";
        blog.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        var blogChangeFeedResponse = await feedIterator.ReadNextAsync();
        var blogFromChangeFeed = blogChangeFeedResponse.First(b => b.Id == blog.Id);
        
        blogFromChangeFeed.Name.Should().Be(blog.Name);
    }
    
    [Fact]
    public async Task ChangeFeedPullModelReturnsUpdatesOfContainerItemWithPartitionKey()
    {
        await using var context = new CosmosDbContext(_connectionString);
        var blog = await AddBlogItem(context);
        
        var cosmosClient = context.Database.GetCosmosClient();
        var container = cosmosClient.GetContainer(context.DatabaseName, "Blogs");
        var feedIterator = container.GetChangeFeedIterator<Blog>(ChangeFeedStartFrom.Beginning(FeedRange.FromPartitionKey(new PartitionKey(blog.BlogId))), ChangeFeedMode.Incremental);
        
        var blogChangeFeedResponse = await feedIterator.ReadNextAsync();
        var blogFromChangeFeed = blogChangeFeedResponse.First(b => b.Id == blog.Id);

        blogFromChangeFeed.Name.Should().Be(blog.Name);
        
        blog.Name = "Updated blog name";
        blog.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        blogChangeFeedResponse = await feedIterator.ReadNextAsync();
        blogFromChangeFeed = blogChangeFeedResponse.First(b => b.Id == blog.Id);
        
        blogFromChangeFeed.Name.Should().Be(blog.Name);
    }

    private async Task HandleBlogChangesAsync(IReadOnlyCollection<Blog> changes, CancellationToken cancellationtoken)
    {
        _changeFeedProcessorTcs?.SetResult(changes.First());
    }

    private async Task<Blog> AddBlogItem(CosmosDbContext context)
    {
        var id = Guid.NewGuid().ToString();
        var blog = new Blog
        {
            Id = id,
            Name = "Change feed blog",
            BlogId = $"change-feed-blog-{id}",
            Description = "Change feed blog description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await context.AddAsync(blog);
        await context.SaveChangesAsync();

        return blog;
    }
}