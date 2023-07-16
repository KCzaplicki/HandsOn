Console.WriteLine("Console application with Redis and RedLock.net - Service B\r\n");

await using var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
var redlockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { redis });

const string lockKey = "resource-1-lock";
var expiryTime = TimeSpan.FromMinutes(1);
var waitTime = TimeSpan.FromMinutes(2);
var retryTime = TimeSpan.FromSeconds(1);

while (true)
{
    // Lock resource
    Console.WriteLine($"[ServiceB] Locking resource '{lockKey}'");
    await using var redLock = await redlockFactory.CreateLockAsync(lockKey, expiryTime, waitTime, retryTime);

    if (redLock.IsAcquired)
    {
        // Perform work
        Console.WriteLine("\tLock acquired. Performing work...");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine("\tWork completed.");

        // Release lock
        Console.WriteLine($"[ServiceB] Releasing lock for resource '{lockKey}'");
        await redLock.DisposeAsync();
    }

    await Task.Delay(TimeSpan.FromSeconds(5));
}