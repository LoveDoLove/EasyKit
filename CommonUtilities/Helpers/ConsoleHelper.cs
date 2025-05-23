using CommonUtilities.Config;

namespace CommonUtilities.Helpers;

/// <summary>
/// Provides static helpers for console input/output, including config-driven color output.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Writes an info message to the console, using config for color scheme if provided.
    /// </summary>
    public static void WriteInfo(string message, CommonUtilities.Config.Config? config = null)
    {
        ConsoleColor? textColor = null;
        ConsoleColor? bgColor = null;

        if (config is not null)
        {
            var textColorObj = config.Get("text_color", "");
            var bgColorObj = config.Get("background_color", "");

            if (textColorObj != null && Enum.TryParse(textColorObj.ToString(), true, out ConsoleColor parsedTextColor))
                textColor = parsedTextColor;
            if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor parsedBgColor))
                bgColor = parsedBgColor;
        }

        // Force visible text if both are black
        if (textColor == ConsoleColor.Black && bgColor == ConsoleColor.Black)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        else
        {
            if (textColor.HasValue)
                Console.ForegroundColor = textColor.Value;
            else
                Console.ForegroundColor = ConsoleColor.White;

            if (bgColor.HasValue)
                Console.BackgroundColor = bgColor.Value;
        }

        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes an error message to the console, using config for color scheme if provided.
    /// </summary>
    public static void WriteError(string message, CommonUtilities.Config.Config? config = null)
    {
        ConsoleColor errorColor = ConsoleColor.Red;
        if (config is not null)
        {
            var errorColorObj = config.Get("error_color", "Red");
            if (errorColorObj != null && Enum.TryParse(errorColorObj.ToString(), true, out ConsoleColor parsedErrorColor))
                errorColor = parsedErrorColor;
        }
        Console.ForegroundColor = errorColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a success message to the console, using config for color scheme if provided.
    /// </summary>
    public static void WriteSuccess(string message, CommonUtilities.Config.Config? config = null)
    {
        ConsoleColor successColor = ConsoleColor.Green;
        if (config is not null)
        {
            var successColorObj = config.Get("success_color", "Green");
            if (successColorObj != null && Enum.TryParse(successColorObj.ToString(), true, out ConsoleColor parsedSuccessColor))
                successColor = parsedSuccessColor;
        }
        Console.ForegroundColor = successColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Prompts the user to press any key to continue.
    /// </summary>
    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("\nPress any key to continue ...");
        Console.ReadKey();
    }

    /// <summary>
    /// Reads a line from the console with validation and cancellation support.
    /// </summary>
    public static string ReadLine(ReadLineModel readLineModel)
    {
        string input;
        while (true)
        {
            Console.WriteLine($"{readLineModel.question} (Enter X = Cancel)");
            input = Console.ReadLine() ?? string.Empty;

            if (!readLineModel.allowedEmpty && string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input! Please enter a non-empty value.");
                continue;
            }

            if (input.ToUpper().Equals("X")) return readLineModel.value;

            break;
        }

        return input.Trim();
    }
}