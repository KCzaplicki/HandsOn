Console.WriteLine("Console Application with Azure Service Bus - Topic sender\r\n");

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
var sender = client.CreateSender(topicName);
Console.WriteLine($"Receiver for topic '{topicName}' created");

var content = $"[{DateTime.UtcNow}] - Hello world!";
var message = new ServiceBusMessage(content)
{
    ContentType = "text/plain",
    ApplicationProperties =
    {
        { "region", "EMEA" }
    }
};
await sender.SendMessageAsync(message);
Console.WriteLine($"Text message '{content}' sent");

await sender.DisposeAsync();
await client.DisposeAsync();