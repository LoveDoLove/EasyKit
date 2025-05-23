using System.ComponentModel;
using System.Diagnostics;

namespace CommonUtilities.Helpers;

public static class ProcessHelper
{
    public static async Task<(string output, string error, int exitCode)> RunProcessAsync(
        string fileName,
        string arguments,
        string? workingDirectory = null,
        int timeoutMilliseconds = 300000, // Default 5 minutes
        CancellationToken cancellationToken = default)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        if (!string.IsNullOrEmpty(workingDirectory))
            process.StartInfo.WorkingDirectory = workingDirectory;

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                outputBuilder.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                errorBuilder.AppendLine(e.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var processTask = process.WaitForExitAsync(cancellationToken);
            var timeoutTask = Task.Delay(timeoutMilliseconds, cancellationToken);

            if (await Task.WhenAny(processTask, timeoutTask) == timeoutTask)
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill(true);
                    }
                    catch
                    {
                    }

                    throw new TimeoutException($"Process timed out: {fileName} {arguments}");
                }

            return (outputBuilder.ToString(), errorBuilder.ToString(), process.ExitCode);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                try
                {
                    process.Kill(true);
                }
                catch
                {
                }

            throw;
        }
        catch (Win32Exception ex)
        {
            throw new InvalidOperationException($"Command not found: {fileName}. {ex.Message}", ex);
        }
        catch (Exception)
        {
            if (!process.HasExited)
                try
                {
                    process.Kill(true);
                }
                catch
                {
                }

            throw;
        }
    }
}