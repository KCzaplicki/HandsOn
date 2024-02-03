namespace HandsOn.AspNetCore.IntegrationTests.Services;

public interface IFeatureFlagsManager
{
    IDictionary<string, string> GetFeatureFlags();
    
    string GetFeatureFlag(string key);
    
    void SetFeatureFlag(string key, string value);
}