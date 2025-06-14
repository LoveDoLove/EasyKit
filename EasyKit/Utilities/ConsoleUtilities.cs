using EasyKit.Models;

namespace EasyKit.Utilities;

/// <summary>
///     Provides static utility methods for console input and output operations,
///     including colorized messages based on optional configuration.
/// </summary>
public static class ConsoleUtilities
{
    /// <summary>
    ///     Writes an informational message to the console.
    ///     The text and background colors can be customized via an optional <see cref="Config" /> object.
    ///     Defaults to white text on the current background if not specified or if colors are black on black.
    /// </summary>
    /// <param name="message">The message to write to the console.</param>
    /// <param name="config">
    ///     Optional configuration object to retrieve 'text_color' and 'background_color' settings.
    ///     Colors should be valid <see cref="ConsoleColor" /> enum names.
    /// </param>
    public static void WriteInfo(string message, Config? config = null)
    {
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        ConsoleColor? textColor = null;
        ConsoleColor? bgColor = null;

        if (config is not null)
        {
            // Attempt to parse text color from config
            var textColorObj =
                config.Get("text_color", string.Empty); // Provide a default to avoid null issues with ToString
            if (textColorObj != null && !string.IsNullOrEmpty(textColorObj.ToString()) &&
                Enum.TryParse(textColorObj.ToString(), true, out ConsoleColor parsedTextColor))
                textColor = parsedTextColor;

            // Attempt to parse background color from config
            var bgColorObj = config.Get("background_color", string.Empty); // Provide a default
            if (bgColorObj != null && !string.IsNullOrEmpty(bgColorObj.ToString()) &&
                Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor parsedBgColor)) bgColor = parsedBgColor;
        }

        // Apply colors
        if (textColor.HasValue)
            Console.ForegroundColor = textColor.Value;
        else
            Console.ForegroundColor = ConsoleColor.White; // Default info text color

        if (bgColor.HasValue) Console.BackgroundColor = bgColor.Value;
        // else, use current console background color

        // Special case: if text and background are both black (or resolved to be), force white text to ensure visibility.
        if (Console.ForegroundColor == ConsoleColor.Black && Console.BackgroundColor == ConsoleColor.Black)
            Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine(message);

        // Reset colors to original
        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;
    }

    /// <summary>
    ///     Writes an error message to the console.
    ///     The text color can be customized via an optional <see cref="Config" /> object (key: 'error_color').
    ///     Defaults to red text.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    /// <param name="config">
    ///     Optional configuration object to retrieve the 'error_color' setting.
    ///     The color should be a valid <see cref="ConsoleColor" /> enum name.
    /// </param>
    public static void WriteError(string message, Config? config = null)
    {
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor errorColor = ConsoleColor.Red; // Default error color

        if (config is not null)
        {
            var errorColorObj = config.Get("error_color", "Red"); // Default to "Red" if not found
            if (errorColorObj != null &&
                Enum.TryParse(errorColorObj.ToString(), true, out ConsoleColor parsedErrorColor))
                errorColor = parsedErrorColor;
        }

        Console.ForegroundColor = errorColor;
        Console.WriteLine(message);
        Console.ForegroundColor = originalFg; // Reset to original foreground color
    }

    /// <summary>
    ///     Writes a success message to the console.
    ///     The text color can be customized via an optional <see cref="Config" /> object (key: 'success_color').
    ///     Defaults to green text.
    /// </summary>
    /// <param name="message">The success message to write.</param>
    /// <param name="config">
    ///     Optional configuration object to retrieve the 'success_color' setting.
    ///     The color should be a valid <see cref="ConsoleColor" /> enum name.
    /// </param>
    public static void WriteSuccess(string message, Config? config = null)
    {
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor successColor = ConsoleColor.Green; // Default success color

        if (config is not null)
        {
            var successColorObj = config.Get("success_color", "Green"); // Default to "Green" if not found
            if (successColorObj != null &&
                Enum.TryParse(successColorObj.ToString(), true, out ConsoleColor parsedSuccessColor))
                successColor = parsedSuccessColor;
        }

        Console.ForegroundColor = successColor;
        Console.WriteLine(message);
        Console.ForegroundColor = originalFg; // Reset to original foreground color
    }

    /// <summary>
    ///     Displays a message prompting the user to press any key to continue and waits for a key press.
    /// </summary>
    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("\nPress any key to continue ...");
        Console.ReadKey(true); // true to not display the pressed key
    }

    /// <summary>
    ///     Reads a line of input from the console with support for validation (non-empty) and cancellation.
    /// </summary>
    /// <param name="readLineModel">
    ///     A <see cref="ReadLineModel" /> object containing the prompt question,
    ///     a flag indicating if empty input is allowed, and a default value to return on cancellation.
    /// </param>
    /// <returns>The trimmed user input, or the <see cref="ReadLineModel.value" /> if the user cancels (by entering 'X').</returns>
    public static string ReadLine(ReadLineModel readLineModel)
    {
        string? input; // Nullable to handle Console.ReadLine()
        while (true)
        {
            // Display the question and cancellation hint
            Console.WriteLine($"{readLineModel.Question} (Enter 'X' to cancel with default value)");
            input = Console.ReadLine();

            // Handle cancellation: if user types "X" (case-insensitive)
            if (input != null && input.Trim().Equals("X", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Cancelled. Using default value: {readLineModel.Value}");
                return readLineModel.Value; // Return the predefined default/cancel value
            }

            // Validate if empty input is allowed
            if (!readLineModel.AllowedEmpty && string.IsNullOrWhiteSpace(input))
            {
                WriteError("Invalid input! Please enter a non-empty value."); // Use WriteError for consistency
                continue; // Re-prompt the user
            }

            // If input is valid (or allowed to be empty and is empty/whitespace), break the loop
            break;
        }

        // Return the trimmed input, or an empty string if input was null (e.g. redirected EOF)
        return input?.Trim() ?? string.Empty;
    }
}