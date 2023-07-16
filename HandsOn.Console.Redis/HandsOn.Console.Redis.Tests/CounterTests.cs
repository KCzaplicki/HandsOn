namespace HandsOn.Console.Redis.Tests;

public class CounterTests : IDisposable
{
    private const string ConnectionString = "localhost";
    
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public CounterTests()
    {
        _redis = ConnectionMultiplexer.Connect(ConnectionString);
        _db = _redis.GetDatabase();
    }

    [Fact]
    public async Task IncrementWillBeSetToOneWhenKeyDoesntExists()
    {
        var key = Guid.NewGuid().ToString();
        
        var valueOnIncrement = await _db.StringIncrementAsync(key);
        
        valueOnIncrement.Should().Be(1);
        var value = await _db.StringGetAsync(key);
        value.Should().Be("1");
    }
    
    [Fact]
    public async Task IncrementIsOneByDefault()
    {
        var key = Guid.NewGuid().ToString();
        
        await _db.StringIncrementAsync(key);
        await _db.StringIncrementAsync(key);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("2");
    }
    
    [Fact]
    public async Task IncrementCanBeSetInParameter()
    {
        var key = Guid.NewGuid().ToString();
        const int incrementBy = 5;
        
        await _db.StringIncrementAsync(key);
        await _db.StringIncrementAsync(key, incrementBy);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("6");
    }
    
    [Fact]
    public async Task DecrementWillBeSetToMinusOneWhenKeyDoesntExists()
    {
        var key = Guid.NewGuid().ToString();
        
        var valueOnDecrement = await _db.StringDecrementAsync(key);
        
        valueOnDecrement.Should().Be(-1);
        var value = await _db.StringGetAsync(key);
        value.Should().Be("-1");
    }
    
    [Fact]
    public async Task DecrementIsMinusOneByDefault()
    {
        var key = Guid.NewGuid().ToString();
        
        await _db.StringDecrementAsync(key);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("-1");
    }
    
    [Fact]
    public async Task DecrementCanBeSetInParameter()
    {
        var key = Guid.NewGuid().ToString();
        const int decrementBy = 5;
        
        await _db.StringIncrementAsync(key);
        await _db.StringDecrementAsync(key, decrementBy);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("-4");
    }
    
    public void Dispose()
    {
        _redis?.Dispose();
    }
}