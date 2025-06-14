namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Provides functionality to display a formatted help view in the console.
/// </summary>
public class HelpView
{
    private readonly ConsoleColor _borderColor;

    // A list of help topics and their descriptions
    private readonly List<(string Topic, string Description)> _helpItems = new();
    private readonly ConsoleColor _textColor;
    private readonly ConsoleColor _titleColor;
    private readonly int _width;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HelpView" /> class.
    /// </summary>
    /// <param name="width">The width of the help view box. Defaults to 60 characters.</param>
    /// <param name="borderColor">The color of the border. Defaults to DarkCyan.</param>
    /// <param name="titleColor">The color of the title text. Defaults to Yellow.</param>
    /// <param name="textColor">The color of the help item text. Defaults to White.</param>
    public HelpView(int width = 60,
        ConsoleColor borderColor = ConsoleColor.DarkCyan,
        ConsoleColor titleColor = ConsoleColor.Yellow,
        ConsoleColor textColor = ConsoleColor.White)
    {
        _width = width;
        _borderColor = borderColor;
        _titleColor = titleColor;
        _textColor = textColor;
    }

    /// <summary>
    ///     Adds a help item to the view.
    /// </summary>
    /// <param name="topic">The topic or command name.</param>
    /// <param name="description">A brief description of the topic or command.</param>
    /// <returns>The current <see cref="HelpView" /> instance for fluent chaining.</returns>
    public HelpView AddHelpItem(string topic, string description)
    {
        _helpItems.Add((topic, description));
        return this;
    }

    /// <summary>
    ///     Displays the help view in the console.
    /// </summary>
    /// <param name="title">The title of the help view. Defaults to "Help".</param>
    public void Show(string title = "Help")
    {
        // Save current cursor position to restore it later
        int originalLeft = Console.CursorLeft;
        int originalTop = Console.CursorTop;

        // Calculate the height and position for centering the help box
        int helpHeight = _helpItems.Count + 4; // Includes title, border, and items
        int leftPosition = (Console.WindowWidth - _width) / 2;
        int topPosition = (Console.WindowHeight - helpHeight) / 2;

        // Store original console colors to restore them later
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        // Draw the top border of the help box
        Console.SetCursorPosition(leftPosition, topPosition);
        Console.ForegroundColor = _borderColor;
        Console.Write("┌" + new string('─', _width - 2) + "┐");

        // Draw the title section
        Console.SetCursorPosition(leftPosition, topPosition + 1);
        Console.Write("│"); // Left border
        Console.ForegroundColor = _titleColor;
        Console.Write(title.PadRight(_width - 2)); // Padded title text
        Console.ForegroundColor = _borderColor;
        Console.Write("│"); // Right border

        // Draw the separator line below the title
        Console.SetCursorPosition(leftPosition, topPosition + 2);
        Console.Write("├" + new string('─', _width - 2) + "┤");

        // Draw each help item
        for (int i = 0; i < _helpItems.Count; i++)
        {
            Console.SetCursorPosition(leftPosition, topPosition + 3 + i);
            Console.Write("│ "); // Left border and padding
            Console.ForegroundColor = _titleColor;
            Console.Write(_helpItems[i].Topic.PadRight(15)); // Topic, padded
            Console.ForegroundColor = _textColor;

            // Truncate description if it's too long to fit
            string desc = _helpItems[i].Description;
            int maxDescLength = _width - 20; // Max length for description (topic padding + borders + spaces)
            if (desc.Length > maxDescLength)
                desc = desc.Substring(0, maxDescLength - 3) + "..."; // Truncate and add ellipsis

            Console.Write(desc.PadRight(_width - 19)); // Description, padded
            Console.ForegroundColor = _borderColor;
            Console.Write(" │"); // Right border and padding
        }

        // Draw the bottom border of the help box
        Console.SetCursorPosition(leftPosition, topPosition + 3 + _helpItems.Count);
        Console.Write("└" + new string('─', _width - 2) + "┘");

        // Add footer instruction text
        string footerText = "Press any key to continue";
        Console.SetCursorPosition(leftPosition + (_width - footerText.Length) / 2, topPosition + 4 + _helpItems.Count);
        Console.ForegroundColor = _textColor;
        Console.Write(footerText);

        // Reset console colors to their original values
        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;

        // Wait for a key press before clearing the help box
        Console.ReadKey(true);

        // Clear the area occupied by the help box
        for (int i = 0; i <= helpHeight + 1; i++) // Iterate through each line of the help box + footer
        {
            Console.SetCursorPosition(leftPosition, topPosition + i);
            Console.Write(new string(' ', _width)); // Overwrite with spaces
        }

        // Restore the original cursor position
        Console.SetCursorPosition(originalLeft, originalTop);
    }

    /// <summary>
    ///     Shows context-sensitive help based on the current menu.
    ///     <remarks>
    ///         This method is intended to be called when a specific help key (e.g., F1) is pressed.
    ///         The implementation for this is pending.
    ///     </remarks>
    /// </summary>
    /// <param name="menuBuilder">The <see cref="MenuBuilder" /> instance representing the current menu context.</param>
    public static void ShowContextHelp(MenuBuilder menuBuilder)
    {
        // Implement context-sensitive help based on the current menu
        // This will be called when F1 is pressed
        // TODO: Implement this in the future
    }
}