using CommonUtilities.Models.Core;
using CommonUtilities.UI.ConsoleUI;
using EasyKit.Models;
using EasyKit.Services;

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
            .AddOption("2", "Change menu width", () => ChangeMenuWidth())
            .AddOption("3", "Toggle logging", () => ToggleSetting("enable_logging"))
            .AddOption("4", "Toggle tips", () => ToggleSetting("show_tips"))
            .AddOption("5", "Toggle exit confirmation", () => ToggleSetting("confirm_exit"))
            .AddOption("6", "Toggle destructive action confirmations",
                () => ToggleSetting("confirm_destructive_actions"))
            .AddOption("7", "Reset all settings to defaults", () => ResetToDefaults())
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