Console.WriteLine("Console application with Redis - Subscriber\r\n");

await using var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
var subscriber = redis.GetSubscriber();

// Subscribe to literal channel
const string channelName = "HandsOnChannel";
Console.WriteLine($"Subscribing to channel '{channelName}'");
await subscriber.SubscribeAsync(new RedisChannel(channelName, RedisChannel.PatternMode.Literal), (channel, message) =>
{
    Console.WriteLine($"[{channelName}] Message received from channel '{channel}'. Content: '{message}'");
});

// Subscribe to pattern channel
const string channelPattern = $"{channelName}.*";
Console.WriteLine($"Subscribing to channels '{channelPattern}'");
await subscriber.SubscribeAsync(new RedisChannel(channelPattern, RedisChannel.PatternMode.Pattern), 
    (channel, message) =>
{
    Console.WriteLine($"[{channelPattern}] Message received from channel '{channel}'. Content: '{message}'");
});

Console.WriteLine("Press any key to exit\r\n");
Console.ReadKey();