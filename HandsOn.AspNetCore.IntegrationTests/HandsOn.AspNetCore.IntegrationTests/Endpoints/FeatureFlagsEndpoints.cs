using HandsOn.AspNetCore.IntegrationTests.Services;

namespace HandsOn.AspNetCore.IntegrationTests.Endpoints;

public static class FeatureFlagsEndpoints
{
    private const string FeatureFlagsEndpointPrefix = "/feature-flags";

    public static void AddFeatureFlagsEndpoints(this WebApplication app)
    {
        var featureFlagsEndpoint = app.MapGroup(FeatureFlagsEndpointPrefix);
        
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
    }
}