namespace EasyKit.Views;

public class MenuBuilder
{
    private readonly List<MenuOption> _options = new();
    private readonly List<KeyboardShortcut> _shortcuts = new();
    private readonly string _title;
    private readonly string _version;
    private readonly int _width;
    private ConsoleColor _borderColor = ConsoleColor.DarkCyan;
    private string _bottomLeftCorner = "└";
    private string _bottomRightCorner = "┘";
    private bool _centerVertically = true;
    private ConsoleColor _helpColor = ConsoleColor.DarkGray;
    private string _helpText = "Use number keys to select an option. Press 0 to exit.";
    private ConsoleColor _highlightColor = ConsoleColor.Cyan;
    private string _horizontalBorder = "─";
    private bool _showKeyboardShortcutsHelp;
    private string? _subtitle;
    private ConsoleColor _textColor = ConsoleColor.White;
    private ConsoleColor _titleColor = ConsoleColor.Yellow;
    private string _topLeftCorner = "┌";
    private string _topRightCorner = "┐";
    private string _verticalBorder = "│";

    public MenuBuilder(string title, int width = 50, string? version = null)
    {
        _title = title;
        _width = width;
        _version = version ?? string.Empty;
    }

    public MenuBuilder AddOption(string key, string text, Action handler)
    {
        _options.Add(new MenuOption(key, text, handler));
        return this;
    }

    public MenuBuilder AddOption(MenuOption option)
    {
        _options.Add(option);
        return this;
    }

    public MenuBuilder WithHelpText(string helpText)
    {
        _helpText = helpText;
        return this;
    }

    public MenuBuilder WithSubtitle(string subtitle)
    {
        _subtitle = subtitle;
        return this;
    }

    public MenuBuilder WithColors(
        ConsoleColor borderColor = ConsoleColor.DarkCyan,
        ConsoleColor highlightColor = ConsoleColor.Cyan,
        ConsoleColor titleColor = ConsoleColor.Yellow,
        ConsoleColor textColor = ConsoleColor.White,
        ConsoleColor helpColor = ConsoleColor.DarkGray)
    {
        _borderColor = borderColor;
        _highlightColor = highlightColor;
        _titleColor = titleColor;
        _textColor = textColor;
        _helpColor = helpColor;
        return this;
    }

    public MenuBuilder WithCenterVertically(bool centerVertically)
    {
        _centerVertically = centerVertically;
        return this;
    }

    public MenuBuilder WithBorderStyle(
        string topLeftCorner = "┌",
        string topRightCorner = "┐",
        string bottomLeftCorner = "└",
        string bottomRightCorner = "┘",
        string horizontalBorder = "─",
        string verticalBorder = "│")
    {
        _topLeftCorner = topLeftCorner;
        _topRightCorner = topRightCorner;
        _bottomLeftCorner = bottomLeftCorner;
        _bottomRightCorner = bottomRightCorner;
        _horizontalBorder = horizontalBorder;
        _verticalBorder = verticalBorder;
        return this;
    }

    public MenuBuilder WithDoubleBorder()
    {
        return WithBorderStyle("╔", "╗", "╚", "╝", "═", "║");
    }

    public MenuBuilder WithRoundedBorder()
    {
        return WithBorderStyle("╭", "╮", "╰", "╯");
    }

    public MenuBuilder AddShortcut(ConsoleKey key, string description, Action handler)
    {
        _shortcuts.Add(new KeyboardShortcut(key, description, handler));
        _showKeyboardShortcutsHelp = true;
        return this;
    }

    public MenuBuilder ShowKeyboardShortcutsHelp(bool show)
    {
        _showKeyboardShortcutsHelp = show;
        return this;
    }

    public void Show()
    {
        Console.Clear();
        DrawHeader();
        HandleInput();
    }

    private void DrawHeader()
    {
        var titleText = !string.IsNullOrEmpty(_version)
            ? $"{_title} v{_version}"
            : _title;

        // Draw Title
        string line = new string(_horizontalBorder[0], _width);
        Console.ForegroundColor = _titleColor;
        Console.WriteLine(line);

        Console.ForegroundColor = _textColor;
        Console.WriteLine(titleText.PadLeft((_width + titleText.Length) / 2).PadRight(_width));

        // Draw current directory below the title
        Console.ForegroundColor = ConsoleColor.Yellow;
        string currentDir = Directory.GetCurrentDirectory();
        Console.WriteLine(("Current Directory: " + currentDir)
            .PadLeft((_width + ("Current Directory: " + currentDir).Length) / 2).PadRight(_width));
        Console.ResetColor();

        // Draw subtitle if provided
        if (!string.IsNullOrEmpty(_subtitle))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine(_subtitle.PadLeft((_width + _subtitle.Length) / 2).PadRight(_width));
            Console.ResetColor();
        }

        Console.ForegroundColor = _titleColor;
        Console.WriteLine(line);
        Console.ResetColor();
    }

    private void DrawOptions()
    {
        var displayOptions = _options.Select(o => $"{o.Key}. {o.Text}").ToArray();

        // Center menu vertically if requested
        if (_centerVertically)
        {
            int topPadding = (Console.WindowHeight - displayOptions.Length - 10) / 2;
            topPadding = Math.Max(0, topPadding); // Prevent negative padding
            for (int i = 0; i < topPadding; i++) Console.WriteLine();
        }

        // Draw menu with highlight and box
        string border = new string(_horizontalBorder[0], _width - 2);
        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_topLeftCorner}{border}{_topRightCorner}");
        Console.ResetColor();

        foreach (var option in displayOptions)
        {
            string padded = option.PadRight(_width - 4);
            Console.ForegroundColor = _highlightColor;
            Console.Write($"{_verticalBorder} ");
            Console.ResetColor();

            Console.ForegroundColor = _textColor;
            Console.Write(padded);
            Console.ResetColor();

            Console.ForegroundColor = _highlightColor;
            Console.WriteLine($" {_verticalBorder}");
            Console.ResetColor();
        }

        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_bottomLeftCorner}{border}{_bottomRightCorner}");
        Console.ResetColor();

        // Add help text
        if (!string.IsNullOrEmpty(_helpText))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine($"\n{_helpText}");
            Console.ResetColor();
        }

        Console.Write("\nChoose an option: ");
    }

    private void ShowHelp()
    {
        var helpView = new HelpView(_width, _borderColor, _titleColor, _textColor);

        // Add general navigation help
        helpView.AddHelpItem("Navigation", "Use arrow keys (↑/↓) or number keys to navigate");
        helpView.AddHelpItem("Selection", "Press Enter to select an option");
        helpView.AddHelpItem("Exit", "Press Esc or 0 to return to previous menu");
        helpView.AddHelpItem("Help", "Press F1 anytime to show this help");

        // Add all keyboard shortcuts
        if (_shortcuts.Count > 0)
            foreach (var shortcut in _shortcuts)
                helpView.AddHelpItem(shortcut.Key.ToString(), shortcut.Description);

        // Show the help view
        helpView.Show($"{_title} - Help");
    }

    private void HandleInput()
    {
        int selectedIndex = 0;

        // Initial draw with selection
        Console.Clear();
        DrawHeader();
        DrawOptionsWithSelection(selectedIndex);

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Handle number keys (traditional navigation)
            if (char.IsDigit(keyInfo.KeyChar))
            {
                string input = keyInfo.KeyChar.ToString();
                var option = _options.FirstOrDefault(o => o.Key.Equals(input, StringComparison.OrdinalIgnoreCase));

                if (option != null)
                {
                    // Clear the current line
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);

                    option.Handler();

                    // If this was the exit option (usually "0"), we don't redraw
                    if (option.Key == "0") return;

                    // Otherwise, redraw the menu
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Press any key to continue.");
                    Console.ResetColor();
                    Console.ReadKey(true);

                    // Redraw the menu
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex);
                }

                continue;
            }

            // Handle arrow key navigation
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                    DrawOptionsWithSelection(selectedIndex);
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = Math.Min(_options.Count - 1, selectedIndex + 1);
                    DrawOptionsWithSelection(selectedIndex);
                    break;

                case ConsoleKey.Enter:
                    if (selectedIndex >= 0 && selectedIndex < _options.Count)
                    {
                        _options[selectedIndex].Handler();

                        // If this was the exit option (usually "0"), we don't redraw
                        if (_options[selectedIndex].Key == "0") return;

                        // Otherwise, redraw the menu
                        Console.Clear();
                        DrawHeader();
                        DrawOptionsWithSelection(selectedIndex);
                    }

                    break;
                case ConsoleKey.Escape:
                    // Find the "Back" or "0" option and execute it
                    var exitOption = _options.FirstOrDefault(o => o.Key == "0");
                    if (exitOption != null)
                    {
                        exitOption.Handler();
                        return;
                    }

                    break;

                case ConsoleKey.F1:
                    // Show help for this menu
                    ShowHelp();
                    // Redraw the menu after showing help
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex);
                    break;

                default:
                    // Check if this key matches any registered shortcuts
                    var shortcut = _shortcuts.FirstOrDefault(s => s.Key == keyInfo.Key);
                    if (shortcut != null)
                    {
                        shortcut.Handler();

                        // Redraw the menu after shortcut execution
                        Console.Clear();
                        DrawHeader();
                        DrawOptionsWithSelection(selectedIndex);
                    }

                    break;
            }
        }
    }

    private void DrawOptionsWithSelection(int selectedIndex)
    {
        var displayOptions = _options.Select(o => $"{o.Key}. {o.Text}").ToArray();

        // Center menu vertically if requested
        if (_centerVertically)
        {
            // Clear existing content
            Console.Clear();
            DrawHeader();

            int topPadding = (Console.WindowHeight - displayOptions.Length - 10) / 2;
            topPadding = Math.Max(0, topPadding); // Prevent negative padding
            for (int i = 0; i < topPadding; i++) Console.WriteLine();
        }

        // Draw menu with highlight and box
        string border = new string(_horizontalBorder[0], _width - 2);
        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_topLeftCorner}{border}{_topRightCorner}");
        Console.ResetColor();

        for (int i = 0; i < displayOptions.Length; i++)
        {
            string option = displayOptions[i];
            string padded = option.PadRight(_width - 4);
            Console.ForegroundColor = _borderColor;
            Console.Write($"{_verticalBorder} ");
            Console.ResetColor();

            // Highlight the selected option
            if (i == selectedIndex)
            {
                Console.BackgroundColor = _highlightColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(padded);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = _textColor;
                Console.Write(padded);
                Console.ResetColor();
            }

            Console.ForegroundColor = _borderColor;
            Console.WriteLine($" {_verticalBorder}");
            Console.ResetColor();
        }

        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_bottomLeftCorner}{border}{_bottomRightCorner}");
        Console.ResetColor(); // Add help text
        if (!string.IsNullOrEmpty(_helpText))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine($"\n{_helpText}");
            Console.ResetColor();
        }

        // Add shortcuts help if enabled
        if (_showKeyboardShortcutsHelp && _shortcuts.Count > 0)
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine("\nKeyboard Shortcuts:");
            foreach (var shortcut in _shortcuts) Console.WriteLine($"  {shortcut.Key} - {shortcut.Description}");
            Console.ResetColor();
        }
    }
}

public class MenuOption
{
    public MenuOption(string key, string text, Action handler)
    {
        Key = key;
        Text = text;
        Handler = handler;
    }

    public string Key { get; }
    public string Text { get; }
    public Action Handler { get; }
}