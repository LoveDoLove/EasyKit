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

using System.Text;

namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Provides methods for prompting user input in the console.
/// </summary>
public class PromptView
{
    /// <summary>
    ///     Prompts the user for input with a given message.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <returns>The string input by the user, or null if input is redirected and an end-of-file is reached.</returns>
    public string? Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    /// <summary>
    ///     Prompts the user for input, allowing cancellation with the Escape key.
    ///     Handles Backspace for editing.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <param name="cancelHint">A hint displayed to the user about how to cancel. Defaults to " (or press Escape to cancel)".</param>
    /// <returns>The string input by the user, or null if the user presses Escape.</returns>
    public string? PromptWithCancel(string message, string cancelHint = " (or press Escape to cancel)")
    {
        Console.Write(message + cancelHint);

        var inputBuilder = new StringBuilder();
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true); // Read key without displaying it

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine(); // Move to next line after cancellation
                return null; // Indicate cancellation
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine(); // Move to next line after input submission
                return inputBuilder.ToString();
            }

            if (key.Key == ConsoleKey.Backspace && inputBuilder.Length > 0)
            {
                inputBuilder.Length--; // Remove last character from builder
                Console.Write("\b \b"); // Erase the character from console (backspace, space, backspace)
            }
            else if (!char.IsControl(key.KeyChar)) // Ignore control characters except Enter, Esc, Backspace
            {
                inputBuilder.Append(key.KeyChar); // Add character to builder
                Console.Write(key.KeyChar); // Display character on console
            }
        }
    }

    /// <summary>
    ///     Prompts the user for a Yes/No confirmation.
    /// </summary>
    /// <param name="message">The confirmation message to display.</param>
    /// <param name="defaultYes">
    ///     Determines the default response if the user presses Enter. True for Yes (Y/n), False for No
    ///     (y/N).
    /// </param>
    /// <returns>True if the user confirms Yes, False if No or Escape is pressed.</returns>
    public bool ConfirmYesNo(string message, bool defaultYes = true)
    {
        string hint = defaultYes ? " (Y/n): " : " (y/N): ";
        Console.Write(message + hint);

        while (true)
        {
            var key = Console.ReadKey(true); // Read key without displaying it

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine(defaultYes ? "Y" : "N"); // Display the default choice
                return defaultYes;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Cancelled"); // Indicate cancellation
                // Typically, cancellation of a Y/N might be treated as 'No' or require specific handling.
                // Here, returning false, which aligns with 'N'.
                return false;
            }

            char lowerKeyChar = char.ToLower(key.KeyChar);
            if (lowerKeyChar == 'y')
            {
                Console.WriteLine("Y");
                return true;
            }

            if (lowerKeyChar == 'n')
            {
                Console.WriteLine("N");
                return false;
            }
            // Ignore other key presses
        }
    }

    /// <summary>
    ///     Prompts the user for input with autocomplete suggestions.
    ///     Supports Escape to cancel, Enter to accept, Up/Down arrows to navigate suggestions,
    ///     Tab to autocomplete with the first suggestion, and F1 to toggle suggestion visibility.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <param name="suggestions">A list of strings to use as autocomplete suggestions.</param>
    /// <param name="cancelHint">A hint about how to cancel. Defaults to " (or press Escape to cancel)".</param>
    /// <returns>The string input by the user (possibly autocompleted), or null if cancelled.</returns>
    public string? PromptWithAutocomplete(string message, List<string> suggestions,
        string? cancelHint = " (or press Escape to cancel)")
    {
        Console.Write(message + (cancelHint ?? ""));

        var inputBuilder = new StringBuilder();
        int cursorPosition = 0; // Current position of the cursor within the input string
        List<string> currentMatchingSuggestions = new();
        int currentSelectedSuggestionIndex = -1; // Index of the currently highlighted suggestion
        bool areSuggestionsVisible = false; // Whether suggestions are currently being displayed

        // Store the initial console cursor position to correctly draw suggestions below the input line
        int initialConsoleLeft = Console.CursorLeft;
        int initialConsoleTop = Console.CursorTop;

        while (true)
        {
            var keyInfo = Console.ReadKey(true); // Read key without displaying it

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    // If a suggestion is selected, use it as the input
                    if (currentSelectedSuggestionIndex >= 0 &&
                        currentSelectedSuggestionIndex < currentMatchingSuggestions.Count)
                    {
                        inputBuilder.Clear().Append(currentMatchingSuggestions[currentSelectedSuggestionIndex]);
                        cursorPosition = inputBuilder.Length; // Move cursor to end of accepted suggestion

                        // Redraw the input line with the accepted suggestion
                        Console.SetCursorPosition(initialConsoleLeft, initialConsoleTop);
                        // Clear the line first (input + some buffer) then write the new input
                        Console.Write(new string(' ',
                            Console.WindowWidth > initialConsoleLeft
                                ? Console.WindowWidth - initialConsoleLeft - 1
                                : 0));
                        Console.SetCursorPosition(initialConsoleLeft, initialConsoleTop);
                        Console.Write(inputBuilder.ToString());
                    }

                    ClearSuggestionsDisplay(currentMatchingSuggestions.Count); // Clear suggestion display area
                    Console.WriteLine(); // Move to next line
                    return inputBuilder.ToString();

                case ConsoleKey.Escape:
                    ClearSuggestionsDisplay(currentMatchingSuggestions.Count);
                    Console.WriteLine(); // Move to next line
                    return null; // Indicate cancellation

                case ConsoleKey.Backspace:
                    if (cursorPosition > 0)
                    {
                        inputBuilder.Remove(cursorPosition - 1, 1);
                        cursorPosition--;
                        RedrawInputLine();
                        UpdateAndDisplaySuggestions();
                    }

                    break;

                case ConsoleKey.Delete:
                    if (cursorPosition < inputBuilder.Length)
                    {
                        inputBuilder.Remove(cursorPosition, 1);
                        RedrawInputLine();
                        UpdateAndDisplaySuggestions();
                    }

                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                        Console.SetCursorPosition(initialConsoleLeft + cursorPosition, initialConsoleTop);
                    }

                    break;

                case ConsoleKey.RightArrow:
                    if (cursorPosition < inputBuilder.Length)
                    {
                        cursorPosition++;
                        Console.SetCursorPosition(initialConsoleLeft + cursorPosition, initialConsoleTop);
                    }

                    break;

                case ConsoleKey.Tab: // Autocomplete with the first (or currently selected) suggestion
                    if (currentMatchingSuggestions.Count > 0)
                    {
                        int suggestionToUse = currentSelectedSuggestionIndex >= 0 ? currentSelectedSuggestionIndex : 0;
                        inputBuilder.Clear().Append(currentMatchingSuggestions[suggestionToUse]);
                        cursorPosition = inputBuilder.Length;
                        RedrawInputLine();
                        UpdateAndDisplaySuggestions(); // Suggestions might change or disappear
                    }

                    break;

                case ConsoleKey.DownArrow:
                    if (areSuggestionsVisible && currentMatchingSuggestions.Count > 0)
                    {
                        currentSelectedSuggestionIndex =
                            (currentSelectedSuggestionIndex + 1) % currentMatchingSuggestions.Count;
                        DrawSuggestionsDisplay();
                    }

                    break;

                case ConsoleKey.UpArrow:
                    if (areSuggestionsVisible && currentMatchingSuggestions.Count > 0)
                    {
                        currentSelectedSuggestionIndex =
                            (currentSelectedSuggestionIndex - 1 + currentMatchingSuggestions.Count) %
                            currentMatchingSuggestions.Count;
                        DrawSuggestionsDisplay();
                    }

                    break;

                case ConsoleKey.F1: // Toggle suggestion visibility
                    areSuggestionsVisible = !areSuggestionsVisible;
                    if (areSuggestionsVisible)
                        UpdateAndDisplaySuggestions();
                    else
                        ClearSuggestionsDisplay(currentMatchingSuggestions.Count);
                    break;

                default:
                    if (!char.IsControl(keyInfo.KeyChar)) // Process printable characters
                    {
                        inputBuilder.Insert(cursorPosition, keyInfo.KeyChar);
                        cursorPosition++;
                        RedrawInputLine();
                        UpdateAndDisplaySuggestions();
                    }

                    break;
            }
        }

        // Helper to redraw the current input line
        void RedrawInputLine()
        {
            Console.SetCursorPosition(initialConsoleLeft, initialConsoleTop);
            // Write current input, then a space to clear the character if input shrinks, then reposition cursor
            Console.Write(inputBuilder + " ");
            Console.SetCursorPosition(initialConsoleLeft + cursorPosition, initialConsoleTop);
        }

        // Helper to update the list of matching suggestions and then draw them
        void UpdateAndDisplaySuggestions()
        {
            if (!areSuggestionsVisible)
            {
                ClearSuggestionsDisplay(currentMatchingSuggestions.Count); // Ensure old ones are cleared if toggled off
                currentMatchingSuggestions.Clear();
                return;
            }

            ClearSuggestionsDisplay(currentMatchingSuggestions
                .Count); // Clear previous suggestions before finding new ones

            string currentInputLower = inputBuilder.ToString().ToLower();
            currentMatchingSuggestions = suggestions
                .Where(s => s.ToLower().Contains(currentInputLower))
                .Take(5) // Limit the number of suggestions displayed
                .ToList();

            currentSelectedSuggestionIndex =
                currentMatchingSuggestions.Count > 0 ? 0 : -1; // Default to first or no selection
            DrawSuggestionsDisplay();
        }

        // Helper to draw the suggestions below the input line
        void DrawSuggestionsDisplay()
        {
            if (!areSuggestionsVisible || currentMatchingSuggestions.Count == 0)
            {
                ClearSuggestionsDisplay(5); // Clear max possible suggestion lines if none to show but were visible
                return;
            }

            int originalCursorLeft = Console.CursorLeft; // Save cursor before drawing suggestions
            int originalCursorTop = Console.CursorTop;

            for (int i = 0; i < currentMatchingSuggestions.Count; i++)
            {
                Console.SetCursorPosition(initialConsoleLeft,
                    initialConsoleTop + i + 1); // Draw suggestions one line below input
                if (i == currentSelectedSuggestionIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan; // Highlight selected suggestion
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray; // Non-selected suggestion color
                }

                // Pad suggestion to a fixed width or console width to ensure proper clearing
                string suggestionText = currentMatchingSuggestions[i];
                int displayWidth = Console.WindowWidth > initialConsoleLeft
                    ? Console.WindowWidth - initialConsoleLeft - 1
                    : suggestionText.Length;
                Console.Write(
                    suggestionText.PadRight(Math.Min(suggestionText.Length + 5,
                        displayWidth))); // Pad for consistent look
                Console.ResetColor();
            }

            // Clear any remaining old suggestion lines if new list is shorter
            ClearSuggestionsDisplay(5, currentMatchingSuggestions.Count);


            Console.SetCursorPosition(originalCursorLeft, originalCursorTop); // Restore cursor to input line
        }

        // Helper to clear the suggestion display area
        void ClearSuggestionsDisplay(int numberOfLinesToClear, int startIndex = 0)
        {
            if (!areSuggestionsVisible && startIndex == 0)
                return; // Don't clear if not visible unless specifically clearing old lines

            int originalCursorLeft = Console.CursorLeft;
            int originalCursorTop = Console.CursorTop;

            for (int i = startIndex; i < numberOfLinesToClear; i++)
            {
                Console.SetCursorPosition(initialConsoleLeft, initialConsoleTop + i + 1);
                Console.Write(new string(' ',
                    Console.WindowWidth > initialConsoleLeft
                        ? Console.WindowWidth - initialConsoleLeft - 1
                        : 0)); // Clear line
            }

            Console.SetCursorPosition(originalCursorLeft, originalCursorTop); // Restore cursor
        }
    }
}