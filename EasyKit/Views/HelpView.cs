namespace EasyKit.Views;

public class HelpView
{
    private readonly ConsoleColor _borderColor;

    // A list of help topics and their descriptions
    private readonly List<(string Topic, string Description)> _helpItems = new();
    private readonly ConsoleColor _textColor;
    private readonly ConsoleColor _titleColor;
    private readonly int _width;

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

    public HelpView AddHelpItem(string topic, string description)
    {
        _helpItems.Add((topic, description));
        return this;
    }

    public void Show(string title = "Help")
    {
        // Save current cursor position
        int originalLeft = Console.CursorLeft;
        int originalTop = Console.CursorTop;

        // Calculate size and position
        int helpHeight = _helpItems.Count + 4; // Title + Border + items
        int leftPosition = (Console.WindowWidth - _width) / 2;
        int topPosition = (Console.WindowHeight - helpHeight) / 2;

        // Store original colors
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        // Draw the help box
        Console.SetCursorPosition(leftPosition, topPosition);
        Console.ForegroundColor = _borderColor;
        Console.Write("┌" + new string('─', _width - 2) + "┐");

        // Draw title
        Console.SetCursorPosition(leftPosition, topPosition + 1);
        Console.Write("│");
        Console.ForegroundColor = _titleColor;
        Console.Write(title.PadRight(_width - 2));
        Console.ForegroundColor = _borderColor;
        Console.Write("│");

        // Draw separator
        Console.SetCursorPosition(leftPosition, topPosition + 2);
        Console.Write("├" + new string('─', _width - 2) + "┤");

        // Draw help items
        for (int i = 0; i < _helpItems.Count; i++)
        {
            Console.SetCursorPosition(leftPosition, topPosition + 3 + i);
            Console.Write("│ ");
            Console.ForegroundColor = _titleColor;
            Console.Write(_helpItems[i].Topic.PadRight(15));
            Console.ForegroundColor = _textColor;

            // Format the description to fit within the box
            string desc = _helpItems[i].Description;
            if (desc.Length > _width - 20) desc = desc.Substring(0, _width - 23) + "...";

            Console.Write(desc.PadRight(_width - 19));
            Console.ForegroundColor = _borderColor;
            Console.Write(" │");
        }

        // Draw bottom border
        Console.SetCursorPosition(leftPosition, topPosition + 3 + _helpItems.Count);
        Console.Write("└" + new string('─', _width - 2) + "┘");

        // Add footer instruction
        Console.SetCursorPosition(leftPosition + (_width - 22) / 2, topPosition + 4 + _helpItems.Count);
        Console.ForegroundColor = _textColor;
        Console.Write("Press any key to continue");

        // Reset colors
        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;

        // Wait for key press
        Console.ReadKey(true);

        // Clear the help box
        for (int i = 0; i <= helpHeight + 1; i++)
        {
            Console.SetCursorPosition(leftPosition, topPosition + i);
            Console.Write(new string(' ', _width));
        }

        // Restore cursor position
        Console.SetCursorPosition(originalLeft, originalTop);
    }

    public static void ShowContextHelp(MenuBuilder menuBuilder)
    {
        // Implement context-sensitive help based on the current menu
        // This will be called when F1 is pressed
        // TODO: Implement this in the future
    }
}