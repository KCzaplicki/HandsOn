using HandsOn.AspNetCore.IntegrationTests.Endpoints;
using HandsOn.AspNetCore.IntegrationTests.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Redis";
});

builder.Services.AddSingleton<IFeatureFlagsManager>(_ =>
{
    var initialState = new Dictionary<string, string>();
    builder.Configuration
        .GetSection("FeatureFlags")
        .Bind(initialState);

    return new FeatureFlagsManager(initialState);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.AddWeatherForecastEndpoints();
app.AddFeatureFlagsEndpoints();
app.AddCacheEndpoints();

app.Run();

public partial class Program { }