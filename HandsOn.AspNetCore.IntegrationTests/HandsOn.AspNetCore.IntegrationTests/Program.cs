using System.Net.Mime;
using HandsOn.AspNetCore.IntegrationTests.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IFeatureFlagsManager>(sp =>
{
    var initialState = new Dictionary<string, string>();
    builder.Configuration
        .GetSection("FeatureFlags")
        .Bind(initialState);

    return new FeatureFlagsManager(initialState);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

var featureFlagsEndpoint = app.MapGroup("/feature-flags");

featureFlagsEndpoint.MapGet("/", (IFeatureFlagsManager featureFlagsManager) => 
        featureFlagsManager.GetFeatureFlags())
    .WithName("GetFeatureFlags")
    .WithOpenApi();

featureFlagsEndpoint.MapGet("/{key}",
        (IFeatureFlagsManager featureFlagsManager, string key) => 
        {
            try 
            {
                return Results.Ok(featureFlagsManager.GetFeatureFlag(key));
            }
            catch (ArgumentOutOfRangeException)
            {
                return Results.NotFound();
            }
        })
    .WithName("GetFeatureFlag")
    .WithOpenApi();

featureFlagsEndpoint.MapPost("/{key}", async (IFeatureFlagsManager featureFlagsManager, HttpContext context, string key) =>
    {
        using var bodyReader = new StreamReader(context.Request.Body);
        var value = await bodyReader.ReadToEndAsync();
        featureFlagsManager.SetFeatureFlag(key, value);
    })
    .WithName("SetFeatureFlag")
    .WithOpenApi();

app.Run();

public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}