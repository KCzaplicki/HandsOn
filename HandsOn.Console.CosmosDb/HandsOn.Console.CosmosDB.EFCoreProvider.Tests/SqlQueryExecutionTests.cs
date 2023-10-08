using HandsOn.Console.CosmosDb.EFCoreProvider;
using HandsOn.Console.CosmosDb.EFCoreProvider.Models;

namespace HandsOn.Console.CosmosDB.EFCoreProvider.Tests;

public class SqlQueryExecutionTests
{
    private readonly string _connectionString;

    public SqlQueryExecutionTests()
    {
        var configurationProvider = new ConfigurationProvider();
        _connectionString = configurationProvider.GetConnectionString();
    }

    [Fact]
    public async Task FromSqlRawExecutesSqlQueries()
    {
        await using var context = new CosmosDbContext(_connectionString);
        
        var sql = "SELECT * FROM c";
        
        var blog = await context.Blogs
            .FromSqlRaw(sql)
            .FirstAsync();
        
        blog.Should().NotBeNull();
    }

    [Fact]
    public async Task CosmosClientExecutesSqlQueries()
    {
        await using var context = new CosmosDbContext(_connectionString);
        var cosmosClient = context.Database.GetCosmosClient();
        
        var sql = "SELECT * FROM c";
        var container = cosmosClient.GetContainer(context.DatabaseName, "Blogs");
        var iterator = container.GetItemQueryIterator<Blog>(sql);
        var response = await iterator.ReadNextAsync();
        var blog = response.First();
        
        blog.Should().NotBeNull();
    }
}