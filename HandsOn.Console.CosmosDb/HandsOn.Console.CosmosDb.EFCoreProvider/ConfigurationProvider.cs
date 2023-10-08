namespace HandsOn.Console.CosmosDb.EFCoreProvider;

public class ConfigurationProvider
{
    private readonly IConfigurationRoot _configuration;

    public ConfigurationProvider()
    {
        var environmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
        
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .Build();
    }
    
    public string GetConnectionString() => _configuration["ConnectionStrings:DefaultConnection"];
}