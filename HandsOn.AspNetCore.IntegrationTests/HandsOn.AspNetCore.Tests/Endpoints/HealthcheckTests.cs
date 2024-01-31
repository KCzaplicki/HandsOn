using System.Net;

namespace HandsOn.AspNetCore.Tests.Endpoints;

public class HealthcheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string HealthcheckEndpoint = "/health";
    
    private readonly WebApplicationFactory<Program> _factory;

    public HealthcheckTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetHealth_ReturnsHealthyStatus()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(HealthcheckEndpoint);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
    
    [Fact]
    public async Task GetHealth_Returns200HttpStatusCode()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(HealthcheckEndpoint);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetHealth_ReturnsTextPlainContentType()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(HealthcheckEndpoint);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
    }
}