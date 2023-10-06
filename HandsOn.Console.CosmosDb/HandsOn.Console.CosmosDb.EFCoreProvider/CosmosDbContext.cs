using HandsOn.Console.CosmosDb.EFCoreProvider.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HandsOn.Console.CosmosDb.EFCoreProvider;

public class CosmosDbContext : DbContext
{
    private readonly string _connectionString;

    public readonly string DatabaseName = "HandsOnDb";

    public CosmosDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        bool RequestChargeEventsFilter(EventId eventId, LogLevel _)
        {
            return eventId == CosmosEventId.ExecutedReadNext ||
                   eventId == CosmosEventId.ExecutedCreateItem ||
                   eventId == CosmosEventId.ExecutedReplaceItem ||
                   eventId == CosmosEventId.ExecutedDeleteItem;
        }

        void LogToConsole(EventData eventData)
        {
            var message = eventData switch
            {
                CosmosQueryExecutedEventData queryExecutedEventData =>
                    $"Request charge: {queryExecutedEventData.RequestCharge}, partition key: {queryExecutedEventData.PartitionKey}, query: {queryExecutedEventData.QuerySql}",
                CosmosItemCommandExecutedEventData itemCommandExecutedEventData =>
                    $"Request charge {itemCommandExecutedEventData.RequestCharge}, partition key: {itemCommandExecutedEventData.PartitionKey}, resource id: {itemCommandExecutedEventData.ResourceId}, operation: {itemCommandExecutedEventData.EventIdCode}",
                _ => throw new InvalidOperationException()
            };

            System.Console.WriteLine(message);
        }

        optionsBuilder
            .UseCosmos(_connectionString, DatabaseName)
            .LogTo(RequestChargeEventsFilter, LogToConsole);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .HasPartitionKey(x => x.BlogId)
            .HasNoDiscriminator()
            .ToContainer("Blogs");
        
        modelBuilder
            .Entity<Post>()
            .HasPartitionKey(x => x.BlogId)
            .HasNoDiscriminator()
            .ToContainer("Posts");
    }
}