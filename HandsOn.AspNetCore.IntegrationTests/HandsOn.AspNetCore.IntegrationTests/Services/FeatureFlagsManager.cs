namespace HandsOn.AspNetCore.IntegrationTests.Services;

public sealed class FeatureFlagsManager : IFeatureFlagsManager
{
    private readonly Dictionary<string, string> _featureFlags;

    public FeatureFlagsManager(IDictionary<string, string> initialState) => 
        _featureFlags = new Dictionary<string, string>(initialState);
    
    public IDictionary<string, string> GetFeatureFlags() => _featureFlags;

    public string GetFeatureFlag(string key)
    {
        if (!_featureFlags.ContainsKey(key))
        {
            throw new ArgumentOutOfRangeException(key);
        }
        
        return _featureFlags[key];
    }

    public void SetFeatureFlag(string key, string value) => _featureFlags[key] = value;
}