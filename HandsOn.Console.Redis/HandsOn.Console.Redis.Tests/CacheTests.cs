namespace HandsOn.Console.Redis.Tests;

public class CacheTests : IDisposable
{
    private const string ConnectionString = "localhost:6379";

    private const string Value = "value";
    
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public CacheTests()
    {
        _redis = ConnectionMultiplexer.Connect(ConnectionString);
        _db = _redis.GetDatabase();
    }
    
    [Fact]
    public async Task StringSet_KeyIsSetToNull_AfterExpiryTime()
    {
        var key = Guid.NewGuid().ToString();
        var expiryTime = TimeSpan.FromSeconds(2);
        await _db.StringSetAsync(key, Value, expiryTime);
        
        var valueBeforeExpiration = await _db.StringGetAsync(key);
        await Task.Delay(expiryTime);
        var valueAfterExpiration = await _db.StringGetAsync(key);

        valueBeforeExpiration.HasValue.Should().BeTrue();
        valueBeforeExpiration.Should().Be(Value);
        valueAfterExpiration.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task StringGetSetExpiry_UpdatesExpirationTime()
    {
        var key = Guid.NewGuid().ToString();
        var expiryTime = TimeSpan.FromSeconds(2);
        await _db.StringSetAsync(key, Value, expiryTime);
        
        var valueBeforeExpiration = await _db.StringGetSetExpiryAsync(key, TimeSpan.FromSeconds(5));
        await Task.Delay(expiryTime);
        var valueAfterExpiryUpdate = await _db.StringGetAsync(key);

        valueBeforeExpiration.HasValue.Should().BeTrue();
        valueBeforeExpiration.Should().Be(Value);
        valueAfterExpiryUpdate.HasValue.Should().BeTrue();
        valueAfterExpiryUpdate.Should().Be(Value);
    }

    [Fact]
    public async Task StringSet_WithWhenNotExists_IgnoreSetNewValue_When_KeyAlreadyExists()
    {
        var key = Guid.NewGuid().ToString();
        await _db.StringSetAsync(key, Value, null, When.Always);
        await _db.StringSetAsync(key, string.Empty, null, When.NotExists);
        
        var value = await _db.StringGetAsync(key);
        
        value.HasValue.Should().BeTrue();
        value.Should().Be(Value);
    }
    
    [Fact]
    public async Task HashSet_StoreHashedKeyValues()
    {
        var key = Guid.NewGuid().ToString();
        var hashKey = Guid.NewGuid().ToString();
        await _db.HashSetAsync(key, new HashEntry[] { new(hashKey, Value) });
        
        var value = await _db.HashGetAsync(key, hashKey);
        
        value.HasValue.Should().BeTrue();
        value.Should().Be(Value);
    }
    
    public void Dispose()
    {
        _redis?.Dispose();
    }
}