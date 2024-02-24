Console.WriteLine("HandsOn.Console.RabbitMQ.Queue - Consumer");

const string queueName = "basic_queue";

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queueName, false, false, false, null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_, eventArgs) =>
{
    var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"[{DateTime.Now.ToLocalTime()}] Received '{message}'");
};

channel.BasicConsume(queueName, true, consumer);

Console.WriteLine("\nPress any key to exit.\n");
Console.ReadKey();