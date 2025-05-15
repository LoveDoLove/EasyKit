namespace EasyKit.Views;

public class ProgressView
{
    private readonly int _barWidth;
    private readonly ConsoleColor _progressColor;
    private readonly string _startText;
    private readonly DateTime _startTime;
    private readonly int _totalSteps;
    private int _currentStep;
    private bool _isComplete;

    public ProgressView(string startText, int totalSteps, ConsoleColor progressColor = ConsoleColor.Cyan,
        int barWidth = 30)
    {
        _startText = startText;
        _totalSteps = totalSteps;
        _progressColor = progressColor;
        _barWidth = barWidth;
        _currentStep = 0;
        _isComplete = false;
        _startTime = DateTime.Now;

        // Draw the initial empty progress bar
        DrawProgressBar();
    }

    public void Update(int step, string? statusMessage = null)
    {
        // Ensure we don't go beyond total steps
        _currentStep = Math.Min(step, _totalSteps);

        // Draw the progress bar
        DrawProgressBar(statusMessage);
    }

    public void Increment(string? statusMessage = null)
    {
        _currentStep++;
        DrawProgressBar(statusMessage);
    }

    public void Complete(string? completionMessage = null)
    {
        _isComplete = true;
        _currentStep = _totalSteps;
        DrawProgressBar(completionMessage ?? "Complete");
        Console.WriteLine(); // Add a newline after completion
    }

    private void DrawProgressBar(string? statusMessage = null)
    {
        // Calculate progress percentage
        int percent = _totalSteps == 0 ? 100 : _currentStep * 100 / _totalSteps;

        // Calculate the number of characters in the bar
        int progressChars = _totalSteps == 0 ? _barWidth : _currentStep * _barWidth / _totalSteps;

        // Create the progress bar
        string progressBar = "[" + new string('█', progressChars) + new string(' ', _barWidth - progressChars) + "]";

        // Calculate elapsed time
        TimeSpan elapsed = DateTime.Now - _startTime;
        string timeText = elapsed.TotalSeconds < 60
            ? $"{elapsed.TotalSeconds:F1}s"
            : $"{elapsed.TotalMinutes:F1}m";

        // Create the status text
        string status = statusMessage ?? $"Step {_currentStep} of {_totalSteps}";

        // Move cursor to the beginning of the line
        Console.SetCursorPosition(0, Console.CursorTop);

        // Clear the current line
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop);

        // Write the start text
        Console.Write($"{_startText} ");

        // Write the progress bar with color
        Console.ForegroundColor = _progressColor;
        Console.Write(progressBar);
        Console.ResetColor();

        // Write the percentage, status and time
        Console.Write($" {percent}% - {status} ({timeText})");

        // If completed, add a checkmark
        if (_isComplete)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" ✓");
            Console.ResetColor();
        }
    }
}