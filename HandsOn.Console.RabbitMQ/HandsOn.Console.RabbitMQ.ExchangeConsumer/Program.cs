Console.WriteLine("HandsOn.Console.RabbitMQ Exchange - Consumer");

if (args.Length != 4)
{
    Console.WriteLine($"\nInvalid parameters count.\n\n  Usage: ./{args[0]} <exchangeType> <queueName> <routingKey>\n");
    return;
}

var exchangeType = args[1];
var queueName = args[2];
var routingKey = args[3];

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "handson_exchange";
channel.ExchangeDeclare(exchangeName, exchangeType);

channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (_, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[{DateTime.Now.ToLocalTime()}] Received '{message}'");
};

channel.BasicConsume(queueName, true, consumer);

Console.WriteLine("\nPress any key to exit.\n");
Console.ReadKey();