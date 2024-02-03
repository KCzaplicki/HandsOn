using HandsOn.AspNetCore.IntegrationTests.Services;

namespace HandsOn.AspNetCore.Tests.Fakes;

public sealed class MockFeatureFlagsManager : IFeatureFlagsManager
{
    private readonly IFeatureFlagsManager _featureFlagsManager;

    public MockFeatureFlagsManager(IFeatureFlagsManager featureFlagsManager)
    {
        _featureFlagsManager = featureFlagsManager;
    }
    
    public IDictionary<string, string> GetFeatureFlags() => _featureFlagsManager.GetFeatureFlags();

    public string GetFeatureFlag(string key) => _featureFlagsManager.GetFeatureFlag(key);

    public void SetFeatureFlag(string key, string value) => 
        _featureFlagsManager.SetFeatureFlag(key, $"DECORATED {value}");
}