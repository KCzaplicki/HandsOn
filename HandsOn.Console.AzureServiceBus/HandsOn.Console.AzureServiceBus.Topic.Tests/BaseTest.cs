using Microsoft.Extensions.Configuration;

namespace HandsOn.Console.AzureServiceBus.Topic.Tests;

public abstract class BaseTest : IAsyncDisposable
{
    protected readonly string TopicName;
    protected readonly string CorrelationFilterSubscriptionName;
    protected readonly string SqlFilterSubscriptionName;
    
    protected readonly ServiceBusClient Client;

    protected BaseTest()
    {
        var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .Build();
        
        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        TopicName = configuration["Topics:BaseTopic"];
        CorrelationFilterSubscriptionName = configuration["Subscriptions:CorrelationFilterSubscription"];
        SqlFilterSubscriptionName = configuration["Subscriptions:SqlFilterSubscription"];
        
        Client = new ServiceBusClient(connectionString);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await Client.DisposeAsync();
    }
}