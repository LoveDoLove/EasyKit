namespace EasyKit.Examples;

public class MenuConversionExample
{
    private readonly Config _config;
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly MenuView _menuView;

    public MenuConversionExample(Config config, LoggerService logger, ConsoleService console, MenuView menuView)
    {
        _config = config;
        _logger = logger;
        _console = console;
        _menuView = menuView;
    }

    // Original style ShowMenu method
    public void ShowMenuOriginal()
    {
        Console.Clear();
        while (true)
        {
            _console.WriteInfo("Original Menu\n" +
                               "1. Option One\n" +
                               "2. Option Two\n" +
                               "3. Option Three\n" +
                               "0. Back to main menu\n");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": OptionOne(); break;
                case "2": OptionTwo(); break;
                case "3": OptionThree(); break;
                case "0": return;
                default:
                    _console.WriteError("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    // Converted to use MenuBuilder
    public void ShowMenuNew()
    {
        // Get user settings
        int menuWidth = _config.Get("menu_width", 50) is int mw ? mw : 50;
        string colorScheme = _config.Get("color_scheme", "dark")?.ToString() ?? "dark";

        // Create menu with fluent configuration
        _menuView.CreateMenu("Menu Example", width: menuWidth)
            // Menu options
            .AddOption("1", "Option One", () => OptionOne())
            .AddOption("2", "Option Two", () => OptionTwo())
            .AddOption("3", "Option Three", () => OptionThree())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to previous menu */
            })
            // Apply the appropriate color scheme based on user settings
            .WithColors(
                colorScheme.ToLower() == "light" ? ConsoleColor.DarkBlue : ConsoleColor.DarkCyan,
                colorScheme.ToLower() == "light" ? ConsoleColor.Blue : ConsoleColor.Cyan,
                ConsoleColor.Yellow,
                colorScheme.ToLower() == "light" ? ConsoleColor.Black : ConsoleColor.White)
            // Show the menu and handle user input
            .Show();
    }

    private void OptionOne()
    {
        _console.WriteInfo("You selected Option One!");
        Console.ReadLine();
    }

    private void OptionTwo()
    {
        _console.WriteInfo("You selected Option Two!");
        Console.ReadLine();
    }

    private void OptionThree()
    {
        _console.WriteInfo("You selected Option Three!");
        Console.ReadLine();
    }
}