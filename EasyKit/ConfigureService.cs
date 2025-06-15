using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyKit;

internal static class ConfigureService
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            // Add other logging providers if needed
        });

        return services.BuildServiceProvider();
    }
}