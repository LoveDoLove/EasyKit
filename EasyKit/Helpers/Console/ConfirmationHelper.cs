namespace EasyKit.Helpers.Console;

/// <summary>
///     Provides confirmation prompts for actions and admin elevation.
/// </summary>
public class ConfirmationHelper
{
    /// <summary>
    ///     ConfirmationService constructor using the new Config class.
    /// </summary>
    public ConfirmationHelper()
    {
    }

    /// <summary>
    ///     Prompts the user to confirm an action.
    /// </summary>
    public bool ConfirmAction(string message, bool defaultYes = true)
    {
        return ConfirmYesNo(message, defaultYes);
    }

    /// <summary>
    ///     Prompts the user for a yes/no confirmation.
    /// </summary>
    private bool ConfirmYesNo(string message, bool defaultYes = true)
    {
        System.Console.Write($"{message} [{(defaultYes ? "Y/n" : "y/N")}]: ");
        var input = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            return defaultYes;
        input = input.Trim().ToLower();
        return defaultYes
            ? !(input == "n" || input == "no")
            : input == "y" || input == "yes";
    }
}