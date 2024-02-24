using System.Text;

Console.WriteLine("HandsOn.Console.RabbitMQ Exchange - Producer");

if (args.Length != 5)
{
    Console.WriteLine($"\nInvalid parameters count.\n\n  Usage: ./{args[0]} <exchangeType> <queueName> <routingKey> <message>\n");
    return;
}

var exchangeType = args[1];
var queueName = args[2];
var routingKey = args[3];
var message = args[4];

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "handson_exchange";
channel.ExchangeDeclare(exchangeName, exchangeType);

channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey);

var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchangeName, routingKey, null, body);

Console.WriteLine($"[{DateTime.Now.ToLocalTime()}] Sent '{message}'\n");