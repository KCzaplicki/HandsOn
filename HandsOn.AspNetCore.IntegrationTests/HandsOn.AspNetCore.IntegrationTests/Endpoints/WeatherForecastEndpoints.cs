using HandsOn.AspNetCore.IntegrationTests.Models;

namespace HandsOn.AspNetCore.IntegrationTests.Endpoints;

public static class WeatherForecastEndpoints
{
    private const string WeatherForecastEndpointPrefix = "/weatherforecast";

    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    public static void AddWeatherForecastEndpoints(this WebApplication app)
    {
        app.MapGet(WeatherForecastEndpointPrefix, () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            Summaries[Random.Shared.Next(Summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();
    }
}