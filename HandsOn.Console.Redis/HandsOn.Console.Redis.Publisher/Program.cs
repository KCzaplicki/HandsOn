Console.WriteLine("Console application with Redis - Publisher\r\n");

await using var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
var publisher = redis.GetSubscriber();

// Publish messages for literal channel
const string channelName = "HandsOnChannel";
Console.WriteLine($"Sending messages to channel '{channelName}'");
var channel = new RedisChannel(channelName, RedisChannel.PatternMode.Literal);
await publisher.PublishAsync(channel, "message1");
await publisher.PublishAsync(channel, "message2");
await publisher.PublishAsync(channel, "message3");

// Publish messages for pattern channel
var userId = Guid.NewGuid().ToString();
Console.WriteLine($"Sending messages to channel '{channelName}'");
var userChannel = new RedisChannel($"{channelName}.{userId}", RedisChannel.PatternMode.Literal);
await publisher.PublishAsync(userChannel, $"message for '{userId}'");
