using Microsoft.Extensions.Configuration;

namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public abstract class BaseTest : IAsyncDisposable
{
    protected readonly string SessionQueueName;
    protected readonly string SimpleQueueName;
    
    protected ServiceBusClient Client;
    
    protected BaseTest()
    {
        var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .Build();
        
        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        SessionQueueName = configuration["Queues:SessionTestQueue"];
        SimpleQueueName = configuration["Queues:SimpleTestQueue"];
        
        Client = new ServiceBusClient(connectionString);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await Client.DisposeAsync();
    }
}