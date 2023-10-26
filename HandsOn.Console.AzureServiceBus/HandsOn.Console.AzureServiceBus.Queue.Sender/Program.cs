using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HandsOn.Console.AzureServiceBus.Common.Models;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Console Application with Azure Service Bus - Queue sender\r\n");

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
var sender = client.CreateSender(queueName);
Console.WriteLine($"Sender for queue '{queueName}' created");

var content = $"[{DateTime.UtcNow}] - Hello world!";
var message = new ServiceBusMessage(content)
{
    ContentType = "text/plain",
};
await sender.SendMessageAsync(message);
Console.WriteLine($"Text message '{content}' sent");

var moneyTransferRequest = new MoneyTransferRequest
{
    SenderId = Guid.NewGuid().ToString(),
    RecipientId = Guid.NewGuid().ToString(),
    Value = 100.00,
    Currency = "EUR",
    RequestedAt = DateTime.UtcNow
};
var moneyTransferRequestJson = JsonSerializer.Serialize(moneyTransferRequest);
var moneyTransferRequestMessage = new ServiceBusMessage(moneyTransferRequestJson)
{
    Subject = nameof(moneyTransferRequest),
    ContentType = "application/json",
};
await sender.SendMessageAsync(moneyTransferRequestMessage);
Console.WriteLine($"JSON message '{moneyTransferRequestJson}' sent");

await sender.DisposeAsync();
await client.DisposeAsync();

