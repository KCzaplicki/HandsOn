using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace HandsOn.AspNetCore.Tests.Endpoints;

public class CacheTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private const string CacheEndpoint = "/cache";
    
    private static readonly KeyValuePair<string, string> CacheItem = new("CacheItem1", "Value1");
    
    private readonly WebApplicationFactory<Program> _factory;
    private IContainer _redisContainer = null!;

    public CacheTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await SetUpRedisContainer();
        await SeedRedisCacheItems();
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.StopAsync();
    }
    
    [Fact]
    public async Task GetCache_Returns200HttpStatusCode_WhenCacheExists()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{CacheEndpoint}/{CacheItem.Key}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetCache_ReturnsApplicationJsonContentType()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{CacheEndpoint}/{CacheItem.Key}");
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task GetCache_ReturnsValue_WhenCacheExists()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{CacheEndpoint}/{CacheItem.Key}");
        var value = await response.Content.ReadAsStringAsync();
        value = JsonSerializer.Deserialize<string>(value);
        
        value.Should().Be(CacheItem.Value);
    }
    
    [Fact]
    public async Task SetCache_Returns200HttpStatusCode()
    {
        var newCacheItem = new KeyValuePair<string, string>($"CacheItem-{Guid.NewGuid()}", "Value");
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync($"{CacheEndpoint}/{newCacheItem.Key}", newCacheItem.Value);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SetCache_ReturnsApplicationJsonContentType()
    {
        var newCacheItem = new KeyValuePair<string, string>($"CacheItem-{Guid.NewGuid()}", "Value");
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync($"{CacheEndpoint}/{newCacheItem.Key}", newCacheItem.Value);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task SetCache_SetsCacheItem_WhenCacheKeyDoesNotExists()
    {
        var newCacheItem = new KeyValuePair<string, string>($"CacheItem-{Guid.NewGuid()}", "Value");
        var client = _factory.CreateClient();
        
        await client.PostAsJsonAsync($"{CacheEndpoint}/{newCacheItem.Key}", newCacheItem.Value);
        
        var cache = _factory.Services.GetRequiredService<IDistributedCache>();
        var value = await cache.GetStringAsync(newCacheItem.Key);
        value = JsonSerializer.Deserialize<string>(value);
        
        value.Should().Be(newCacheItem.Value);
    }
    
    [Fact]
    public async Task SetCache_UpdatesCacheItem_WhenCacheKeyExists()
    {
        var existingCacheItem = new KeyValuePair<string, string>($"CacheItem-{Guid.NewGuid()}", "Value");
        var cache = _factory.Services.GetRequiredService<IDistributedCache>();
        await cache.SetStringAsync(existingCacheItem.Key, existingCacheItem.Value);
        
        var newValue = "NewValue";
        
        var client = _factory.CreateClient();
        
        await client.PostAsJsonAsync($"{CacheEndpoint}/{CacheItem.Key}", newValue);
        
        var value = await cache.GetStringAsync(CacheItem.Key);
        value = JsonSerializer.Deserialize<string>(value);
        
        value.Should().Be(newValue);
    }

    private async Task SeedRedisCacheItems()
    {
        var cache = _factory.Services.GetRequiredService<IDistributedCache>();
        await cache.SetStringAsync(CacheItem.Key, CacheItem.Value);
    }

    private async Task SetUpRedisContainer()
    {
        _redisContainer = new ContainerBuilder()
            .WithImage("redis")
            .WithPortBinding(6379)
            .Build();

        await _redisContainer.StartAsync();
    }
}