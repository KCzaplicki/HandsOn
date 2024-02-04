using Microsoft.Extensions.Caching.Distributed;

namespace HandsOn.AspNetCore.IntegrationTests.Endpoints;

public static class CacheEndpoints
{
    private const string CacheEndpointPrefix = "/cache";

    public static void AddCacheEndpoints(this WebApplication app)
    {
        var cacheEndpoints = app.MapGroup(CacheEndpointPrefix);

        cacheEndpoints.MapGet("/{key}", async (IDistributedCache cache, string key) => await cache.GetStringAsync(key))
            .WithName("GetCache")
            .WithOpenApi();

        cacheEndpoints.MapPost("/{key}", async (IDistributedCache cache, string key, HttpRequest request) =>
            {
                var bodyReader = new StreamReader(request.Body);
                var value = await bodyReader.ReadToEndAsync();
                await cache.SetStringAsync(key, value);
            })
            .WithName("SetCache")
            .WithOpenApi();
    }
}