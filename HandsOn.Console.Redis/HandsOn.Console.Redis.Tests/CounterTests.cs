namespace HandsOn.Console.Redis.Tests;

public class CounterTests
{
    private const string ConnectionString = "localhost:6379";
    
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public CounterTests()
    {
        _redis = ConnectionMultiplexer.Connect(ConnectionString);
        _db = _redis.GetDatabase();
    }

    [Fact]
    public async Task StringIncrement_SetKeyWithInitialValue1()
    {
        var key = Guid.NewGuid().ToString();
        
        var valueOnIncrement = await _db.StringIncrementAsync(key);
        
        valueOnIncrement.Should().Be(1);
        var value = await _db.StringGetAsync(key);
        value.Should().Be("1");
    }
    
    [Fact]
    public async Task StringIncrement_IncrementsValueBy1()
    {
        var key = Guid.NewGuid().ToString();
        
        await _db.StringIncrementAsync(key);
        await _db.StringIncrementAsync(key);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("2");
    }
    
    [Fact]
    public async Task StringIncrement_IncrementsValue_ByValueProvidedInParameter()
    {
        var key = Guid.NewGuid().ToString();
        const int incrementBy = 5;
        
        await _db.StringIncrementAsync(key);
        await _db.StringIncrementAsync(key, incrementBy);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("6");
    }
    
    [Fact]
    public async Task StringDecrement_SetKeyWithInitialValue1()
    {
        var key = Guid.NewGuid().ToString();
        
        var valueOnDecrement = await _db.StringDecrementAsync(key);
        
        valueOnDecrement.Should().Be(-1);
        var value = await _db.StringGetAsync(key);
        value.Should().Be("-1");
    }
    
    [Fact]
    public async Task StringDecrement_DecrementsValueBy1()
    {
        var key = Guid.NewGuid().ToString();
        
        await _db.StringIncrementAsync(key);
        await _db.StringDecrementAsync(key);
        
        var value = await _db.StringGetAsync(key);
        value.Should().Be("0");
    }
    
    [Fact]
    public async Task StringDecrement_DecrementsValue_ByValueProvidedInParameter()
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