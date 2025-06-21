// MIT License
// 
// Copyright (c) 2025 LoveDoLove
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Builds and displays interactive console menus.
/// </summary>
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

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuBuilder" /> class.
    /// </summary>
    /// <param name="title">The title of the menu.</param>
    /// <param name="width">The width of the menu in characters. Defaults to 50.</param>
    /// <param name="version">Optional version string to display with the title.</param>
    public MenuBuilder(string title, int width = 50, string? version = null)
    {
        _title = title;
        _width = width;
        _version = version ?? string.Empty;
    }

    /// <summary>
    ///     Adds a new option to the menu.
    /// </summary>
    /// <param name="key">The key used to select the option (e.g., "1", "A").</param>
    /// <param name="text">The text displayed for the option.</param>
    /// <param name="handler">The action to execute when the option is selected.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder AddOption(string key, string text, Action handler)
    {
        _options.Add(new MenuOption(key, text, handler));
        return this;
    }

    /// <summary>
    ///     Adds an existing <see cref="MenuOption" /> instance to the menu.
    /// </summary>
    /// <param name="option">The <see cref="MenuOption" /> to add.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder AddOption(MenuOption option)
    {
        _options.Add(option);
        return this;
    }

    /// <summary>
    ///     Sets the help text displayed below the menu options.
    /// </summary>
    /// <param name="helpText">The help text to display.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder WithHelpText(string helpText)
    {
        _helpText = helpText;
        return this;
    }

    /// <summary>
    ///     Sets an optional subtitle to be displayed below the main title.
    /// </summary>
    /// <param name="subtitle">The subtitle text.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder WithSubtitle(string subtitle)
    {
        _subtitle = subtitle;
        return this;
    }

    /// <summary>
    ///     Customizes the colors used for various parts of the menu.
    /// </summary>
    /// <param name="borderColor">The color for the menu border.</param>
    /// <param name="highlightColor">The background color for the selected option.</param>
    /// <param name="titleColor">The color for the menu title.</param>
    /// <param name="textColor">The color for the menu option text.</param>
    /// <param name="helpColor">The color for the help text.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
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

    /// <summary>
    ///     Specifies whether the menu should be centered vertically in the console window.
    /// </summary>
    /// <param name="centerVertically">True to center vertically, false otherwise.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder WithCenterVertically(bool centerVertically)
    {
        _centerVertically = centerVertically;
        return this;
    }

    /// <summary>
    ///     Customizes the characters used for the menu border.
    /// </summary>
    /// <param name="topLeftCorner">Character for the top-left corner.</param>
    /// <param name="topRightCorner">Character for the top-right corner.</param>
    /// <param name="bottomLeftCorner">Character for the bottom-left corner.</param>
    /// <param name="bottomRightCorner">Character for the bottom-right corner.</param>
    /// <param name="horizontalBorder">Character for horizontal border lines.</param>
    /// <param name="verticalBorder">Character for vertical border lines.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
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

    /// <summary>
    ///     Applies a double-line border style to the menu.
    /// </summary>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder WithDoubleBorder()
    {
        return WithBorderStyle("╔", "╗", "╚", "╝", "═", "║");
    }

    /// <summary>
    ///     Applies a rounded-corner border style to the menu.
    /// </summary>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder WithRoundedBorder()
    {
        return WithBorderStyle("╭", "╮", "╰", "╯");
    }

    /// <summary>
    ///     Adds a keyboard shortcut to the menu.
    /// </summary>
    /// <param name="key">The <see cref="ConsoleKey" /> that triggers the shortcut.</param>
    /// <param name="description">A description of the shortcut's action.</param>
    /// <param name="handler">The action to execute when the shortcut is pressed.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder AddShortcut(ConsoleKey key, string description, Action handler)
    {
        _shortcuts.Add(new KeyboardShortcut(key, description, handler));
        _showKeyboardShortcutsHelp = true; // Automatically enable help display if shortcuts are added
        return this;
    }

    /// <summary>
    ///     Controls whether help text for keyboard shortcuts is displayed.
    /// </summary>
    /// <param name="show">True to show keyboard shortcut help, false otherwise.</param>
    /// <returns>The current <see cref="MenuBuilder" /> instance for fluent chaining.</returns>
    public MenuBuilder ShowKeyboardShortcutsHelp(bool show)
    {
        _showKeyboardShortcutsHelp = show;
        return this;
    }

    /// <summary>
    ///     Clears the console and displays the menu, then handles user input.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        DrawHeader();
        HandleInput(); // This will draw options and then loop for input
    }

    /// <summary>
    ///     Draws the header section of the menu, including title, version, and current directory.
    /// </summary>
    private void DrawHeader()
    {
        var titleText = !string.IsNullOrEmpty(_version)
            ? $"{_title} v{_version}" // Combine title and version if version is provided
            : _title;

        // Draw top border line for the header
        string line = new string(_horizontalBorder[0], _width);
        Console.ForegroundColor = _titleColor;
        Console.WriteLine(line);

        // Draw the main title, centered
        Console.ForegroundColor = _textColor;
        Console.WriteLine(titleText.PadLeft((_width + titleText.Length) / 2).PadRight(_width));

        // Draw current directory below the title, centered
        Console.ForegroundColor = ConsoleColor.Yellow; // Specific color for directory
        string currentDir = Directory.GetCurrentDirectory();
        string dirDisplay = $"Current Directory: {currentDir}";
        // Ensure directory display does not exceed menu width
        if (dirDisplay.Length > _width) dirDisplay = dirDisplay.Substring(0, _width - 3) + "...";
        Console.WriteLine(dirDisplay.PadLeft((_width + dirDisplay.Length) / 2).PadRight(_width));
        Console.ResetColor();

        // Draw subtitle if provided, centered
        if (!string.IsNullOrEmpty(_subtitle))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine(_subtitle.PadLeft((_width + _subtitle.Length) / 2).PadRight(_width));
            Console.ResetColor();
        }

        // Draw bottom border line for the header
        Console.ForegroundColor = _titleColor;
        Console.WriteLine(line);
        Console.ResetColor();
    }

    /// <summary>
    ///     Draws the menu options without selection highlighting. (Not used directly by Show(), kept for potential future use
    ///     or debugging)
    /// </summary>
    private void DrawOptions()
    {
        var displayOptions = _options.Select(o => $"{o.Key}. {o.Text}").ToArray();

        // Center menu vertically if requested
        if (_centerVertically)
        {
            int topPadding = (Console.WindowHeight - displayOptions.Length - 10) / 2; // Approximate centering
            topPadding = Math.Max(0, topPadding); // Ensure non-negative padding
            for (int i = 0; i < topPadding; i++) Console.WriteLine();
        }

        // Draw menu box
        string border = new string(_horizontalBorder[0], _width - 2);
        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_topLeftCorner}{border}{_topRightCorner}"); // Top border
        Console.ResetColor();

        foreach (var option in displayOptions)
        {
            string padded = option.PadRight(_width - 4); // Padding for text within borders
            Console.ForegroundColor = _borderColor; // Border color for vertical lines
            Console.Write($"{_verticalBorder} "); // Left vertical border and space
            Console.ResetColor();

            Console.ForegroundColor = _textColor; // Text color for option
            Console.Write(padded);
            Console.ResetColor();

            Console.ForegroundColor = _borderColor; // Border color for vertical lines
            Console.WriteLine($" {_verticalBorder}"); // Space and right vertical border
            Console.ResetColor();
        }

        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_bottomLeftCorner}{border}{_bottomRightCorner}"); // Bottom border
        Console.ResetColor();

        // Display help text if available
        if (!string.IsNullOrEmpty(_helpText))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine($"\n{_helpText}");
            Console.ResetColor();
        }

        Console.Write("\nChoose an option: "); // Prompt for input
    }

    /// <summary>
    ///     Displays a help screen for the current menu, including general navigation and registered shortcuts.
    /// </summary>
    private void ShowHelp()
    {
        var helpView = new HelpView(_width, _borderColor, _titleColor, _textColor);

        // Add general navigation help items
        helpView.AddHelpItem("Navigation", "Use arrow keys (↑/↓) or number keys to navigate");
        helpView.AddHelpItem("Selection", "Press Enter to select an option");
        helpView.AddHelpItem("Exit", "Press Esc or 0 to return to previous menu/exit");
        helpView.AddHelpItem("Help", "Press F1 anytime to show this help");

        // Add all registered keyboard shortcuts to the help view
        if (_shortcuts.Count > 0)
            foreach (var shortcut in _shortcuts)
                helpView.AddHelpItem(shortcut.Key.ToString(), shortcut.Description);

        // Show the constructed help view
        helpView.Show($"{_title} - Help");
    }

    /// <summary>
    ///     Handles user input for menu navigation and option selection.
    /// </summary>
    private void HandleInput()
    {
        int selectedIndex = 0; // Default to the first option

        // Initial draw of the menu with the first option selected
        Console.Clear(); // Clear console before drawing
        DrawHeader();
        DrawOptionsWithSelection(selectedIndex);

        while (true) // Loop indefinitely until an exit condition is met
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key press without displaying it

            // Handle number key input for direct option selection
            if (char.IsDigit(keyInfo.KeyChar))
            {
                string input = keyInfo.KeyChar.ToString();
                var option = _options.FirstOrDefault(o => o.Key.Equals(input, StringComparison.OrdinalIgnoreCase));

                if (option != null)
                {
                    // Clear the "Choose an option:" prompt line before executing handler
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth)); // Overwrite with spaces
                    Console.SetCursorPosition(0, Console.CursorTop); // Reset cursor

                    option.Handler(); // Execute the selected option's action

                    // If the selected option is the exit key (typically "0"), return from HandleInput
                    if (option.Key == "0") return;

                    // Redraw the menu for subsequent interactions
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex); // Redraw with current selection
                }
                else
                {
                    // Invalid number key pressed
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Press any key to continue.");
                    Console.ResetColor();
                    Console.ReadKey(true); // Wait for acknowledgement

                    // Redraw the menu
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex);
                }

                continue; // Continue to next iteration of the input loop
            }

            // Handle special key presses (arrows, Enter, Esc, F1)
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(0, selectedIndex - 1); // Move selection up, clamping at 0
                    DrawOptionsWithSelection(selectedIndex); // Redraw with new selection
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex =
                        Math.Min(_options.Count - 1, selectedIndex + 1); // Move selection down, clamping at last option
                    DrawOptionsWithSelection(selectedIndex); // Redraw with new selection
                    break;

                case ConsoleKey.Enter:
                    if (selectedIndex >= 0 && selectedIndex < _options.Count) // Check if selection is valid
                    {
                        _options[selectedIndex].Handler(); // Execute handler for the selected option

                        // If the selected option is the exit key, return
                        if (_options[selectedIndex].Key == "0") return;

                        // Redraw menu
                        Console.Clear();
                        DrawHeader();
                        DrawOptionsWithSelection(selectedIndex);
                    }

                    break;

                case ConsoleKey.Escape:
                    // Attempt to find and execute an "exit" or "back" option (key "0")
                    var exitOption = _options.FirstOrDefault(o => o.Key == "0");
                    if (exitOption != null)
                    {
                        exitOption.Handler();
                        return; // Exit input loop
                    }

                    // If no "0" option, Escape might do nothing or be handled by a specific shortcut
                    break;

                case ConsoleKey.F1:
                    ShowHelp(); // Display the help screen
                    // Redraw the menu after help is closed
                    Console.Clear();
                    DrawHeader();
                    DrawOptionsWithSelection(selectedIndex);
                    break;

                default:
                    // Check if the pressed key matches any registered keyboard shortcuts
                    var shortcut = _shortcuts.FirstOrDefault(s => s.Key == keyInfo.Key);
                    if (shortcut != null)
                    {
                        shortcut.Handler(); // Execute shortcut's action

                        // Redraw the menu after shortcut execution
                        Console.Clear();
                        DrawHeader();
                        DrawOptionsWithSelection(selectedIndex);
                    }

                    // If no match, the key press is ignored
                    break;
            }
        }
    }

    /// <summary>
    ///     Draws the menu options, highlighting the currently selected option.
    /// </summary>
    /// <param name="selectedIndex">The index of the option to highlight.</param>
    private void DrawOptionsWithSelection(int selectedIndex)
    {
        var displayOptions = _options.Select(o => $"{o.Key}. {o.Text}").ToArray();

        // If centering vertically, clear and redraw header, then add padding
        if (_centerVertically)
        {
            Console.Clear(); // Clear entire console for vertical centering
            DrawHeader(); // Redraw header first

            // Calculate top padding to center the menu block
            // Approximate height: options + help text lines + borders + title area
            int approximateMenuBlockHeight = displayOptions.Length +
                                             (_showKeyboardShortcutsHelp && _shortcuts.Count > 0
                                                 ? _shortcuts.Count + 2
                                                 : 0) + (!string.IsNullOrEmpty(_helpText) ? 2 : 0) + 4;
            int topPadding =
                (Console.WindowHeight - approximateMenuBlockHeight - Console.CursorTop) /
                2; // Subtract current cursor top from window height
            topPadding = Math.Max(0, topPadding); // Ensure non-negative padding
            for (int i = 0; i < topPadding; i++) Console.WriteLine();
        }


        // If not centering, ensure cursor is positioned correctly after header
        // This might involve clearing specific lines if header height varies
        // For simplicity, current implementation relies on Console.Clear() in HandleInput or vertical centering logic
        // Draw menu box top border
        string border = new string(_horizontalBorder[0], _width - 2);
        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_topLeftCorner}{border}{_topRightCorner}");
        Console.ResetColor();

        // Draw each menu option
        for (int i = 0; i < displayOptions.Length; i++)
        {
            string optionText = displayOptions[i];
            // Ensure option text fits within the menu width, accounting for borders and padding
            string paddedOptionText = optionText.PadRight(_width - 4);
            if (paddedOptionText.Length > _width - 4) // Truncate if too long
                paddedOptionText = paddedOptionText.Substring(0, _width - 7) + "...";


            Console.ForegroundColor = _borderColor; // Color for vertical border
            Console.Write($"{_verticalBorder} "); // Left vertical border and space
            Console.ResetColor();

            // Highlight the selected option
            if (i == selectedIndex)
            {
                Console.BackgroundColor = _highlightColor; // Highlight background
                Console.ForegroundColor = ConsoleColor.Black; // Text color on highlight
                Console.Write(paddedOptionText);
                Console.ResetColor(); // Reset colors after writing highlighted option
            }
            else
            {
                Console.ForegroundColor = _textColor; // Standard text color
                Console.Write(paddedOptionText);
                Console.ResetColor();
            }

            Console.ForegroundColor = _borderColor; // Color for vertical border
            Console.WriteLine($" {_verticalBorder}"); // Space and right vertical border
            Console.ResetColor();
        }

        // Draw menu box bottom border
        Console.ForegroundColor = _borderColor;
        Console.WriteLine($"{_bottomLeftCorner}{border}{_bottomRightCorner}");
        Console.ResetColor();

        // Display help text if available
        if (!string.IsNullOrEmpty(_helpText))
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine($"\n{_helpText}");
            Console.ResetColor();
        }

        // Display keyboard shortcuts help if enabled and shortcuts exist
        if (_showKeyboardShortcutsHelp && _shortcuts.Count > 0)
        {
            Console.ForegroundColor = _helpColor;
            Console.WriteLine("\nKeyboard Shortcuts:");
            foreach (var shortcut in _shortcuts) Console.WriteLine($"  {shortcut.Key} - {shortcut.Description}");
            Console.ResetColor();
        }
    }
}

/// <summary>
///     Represents a single option in a console menu.
/// </summary>
public class MenuOption
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuOption" /> class.
    /// </summary>
    /// <param name="key">The key used to select the option (e.g., "1", "A").</param>
    /// <param name="text">The text displayed for the option.</param>
    /// <param name="handler">The action to execute when the option is selected.</param>
    public MenuOption(string key, string text, Action handler)
    {
        Key = key;
        Text = text;
        Handler = handler;
    }

    /// <summary>
    ///     Gets the key used to select the option.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the text displayed for the option.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Gets the action to execute when the option is selected.
    /// </summary>
    public Action Handler { get; }
}