using CommonUtilities.Utilities;
using Serilog;

namespace EasyKit.Services;

public class LoggerService
{
    public void Info(string message)
    {
        Log.Information(message);
    }

    public void Error(string message)
    {
        Log.Error(message);
    }

    public void Warning(string message)
    {
        Log.Warning(message);
    }

    public void Debug(string message)
    {
        Log.Debug(message);
    }

    public void Error(Exception exception, string message)
    {
        Log.Error(exception, message);
    }

    public void Fatal(string message)
    {
        Log.Fatal(message);
    }

    public void Fatal(Exception exception, string message)
    {
        Log.Fatal(exception, message);
    }
}