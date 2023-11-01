using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public abstract class BaseTest : IAsyncDisposable
{
    protected readonly string QueueName;
    
    protected ServiceBusClient Client;
    
    protected BaseTest()
    {
        var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .Build();
        
        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        QueueName = configuration["Queues:TestQueue"];
        
        Client = new ServiceBusClient(connectionString);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await Client.DisposeAsync();
    }
}