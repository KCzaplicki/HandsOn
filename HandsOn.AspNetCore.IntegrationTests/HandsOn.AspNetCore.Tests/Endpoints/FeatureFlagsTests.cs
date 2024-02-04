using System.Net;
using System.Net.Http.Json;
using HandsOn.AspNetCore.IntegrationTests.Services;
using HandsOn.AspNetCore.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HandsOn.AspNetCore.Tests.Endpoints;

public class FeatureFlagsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string FeatureFlagsEndpoint = "/feature-flags";
    
    public static readonly KeyValuePair<string, string> MockFeature = new("MockFeature", "MockValue");

    private readonly CustomWebApplicationFactory<Program> _factory;

    public FeatureFlagsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetFeatureFlags_Returns200HttpStatusCode()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(FeatureFlagsEndpoint);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetFeatureFlags_ReturnsApplicationJsonContentType()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(FeatureFlagsEndpoint);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task GetFeatureFlags_ReturnsFeatureFlags()
    {
        var client = _factory.CreateClient();
        
        var featureFlags = await client.GetFromJsonAsync<Dictionary<string, string>>(FeatureFlagsEndpoint);
        
        featureFlags.Should().NotBeNull();
        featureFlags.Should().BeOfType<Dictionary<string, string>>();
        featureFlags.Should().Contain(MockFeature);
    }
    
    [Fact]
    public async Task GetFeatureFlag_Returns200HttpStatusCode_WhenFeatureFlagExists()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{FeatureFlagsEndpoint}/{MockFeature.Key}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetFeatureFlag_Returns404HttpStatusCode_WhenFeatureFlagDoesNotExist()
    {
        const string featureFlagKey = "NOT_EXISTING_FEATURE_FLAG";
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{FeatureFlagsEndpoint}/{featureFlagKey}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetFeatureFlag_ReturnsApplicationJsonContentType()
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync($"{FeatureFlagsEndpoint}/{MockFeature.Key}");
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetFeatureFlag_ReturnsFeatureFlagValue()
    {
        var client = _factory.CreateClient();
        
        var featureFlag = await client.GetFromJsonAsync<string>($"{FeatureFlagsEndpoint}/{MockFeature.Key}");
        
        featureFlag.Should().NotBeNullOrEmpty();
        featureFlag.Should().Be(MockFeature.Value);
    }
    
    [Fact]
    public async Task SetFeatureFlag_Returns200HttpStatusCode()
    {
        var featureFlag = new KeyValuePair<string, string>($"NewFeatureFlag-{Guid.NewGuid()}", "NewFeatureFlagValue");
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync($"{FeatureFlagsEndpoint}/{featureFlag.Key}", featureFlag.Value);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SetFeatureFlag_ReturnsApplicationJsonContentType()
    {
        var featureFlag = new KeyValuePair<string, string>($"NewFeatureFlag-{Guid.NewGuid()}", "NewFeatureFlagValue");
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync($"{FeatureFlagsEndpoint}/{featureFlag.Key}", featureFlag.Value);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task SetFeatureFlag_CreatesFeatureFlag_WhenFeatureFlagDoesNotExist()
    {
        var featureFlag = new KeyValuePair<string, string>($"NewFeatureFlag-{Guid.NewGuid()}", "NewFeatureFlagValue");
        var client = _factory.CreateClient();
        
        await client.PostAsJsonAsync($"{FeatureFlagsEndpoint}/{featureFlag.Key}", featureFlag.Value);
        
        var featureFlagsManager = _factory.Services.GetService<IFeatureFlagsManager>();
        var responseFeatureFlag = featureFlagsManager.GetFeatureFlag(featureFlag.Key);
        
        responseFeatureFlag.Should().NotBeNullOrEmpty();
        responseFeatureFlag.Should().Contain(featureFlag.Value);
        responseFeatureFlag.Should().StartWith("DECORATED");
    }
    
    [Fact]
    public async Task SetFeatureFlag_UpdatesFeatureFlag_WhenFeatureFlagExists()
    {
        var featureFlag = new KeyValuePair<string, string>($"NewFeatureFlag-{Guid.NewGuid()}", "NewFeatureFlagValue");
        var featureFlagsManager = _factory.Services.GetService<IFeatureFlagsManager>();
        featureFlagsManager.SetFeatureFlag(featureFlag.Key, string.Empty);
        var client = _factory.CreateClient();
        
        await client.PostAsJsonAsync($"{FeatureFlagsEndpoint}/{featureFlag.Key}", featureFlag.Value);
        
        var responseFeatureFlag = featureFlagsManager.GetFeatureFlag(featureFlag.Key);
        
        responseFeatureFlag.Should().NotBeNullOrEmpty();
        responseFeatureFlag.Should().Contain(featureFlag.Value);
        responseFeatureFlag.Should().StartWith("DECORATED");
    }
}

public sealed class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> 
    where TEntryPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IFeatureFlagsManager>();
            services.AddSingleton<IFeatureFlagsManager>(new FeatureFlagsManager(new Dictionary<string, string>
            {
                { FeatureFlagsTests.MockFeature.Key, FeatureFlagsTests.MockFeature.Value }
            }));

            services.Decorate<IFeatureFlagsManager, MockFeatureFlagsManager>();
        });
    }
}