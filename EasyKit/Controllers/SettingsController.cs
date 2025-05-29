using CommonUtilities.Config;
using CommonUtilities.Utilities;

namespace EasyKit.Controllers;

internal class SettingsController
{
    private readonly Config _config;
    private readonly ConsoleService _console;
    private readonly PromptView _prompt;

    /// <summary>
    ///     SettingsController constructor using the new Config class.
    /// </summary>
    public SettingsController(Config config, ConsoleService console, PromptView prompt)
    {
        _config = config;
        _console = console;
        _prompt = prompt;
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 50;
        string colorSchemeStr = "dark";

        // Try to get user preferences from config
        var menuWidthObj = _config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        var colorSchemeObj = _config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorSchemeStr = colorSchemeObj.ToString() ?? "dark";

        // Apply the appropriate color scheme based on user settings
        var colorScheme = MenuTheme.ColorScheme.Dark;
        if (colorSchemeStr.ToLower() == "light")
            colorScheme = MenuTheme.ColorScheme.Light;

        // Create and configure the menu for Settings
        var menuView = new MenuView();
        menuView.CreateMenu("Settings", width: menuWidth)
            .AddOption("1", "View current configuration", () => ViewConfig())
            .AddOption("2", "Change console colors", () => ChangeColorScheme())
            .AddOption("3", "Change error message color", () => ChangeSpecificColor("error_color", "Red"))
            .AddOption("4", "Change success message color", () => ChangeSpecificColor("success_color", "Green"))
            .AddOption("5", "Toggle logging", () => ToggleSetting("enable_logging"))
            .AddOption("6", "Toggle tips", () => ToggleSetting("show_tips"))
            .AddOption("7", "Toggle exit confirmation", () => ToggleSetting("confirm_exit"))
            .AddOption("8", "Toggle destructive action confirmations",
                () => ToggleSetting("confirm_destructive_actions"))
            .AddOption("9", "Change menu width", () => ChangeMenuWidth())
            .AddOption("R", "Reset all settings to defaults", () => ResetToDefaults())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White)
            .WithHelpText("Select an option or press 0 to return to the main menu. Press R to reset all settings.")
            .WithRoundedBorder()
            .Show();
    }

    private void ViewConfig()
    {
        _console.WriteInfo("Current Configuration:");
        foreach (var kv in _config.Settings)
            _console.WriteInfo($"{kv.Key}: {kv.Value}");
        Console.ReadLine();
    }

    private void ChangeColorScheme()
    {
        _console.WriteInfo("Color Settings");
        _console.WriteInfo("=============");

        // Display all available console colors for selection
        _console.WriteInfo("\nAvailable Text Colors:");
        foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
        {
            Console.ForegroundColor = color;
            Console.Write($"{(int)color}. {color}  ");
        }

        Console.ResetColor();

        _console.WriteInfo("\n\nAvailable Background Colors:");
        foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
        {
            Console.BackgroundColor = color;
            Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write($"{(int)color}. {color}  ");
        }

        Console.ResetColor();

        _console.WriteInfo("\n");

        // Get current color settings
        var currentScheme = _config.Get("color_scheme", "dark")?.ToString() ?? "dark";
        var currentTextColor = _config.Get("text_color", "White")?.ToString() ?? "White";
        var currentBgColor = _config.Get("background_color", "Black")?.ToString() ?? "Black";

        _console.WriteInfo(
            $"Current settings: Scheme={currentScheme}, Text={currentTextColor}, Background={currentBgColor}");

        // Get text color
        var textColorInput = _prompt.Prompt("\nEnter text color number or name (leave empty to keep current): ");
        ConsoleColor textColor;

        if (!string.IsNullOrWhiteSpace(textColorInput))
        {
            if (int.TryParse(textColorInput, out int textColorNum) &&
                Enum.IsDefined(typeof(ConsoleColor), textColorNum))
            {
                textColor = (ConsoleColor)textColorNum;
            }
            else if (Enum.TryParse(textColorInput, true, out textColor))
            {
                // Successfully parsed by name
            }
            else
            {
                _console.WriteError("Invalid text color. Setting not changed.");
                Console.ReadLine();
                return;
            }

            _config.Settings["text_color"] = textColor.ToString();
        }

        // Get background color
        var bgColorInput = _prompt.Prompt("Enter background color number or name (leave empty to keep current): ");
        ConsoleColor bgColor;

        if (!string.IsNullOrWhiteSpace(bgColorInput))
        {
            if (int.TryParse(bgColorInput, out int bgColorNum) &&
                Enum.IsDefined(typeof(ConsoleColor), bgColorNum))
            {
                bgColor = (ConsoleColor)bgColorNum;
            }
            else if (Enum.TryParse(bgColorInput, true, out bgColor))
            {
                // Successfully parsed by name
            }
            else
            {
                _console.WriteError("Invalid background color. Setting not changed.");
                Console.ReadLine();
                return;
            }

            _config.Settings["background_color"] = bgColor.ToString();
        }

        // Check if colors would be the same and hard to read
        var finalTextColor = _config.Get("text_color", "White")?.ToString() ?? "White";
        var finalBgColor = _config.Get("background_color", "Black")?.ToString() ?? "Black";

        if (finalTextColor.Equals(finalBgColor, StringComparison.OrdinalIgnoreCase))
        {
            _console.WriteError("Warning: Text and background colors are the same. This will be difficult to read.");
            var confirm = _prompt.Prompt("Continue anyway? (y/n): ");
            if (confirm?.ToLower() != "y")
            {
                _console.WriteInfo("Color change canceled.");
                Console.ReadLine();
                return;
            }
        }

        // Deprecated but keep for backward compatibility
        var schemeInput = _prompt.Prompt("Choose scheme type (dark/light) for menu colors: ");
        if (!string.IsNullOrWhiteSpace(schemeInput) &&
            (schemeInput.ToLower() == "dark" || schemeInput.ToLower() == "light"))
            _config.Settings["color_scheme"] = schemeInput.ToLower();

        // Save all settings
        _config.SaveConfig();

        // Preview the new colors
        Console.WriteLine();

        ConsoleColor previewTextColor;
        if (Enum.TryParse(_config.Get("text_color", "White")?.ToString() ?? "White", true, out previewTextColor))
            Console.ForegroundColor = previewTextColor;
        else
            Console.ForegroundColor = ConsoleColor.White;

        ConsoleColor previewBgColor;
        if (Enum.TryParse(_config.Get("background_color", "Black")?.ToString() ?? "Black", true, out previewBgColor))
            Console.BackgroundColor = previewBgColor;
        else
            Console.BackgroundColor = ConsoleColor.Black;

        Console.WriteLine("This is a preview of your new color settings.");
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();

        _console.WriteSuccess("Color settings updated successfully.");
        Console.ReadLine();
    }

    private void ChangeSpecificColor(string colorSetting, string defaultColor)
    {
        _console.WriteInfo($"Change {colorSetting} Color");
        _console.WriteInfo("======================");

        // Display all available console colors for selection
        _console.WriteInfo("\nAvailable Colors:");
        foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
        {
            Console.ForegroundColor = color;
            Console.Write($"{(int)color}. {color}  ");
        }

        Console.ResetColor();

        // Get current color setting
        var currentColor = _config.Get(colorSetting, defaultColor)?.ToString() ?? defaultColor;
        _console.WriteInfo($"\n\nCurrent {colorSetting}: {currentColor}");

        // Get color input
        var colorInput = _prompt.Prompt("\nEnter color number or name (leave empty to keep current): ");
        ConsoleColor selectedColor;

        if (!string.IsNullOrWhiteSpace(colorInput))
        {
            if (int.TryParse(colorInput, out int colorNum) &&
                Enum.IsDefined(typeof(ConsoleColor), colorNum))
            {
                selectedColor = (ConsoleColor)colorNum;
                _config.Settings[colorSetting] = selectedColor.ToString();
                _config.SaveConfig();

                // Preview the color
                Console.ForegroundColor = selectedColor;
                Console.WriteLine($"This is a preview of the new {colorSetting}.");
                Console.ResetColor();

                _console.WriteSuccess($"{colorSetting} updated to {selectedColor}.");
            }
            else if (Enum.TryParse(colorInput, true, out selectedColor))
            {
                _config.Settings[colorSetting] = selectedColor.ToString();
                _config.SaveConfig();

                // Preview the color
                Console.ForegroundColor = selectedColor;
                Console.WriteLine($"This is a preview of the new {colorSetting}.");
                Console.ResetColor();

                _console.WriteSuccess($"{colorSetting} updated to {selectedColor}.");
            }
            else
            {
                _console.WriteError("Invalid color. Setting not changed.");
            }
        }
        else
        {
            _console.WriteInfo("No changes made.");
        }

        Console.ReadLine();
    }

    private void ToggleSetting(string key)
    {
        if (_config.Settings.ContainsKey(key))
        {
            var current = _config.Settings[key] is bool b ? b : false;
            _config.Settings[key] = !current;
            _config.SaveConfig();
            _console.WriteSuccess($"{key} set to {!current}.");
        }
        else
        {
            _console.WriteError($"Setting {key} not found.");
        }

        Console.ReadLine();
    }

    private void ChangeMenuWidth()
    {
        var input = _prompt.Prompt("Enter menu width (20-120): ");
        if (int.TryParse(input, out int width) && width >= 20 && width <= 120)
        {
            _config.Settings["menu_width"] = width;
            _config.SaveConfig();
            _console.WriteSuccess($"Menu width set to {width}.");
        }
        else
        {
            _console.WriteError("Invalid width.");
        }

        Console.ReadLine();
    }

    /// <summary>
    ///     Resets all settings to their default values after user confirmation.
    /// </summary>
    private void ResetToDefaults()
    {
        var confirm = _prompt.Prompt("Are you sure you want to reset all settings to defaults? (y/n): ");
        if (confirm?.Trim().ToLower() == "y")
        {
            _config.ResetToDefaults();
            _console.WriteSuccess("All settings have been reset to default values.");
        }
        else
        {
            _console.WriteInfo("Reset to defaults canceled.");
        }

        Console.ReadLine();
    }
}