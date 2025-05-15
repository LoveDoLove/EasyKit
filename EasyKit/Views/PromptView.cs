namespace EasyKit.Views;

public class PromptView
{
    public string? Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    public string? PromptWithCancel(string message, string cancelHint = " (or press Escape to cancel)")
    {
        Console.Write(message + cancelHint);

        string input = "";
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return null; // Return null to indicate cancellation
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input;
            }

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b"); // Erase the last character
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }

    public bool ConfirmYesNo(string message, bool defaultYes = true)
    {
        string hint = defaultYes ? " (Y/n): " : " (y/N): ";
        Console.Write(message + hint);

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine(defaultYes ? "Y" : "N");
                return defaultYes;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Cancelled");
                return false;
            }

            if (char.ToLower(key.KeyChar) == 'y')
            {
                Console.WriteLine("Y");
                return true;
            }

            if (char.ToLower(key.KeyChar) == 'n')
            {
                Console.WriteLine("N");
                return false;
            }
        }
    }

    public string? PromptWithAutocomplete(string message, List<string> suggestions,
        string? cancelHint = " (or press Escape to cancel)")
    {
        Console.Write(message + (cancelHint ?? ""));

        string input = "";
        int cursorPosition = 0;
        List<string> matchingSuggestions = new();
        int selectedSuggestion = -1;
        bool showSuggestions = false;

        // Save the initial cursor position for drawing suggestions
        int initialLeft = Console.CursorLeft;
        int initialTop = Console.CursorTop;

        while (true)
        {
            // Handle key input
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    // Accept current input or selected suggestion
                    if (selectedSuggestion >= 0 && selectedSuggestion < matchingSuggestions.Count)
                    {
                        input = matchingSuggestions[selectedSuggestion];

                        // Clear current line and redraw with the accepted suggestion
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(new string(' ', input.Length + 5)); // Clear with some extra space
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(input);
                    }

                    // Clear suggestions area
                    ClearSuggestions(matchingSuggestions.Count);

                    Console.WriteLine();
                    return input;

                case ConsoleKey.Escape:
                    // Cancel operation
                    ClearSuggestions(matchingSuggestions.Count);
                    Console.WriteLine();
                    return null;

                case ConsoleKey.Backspace:
                    if (input.Length > 0 && cursorPosition > 0)
                    {
                        // Remove character before cursor
                        input = input.Remove(cursorPosition - 1, 1);
                        cursorPosition--;

                        // Redraw input
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(input + " "); // Extra space to clear any left-over character
                        Console.SetCursorPosition(initialLeft + cursorPosition, initialTop);

                        // Update suggestions
                        UpdateSuggestions();
                    }

                    break;

                case ConsoleKey.Delete:
                    if (input.Length > 0 && cursorPosition < input.Length)
                    {
                        // Remove character at cursor
                        input = input.Remove(cursorPosition, 1);

                        // Redraw input
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(input + " "); // Extra space to clear any left-over character
                        Console.SetCursorPosition(initialLeft + cursorPosition, initialTop);

                        // Update suggestions
                        UpdateSuggestions();
                    }

                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                        Console.SetCursorPosition(initialLeft + cursorPosition, initialTop);
                    }

                    break;

                case ConsoleKey.RightArrow:
                    if (cursorPosition < input.Length)
                    {
                        cursorPosition++;
                        Console.SetCursorPosition(initialLeft + cursorPosition, initialTop);
                    }

                    break;

                case ConsoleKey.Tab:
                    // Auto-complete with first suggestion
                    if (matchingSuggestions.Count > 0)
                    {
                        input = matchingSuggestions[0];
                        cursorPosition = input.Length;

                        // Redraw input
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(input);

                        // Update suggestions
                        UpdateSuggestions();
                    }

                    break;

                case ConsoleKey.DownArrow:
                    // Navigate through suggestions
                    if (matchingSuggestions.Count > 0)
                    {
                        selectedSuggestion = (selectedSuggestion + 1) % matchingSuggestions.Count;
                        DrawSuggestions();
                    }

                    break;

                case ConsoleKey.UpArrow:
                    // Navigate through suggestions
                    if (matchingSuggestions.Count > 0)
                    {
                        selectedSuggestion = selectedSuggestion <= 0
                            ? matchingSuggestions.Count - 1
                            : selectedSuggestion - 1;
                        DrawSuggestions();
                    }

                    break;

                case ConsoleKey.F1:
                    // Toggle suggestions visibility
                    showSuggestions = !showSuggestions;
                    if (showSuggestions)
                        // If showing suggestions, update them based on current input
                        UpdateSuggestions();
                    else
                        // If hiding, clear the suggestions area
                        ClearSuggestions(matchingSuggestions.Count);
                    break;

                default:
                    // Add character to input
                    if (!char.IsControl(key.KeyChar))
                    {
                        if (cursorPosition == input.Length)
                            input += key.KeyChar;
                        else
                            input = input.Insert(cursorPosition, key.KeyChar.ToString());

                        cursorPosition++;

                        // Redraw input
                        Console.SetCursorPosition(initialLeft, initialTop);
                        Console.Write(input);
                        Console.SetCursorPosition(initialLeft + cursorPosition, initialTop);

                        // Update suggestions
                        UpdateSuggestions();
                    }

                    break;
            }
        }

        // Helper methods for autocomplete functionality
        void UpdateSuggestions()
        {
            if (!showSuggestions) return;

            // Clear previous suggestions
            ClearSuggestions(matchingSuggestions.Count);

            // Find matching suggestions
            matchingSuggestions = suggestions
                .Where(s => s.ToLower().Contains(input.ToLower()))
                .Take(5) // Limit to 5 suggestions
                .ToList();

            // Reset selection
            selectedSuggestion = matchingSuggestions.Count > 0 ? 0 : -1;

            // Draw new suggestions
            DrawSuggestions();
        }

        void DrawSuggestions()
        {
            if (matchingSuggestions.Count == 0) return;

            // Save current position
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            // Draw each suggestion
            for (int i = 0; i < matchingSuggestions.Count; i++)
            {
                Console.SetCursorPosition(initialLeft, initialTop + i + 2);

                if (i == selectedSuggestion)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.Write(matchingSuggestions[i].PadRight(30));
                Console.ResetColor();
            }

            // Restore cursor position
            Console.SetCursorPosition(currentLeft, currentTop);
        }

        void ClearSuggestions(int count)
        {
            // Save current position
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            // Clear suggestion lines
            for (int i = 0; i < count; i++)
            {
                Console.SetCursorPosition(0, initialTop + i + 2);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            // Restore cursor position
            Console.SetCursorPosition(currentLeft, currentTop);
        }
    }
}