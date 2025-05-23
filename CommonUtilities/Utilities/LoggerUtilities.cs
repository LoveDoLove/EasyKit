using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace CommonUtilities.Utilities;

/// <summary>
/// Provides static helpers for configuring and writing logs using Serilog.
/// </summary>
public static class LoggerUtilities
{
    /// <summary>
    /// Starts Serilog logging with file and console sinks.
    /// </summary>
    public static void StartLog(string applicationName = "Application")
    {
        string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDirectory);
        string logFilePath = Path.Combine(logDirectory, $"{applicationName.ToLowerInvariant()}.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .WriteTo.File(
                logFilePath,
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 7,
                rollingInterval: RollingInterval.Day,
                buffered: true,
                flushToDiskInterval: TimeSpan.FromSeconds(5))
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Literate,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();
    }

    /// <summary>
    /// Stops Serilog logging and flushes all buffered logs.
    /// </summary>
    public static void StopLog()
    {
        Log.CloseAndFlush();
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    public static void Info(string message) => Log.Information(message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public static void Error(string message) => Log.Error(message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public static void Warning(string message) => Log.Warning(message);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    public static void Debug(string message) => Log.Debug(message);

    /// <summary>
    /// Logs an error message with exception.
    /// </summary>
    public static void Error(Exception exception, string message) => Log.Error(exception, message);

    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    public static void Fatal(string message) => Log.Fatal(message);

    /// <summary>
    /// Logs a fatal message with exception.
    /// </summary>
    public static void Fatal(Exception exception, string message) => Log.Fatal(exception, message);
}