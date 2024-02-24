Console.WriteLine("HandsOn.Console.RabbitMQ.Queue - Producer");

if (args.Length != 2)
{
    Console.WriteLine($"\nInvalid parameters count.\n\n  Usage: ./{args[0]} <message>\n");
    return;
}

var message = args[1];

const string queueName = "basic_queue";

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queueName, false, false, false, null);

var messageBytes = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(string.Empty, queueName, null, messageBytes);

Console.WriteLine($"[{DateTime.Now.ToLocalTime()}] Sent '{message}'\n");