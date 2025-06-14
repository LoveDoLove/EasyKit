namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Provides functionality to display a progress bar in the console.
/// </summary>
public class ProgressView
{
    private readonly int _barWidth;
    private readonly ConsoleColor _progressColor;
    private readonly string _startText;
    private readonly DateTime _startTime;
    private readonly int _totalSteps;
    private int _currentStep;
    private bool _isComplete;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressView" /> class and draws the initial progress bar.
    /// </summary>
    /// <param name="startText">Text to display before the progress bar (e.g., "Loading:", "Processing:").</param>
    /// <param name="totalSteps">The total number of steps for the operation.</param>
    /// <param name="progressColor">The color of the progress bar. Defaults to Cyan.</param>
    /// <param name="barWidth">The width of the progress bar itself (excluding text). Defaults to 30 characters.</param>
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

    /// <summary>
    ///     Updates the progress bar to a specific step.
    /// </summary>
    /// <param name="step">The current step number.</param>
    /// <param name="statusMessage">
    ///     Optional message to display next to the progress bar (e.g., "Processing item X...").
    ///     Defaults to "Step X of Y".
    /// </param>
    public void Update(int step, string? statusMessage = null)
    {
        // Ensure current step does not exceed total steps
        _currentStep = Math.Min(step, _totalSteps);
        _currentStep = Math.Max(0, _currentStep); // Ensure current step is not negative

        // Redraw the progress bar with the new status
        DrawProgressBar(statusMessage);
    }

    /// <summary>
    ///     Increments the progress bar by one step.
    /// </summary>
    /// <param name="statusMessage">Optional message to display next to the progress bar.</param>
    public void Increment(string? statusMessage = null)
    {
        if (_currentStep < _totalSteps) _currentStep++;
        DrawProgressBar(statusMessage);
    }

    /// <summary>
    ///     Marks the progress as complete, fills the progress bar, and displays a completion message.
    /// </summary>
    /// <param name="completionMessage">Optional message to display upon completion. Defaults to "Complete".</param>
    public void Complete(string? completionMessage = null)
    {
        _isComplete = true;
        _currentStep = _totalSteps; // Ensure bar is full
        DrawProgressBar(completionMessage ?? "Complete");
        Console.WriteLine(); // Add a newline to move cursor below the completed progress bar
    }

    /// <summary>
    ///     Draws or redraws the progress bar in the console.
    /// </summary>
    /// <param name="statusMessage">The message to display next to the progress bar.</param>
    private void DrawProgressBar(string? statusMessage = null)
    {
        // Calculate progress percentage. Avoid division by zero if totalSteps is 0.
        int percent = _totalSteps == 0 ? 100 : _currentStep * 100 / _totalSteps;
        percent = Math.Clamp(percent, 0, 100); // Ensure percent is between 0 and 100

        // Calculate the number of filled characters in the progress bar. Avoid division by zero.
        int progressChars = _totalSteps == 0 ? _barWidth : _currentStep * _barWidth / _totalSteps;
        progressChars = Math.Clamp(progressChars, 0, _barWidth); // Ensure it's within bar width

        // Construct the progress bar string (e.g., "[████    ]")
        string progressBarString =
            "[" + new string('█', progressChars) + new string(' ', _barWidth - progressChars) + "]";

        // Calculate elapsed time since the progress view was started
        TimeSpan elapsed = DateTime.Now - _startTime;
        // Format elapsed time (seconds if under a minute, otherwise minutes)
        string timeText = elapsed.TotalSeconds < 60
            ? $"{elapsed.TotalSeconds:F1}s" // e.g., "12.3s"
            : $"{elapsed.TotalMinutes:F1}m"; // e.g., "1.2m"

        // Determine the status text to display
        string currentStatusText = statusMessage ?? $"Step {_currentStep} of {_totalSteps}";

        // Prepare the full line to be written to ensure it fits and for clearing purposes
        string lineToWrite = $"{_startText} {progressBarString} {percent}% - {currentStatusText} ({timeText})";
        if (_isComplete) lineToWrite += " ✓";

        // Move cursor to the beginning of the current line to overwrite
        Console.SetCursorPosition(0, Console.CursorTop);

        // Clear the current line by writing spaces, then return cursor to start
        // This prevents artifacts if the new line is shorter than the old one.
        Console.Write(new string(' ',
            Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 0)); // Handle WindowWidth being 0
        Console.SetCursorPosition(0, Console.CursorTop);

        // Write the descriptive text (e.g., "Loading:")
        Console.Write($"{_startText} ");

        // Write the progress bar itself with the specified color
        Console.ForegroundColor = _progressColor;
        Console.Write(progressBarString);
        Console.ResetColor(); // Reset color after drawing the bar

        // Write the percentage, status message, and elapsed time
        Console.Write($" {percent}% - {currentStatusText} ({timeText})");

        // If the task is marked as complete, add a green checkmark
        if (_isComplete)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" ✓");
            Console.ResetColor();
        }
    }
}