namespace EasyKit.Services;

public class ConfirmationService
{
    private readonly Config? _config;
    private readonly ConsoleService? _console;
    private readonly PromptView _prompt = new();

    public ConfirmationService()
    {
        // Default constructor for backward compatibility
    }

    public ConfirmationService(ConsoleService console, Config config)
    {
        _console = console;
        _config = config;
    }

    public bool ConfirmAction(string message, bool defaultYes = true)
    {
        return _prompt.ConfirmYesNo(message, defaultYes);
    }

    public bool ConfirmDestructiveAction(string action, string details = "", bool defaultNo = true)
    {
        // Check if confirmations are enabled in config
        if (_config != null)
        {
            var confirmEnabled = _config.Get("confirm_destructive_actions", true);
            if (confirmEnabled is bool b && !b)
                // Confirmations disabled, proceed without asking
                return true;
        }

        if (_console != null)
        {
            _console.WriteInfo($"You are about to: {action}");

            if (!string.IsNullOrEmpty(details)) _console.WriteInfo(details);

            _console.WriteInfo(new string('-', 50));
        }
        else
        {
            Console.WriteLine($"You are about to: {action}");

            if (!string.IsNullOrEmpty(details)) Console.WriteLine(details);

            Console.WriteLine(new string('-', 50));
        }

        return _prompt.ConfirmYesNo("Are you sure you want to proceed?", !defaultNo);
    }

    public bool ConfirmAdminElevation(bool displayHeader = true)
    {
        if (displayHeader)
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║             ADMINISTRATOR RIGHTS REQUIRED             ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        Console.WriteLine("EasyKit requires administrator privileges to function properly.");
        Console.WriteLine("Some features may not work correctly without admin rights.");
        Console.WriteLine();

        return _prompt.ConfirmYesNo("Would you like to restart with admin rights?");
    }
}