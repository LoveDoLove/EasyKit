using CommonUtilities.Config;

namespace EasyKit.Services;

public class ConsoleService
{
    /// <summary>
    ///     ConsoleService constructor using the new Config class.
    /// </summary>
    public ConsoleService(Config config)
    {
        Config = config;
    }

    // Add a public property to access the config
    public Config Config { get; }

    public void WriteInfo(string message)
    {
        // Get text color from config
        var textColorObj = Config.Get("text_color", "");
        var bgColorObj = Config.Get("background_color", "");

        // If specific colors are set, use them
        ConsoleColor? textColor = null;
        ConsoleColor? bgColor = null;
        if (textColorObj != null && Enum.TryParse(textColorObj.ToString(), true, out ConsoleColor parsedTextColor))
            textColor = parsedTextColor;
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor parsedBgColor))
            bgColor = parsedBgColor;

        // Force visible text if both are black
        if (textColor == ConsoleColor.Black && bgColor == ConsoleColor.Black)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        else
        {
            if (textColor.HasValue)
            {
                Console.ForegroundColor = textColor.Value;
            }
            else
            {
                var schemeObj = Config.Get("color_scheme", "dark");
                string scheme = schemeObj?.ToString() ?? "dark";
                Console.ForegroundColor = scheme == "light" ? ConsoleColor.Black : ConsoleColor.White;
            }

            if (bgColor.HasValue)
                Console.BackgroundColor = bgColor.Value;
        }

        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteError(string message)
    {
        // Get error text color from config or default to red
        var errorColorObj = Config.Get("error_color", "Red");
        if (errorColorObj != null && Enum.TryParse(errorColorObj.ToString(), true, out ConsoleColor errorColor))
            Console.ForegroundColor = errorColor;
        else
            Console.ForegroundColor = ConsoleColor.Red;

        // Use background color if specified
        var bgColorObj = Config.Get("background_color", "");
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor bgColor))
            Console.BackgroundColor = bgColor;

        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteSuccess(string message)
    {
        // Get success text color from config or default to green
        var successColorObj = Config.Get("success_color", "Green");
        if (successColorObj != null && Enum.TryParse(successColorObj.ToString(), true, out ConsoleColor successColor))
            Console.ForegroundColor = successColor;
        else
            Console.ForegroundColor = ConsoleColor.Green;

        // Use background color if specified
        var bgColorObj = Config.Get("background_color", "");
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor bgColor))
            Console.BackgroundColor = bgColor;

        Console.WriteLine(message);
        Console.ResetColor();
    }
}