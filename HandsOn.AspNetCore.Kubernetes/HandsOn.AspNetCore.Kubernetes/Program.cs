using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Redis";
});

var app = builder.Build();

app.MapGet("/", () => $"HandsOn.AspNetCore.Kubernetes - {Environment.GetEnvironmentVariable("POD_NAME")}");
app.MapGet("/env", () => Results.Json(Environment.GetEnvironmentVariables(), new JsonSerializerOptions { WriteIndented = true }));

app.MapGet("/cache/{key}", async (string key, IDistributedCache cache) => Results.Json(await cache.GetStringAsync(key)));
app.MapPost("/cache/{key}", async (string key, HttpRequest httpRequest, IDistributedCache cache) =>
{
    using var reader = new StreamReader(httpRequest.Body);
    var value = await reader.ReadToEndAsync();
    
    await cache.SetStringAsync(key, value);
    return Results.Ok();
});
app.MapDelete("/cache/{key}", async (string key, IDistributedCache cache) =>
{
    await cache.RemoveAsync(key);
    return Results.Ok();
});

app.Run();