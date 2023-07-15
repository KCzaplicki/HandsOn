Console.WriteLine("Console application with redis database\r\n");

await using var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
var db = redis.GetDatabase();

// Test connection
var pong = await db.PingAsync();
Console.WriteLine($"Ping responded in '{pong}'");

// Set value
const string key = "key1";
Console.WriteLine($"Setting value for '{key}'");
db.StringSet(key, "value1");

// Get value
var value1 = await db.StringGetAsync(key);
Console.WriteLine($"Value for '{key}' is '{value1}'");
