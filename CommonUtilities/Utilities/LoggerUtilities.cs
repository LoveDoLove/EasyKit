using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace CommonUtilities.Utilities;

public static class LoggerUtilities
{
    public static void StartLog(string applicationName = "Application")
    {
        // Create logs directory
        string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDirectory); // Ensure log directory exists
        string logFilePath = Path.Combine(logDirectory, $"{applicationName.ToLowerInvariant()}.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName) // Adds app identifier to all logs
            .WriteTo.File(
                logFilePath, // Use the combined path
                fileSizeLimitBytes: 10_000_000, // 10MB size limit
                retainedFileCountLimit: 7, // Keep 7 days of logs
                rollingInterval: RollingInterval.Day,
                buffered: true, // Buffer writes for better performance
                flushToDiskInterval: TimeSpan.FromSeconds(5))
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Literate,
                restrictedToMinimumLevel: LogEventLevel.Information) // Only important info to console
            .CreateLogger();
    }

    public static void StopLog()
    {
        Log.CloseAndFlush(); // Ensure all buffered logs are written
    }
}