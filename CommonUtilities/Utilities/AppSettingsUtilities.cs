using Microsoft.Extensions.Configuration;

// Required for Path and FileNotFoundException

namespace CommonUtilities.Utilities;

public static class AppSettingsUtilities
{
    private static readonly Lazy<IConfiguration> LazyConfiguration = new(AppSettingsInit);

    public static IConfiguration Configuration => LazyConfiguration.Value;

    private static IConfiguration AppSettingsInit()
    {
        // Use Path.Combine for robust path construction
        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        if (!File.Exists(appSettingsPath))
            // Throw an exception instead of exiting the application
            throw new FileNotFoundException("appsettings.json not found!", appSettingsPath);

        return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build();
    }
}