using Azure.Messaging.ServiceBus;
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
Console.WriteLine($"Message received: '{message.Body}'");
await receiver.CompleteMessageAsync(message);

await receiver.DisposeAsync();
await client.DisposeAsync();