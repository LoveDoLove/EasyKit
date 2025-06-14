using CommonUtilities.Helpers.ContextMenuManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

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

        // Register IContextMenuManager
        services.AddScoped<IContextMenuManager>(sp =>
        {
            var osPlatform = OSPlatform.Create("Unknown");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                osPlatform = OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) osPlatform = OSPlatform.Linux;
            // Add other OS checks like OSPlatform.OSX if needed

            if (osPlatform == OSPlatform.Windows)
                return new WindowsContextMenuManager(@"Software\\EasyKit\\ContextMenuEntries");

            if (osPlatform == OSPlatform.Linux) return new LinuxContextMenuManager();
            // Potentially add MacOS support or a default/null implementation
            var logger = sp.GetRequiredService<ILogger<Program>>(); // Fallback logger for the warning
            logger.LogWarning(
                $"Context menu management is not supported on this OS: {RuntimeInformation.OSDescription}. Attempting to use it will result in an exception.");
            // Return a NullContextMenuManager or throw if strict support is required.
            // For now, let's throw as per the initial requirement.
            throw new PlatformNotSupportedException(
                $"Context menu management is not supported on this OS: {RuntimeInformation.OSDescription}.");
        });

        return services.BuildServiceProvider();
    }
}