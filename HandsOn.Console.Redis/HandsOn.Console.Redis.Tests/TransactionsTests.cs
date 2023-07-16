namespace HandsOn.Console.Redis.Tests;

public class TransactionsTests
{
    private const string ConnectionString = "localhost";
    
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public TransactionsTests()
    {
        _redis = ConnectionMultiplexer.Connect(ConnectionString);
        _db = _redis.GetDatabase();
    }

    [Fact]
    public async Task TransactionSetValuesAtomicallyOnExecute()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        
        var transaction = _db.CreateTransaction();
        transaction.StringSetAsync(key1, "value");
        transaction.StringSetAsync(key2, "value");
        await transaction.ExecuteAsync();
        
        var values = await _db.StringGetAsync(new[] { new RedisKey(key1), new RedisKey(key2) });
        values.Select(v => v.ToString()).Should().BeEquivalentTo("value", "value");
    }
    
    [Fact]
    public async Task TransactionWithoutExecuteDiscardsChanges()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        
        var transaction = _db.CreateTransaction();
        transaction.StringSetAsync(key1, "value");
        transaction.StringSetAsync(key2, "value");
        
        var values = await _db.StringGetAsync(new[] { new RedisKey(key1), new RedisKey(key2) });
        values.Any(v => v.HasValue).Should().BeFalse();
    }
    
    [Fact]
    public async Task MultipleTransactionsCanBeCreated()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        
        var transaction1 = _db.CreateTransaction();
        var transaction2 = _db.CreateTransaction();
        
        var result1 = await transaction1.ExecuteAsync();
        var result2 = await transaction2.ExecuteAsync();
        
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }
    
    [Fact]
    public async Task LastTransactionChangesOverridesPreviousChanges()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        
        var transaction1 = _db.CreateTransaction();
        var transaction2 = _db.CreateTransaction();
        
        transaction1.StringSetAsync(key1, "value");
        transaction1.StringSetAsync(key2, "value");
        await transaction1.ExecuteAsync();
        
        transaction2.StringSetAsync(key1, "value2");
        transaction2.StringSetAsync(key2, "value2");
        await transaction2.ExecuteAsync();
        
        var values = await _db.StringGetAsync(new[] { new RedisKey(key1), new RedisKey(key2) });
        values.Select(v => v.ToString()).Should().BeEquivalentTo("value2", "value2");
    }
}