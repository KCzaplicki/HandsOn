Console.WriteLine("Console Application with Azure Service Bus - Topic subscription with region filters\r\n");

var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
Console.WriteLine($"Application environment: {environmentName}\r\n");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .Build();
    
var connectionString = configuration["ConnectionStrings:DefaultConnection"];
var client = new ServiceBusClient(connectionString);
Console.WriteLine("Client created");

var topicName = configuration["Topics:BaseTopic"];
var subscriptionName = configuration["Subscriptions:Subscription2"];
var receiver = client.CreateReceiver(topicName, subscriptionName);
Console.WriteLine($"Subscription '{subscriptionName}' for topic '{topicName}' created");

var message = await receiver.ReceiveMessageAsync();
Console.WriteLine($"Message received: '{message.Body}', region: '{message.ApplicationProperties["region"]}'");
await receiver.CompleteMessageAsync(message);

await receiver.DisposeAsync();
await client.DisposeAsync();