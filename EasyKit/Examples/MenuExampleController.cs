namespace EasyKit.Examples;

// This is an example class showing how to use the new MenuBuilder system
public class MenuExampleController
{
    private readonly Config _config;
    private readonly ConsoleService _console;
    private readonly MenuView _menuView;

    public MenuExampleController(Config config, ConsoleService console, MenuView menuView)
    {
        _config = config;
        _console = console;
        _menuView = menuView;
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = _config.Get("menu_width", 50) is int mw ? mw : 50;
        string colorScheme = _config.Get("color_scheme", "dark")?.ToString() ?? "dark";

        // Apply the appropriate color scheme based on user settings
        var (border, highlight, title, text, help) = colorScheme.ToLower() == "light"
            ? MenuTheme.ColorScheme.Light
            : MenuTheme.ColorScheme.Dark;

        // Create a menu with fluent configuration
        _menuView.CreateMenu("Example Menu", "1.0", menuWidth)
            // Add menu options with handlers
            .AddOption("1", "Option One", () => HandleOption1())
            .AddOption("2", "Option Two", () => HandleOption2())
            .AddOption("3", "Option Three", () => HandleOption3())
            .AddOption("0", "Back to Main Menu", () =>
            {
                /* Return to previous menu */
            })
            // Customize the appearance
            .WithColors(border, highlight, title, text, help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithSubtitle("This is an example of what you can do with MenuBuilder")
            // Use rounded borders for a modern look
            .WithRoundedBorder()
            // Show the menu and handle user input
            .Show();
    }

    private void HandleOption1()
    {
        _console.WriteInfo("You selected Option One!");
        Console.ReadLine();
    }

    private void HandleOption2()
    {
        _console.WriteInfo("You selected Option Two!");
        Console.ReadLine();
    }

    private void HandleOption3()
    {
        _console.WriteInfo("You selected Option Three!");
        Console.ReadLine();
    }
}