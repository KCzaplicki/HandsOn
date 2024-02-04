using System.Net;
using System.Net.Http.Json;
using HandsOn.AspNetCore.IntegrationTests.Models;

namespace HandsOn.AspNetCore.Tests.Endpoints;

public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string WeatherForecastEndpoint = "/weatherforecast";
    
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetWeatherForecast_Returns200HttpStatusCode()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(WeatherForecastEndpoint);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetWeatherForecast_ReturnsApplicationJsonContentType()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(WeatherForecastEndpoint);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task GetWeatherForecast_ReturnsWeatherForecast()
    {
        var client = _factory.CreateClient();
        
        var weatherForecast = await client.GetFromJsonAsync<WeatherForecast[]>(WeatherForecastEndpoint);
        
        weatherForecast.Should().NotBeNull();
        weatherForecast.Should().BeOfType<WeatherForecast[]>();
        weatherForecast.Should().HaveCount(5);

        foreach (var weatherForecastItem in weatherForecast)
        {
            weatherForecastItem.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.Now));
            weatherForecastItem.TemperatureC.Should().BeInRange(-20, 55);
            weatherForecastItem.Summary.Should().NotBeNullOrEmpty();
        }
    }
}