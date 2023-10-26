using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HandsOn.Console.AzureServiceBus.Common.Models;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Console Application with Azure Service Bus - Queue receiver\r\n");

var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
Console.WriteLine($"Application environment: {environmentName}\r\n");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .Build();

var connectionString = configuration["ConnectionStrings:DefaultConnection"];
var client = new ServiceBusClient(connectionString);
Console.WriteLine("Client created");

var queueName = configuration["Queues:BaseQueue"];
var receiver = client.CreateReceiver(queueName);
Console.WriteLine($"Receiver for queue '{queueName}' created");

var message = await receiver.ReceiveMessageAsync();

switch (message.ContentType)
{
    case "text/plain":
        Console.WriteLine($"Message received: '{message.Body}'");
        await receiver.CompleteMessageAsync(message);
        break;
    case "application/json":
        var messageType = typeof(BaseModel).Assembly
            .GetTypes()
            .FirstOrDefault(t => t.Name.Contains(message.Subject, StringComparison.InvariantCultureIgnoreCase));

        if(messageType is null)
        {
            Console.WriteLine($"Message type mapping not found. Message body: '{message.Body}'");
            await receiver.DeadLetterMessageAsync(message);
            break;
        }
        
        var moneyTransferRequest = JsonSerializer.Deserialize(message.Body, messageType);
        Console.WriteLine($"JSON Message type: {message.Subject}, content: '{moneyTransferRequest}'");
        await receiver.CompleteMessageAsync(message);
        break;
    default:
        Console.WriteLine($"Message received: '{message.Body}'");
        await receiver.DeadLetterMessageAsync(message);
        break;
}

await receiver.DisposeAsync();
await client.DisposeAsync();