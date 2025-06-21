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
///     Provides methods to display various types of menus in the console.
///     This class is a simpler way to show basic menus compared to <see cref="MenuBuilder" />
///     for more complex scenarios.
/// </summary>
public class MenuView
{
    /// <summary>
    ///     Displays the main menu with a predefined set of options.
    /// </summary>
    /// <param name="title">The title of the main menu.</param>
    /// <param name="version">The version string to display with the title.</param>
    /// <param name="width">The width of the menu in characters. Defaults to 50.</param>
    public void ShowMainMenu(string title, string version, int width = 50)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
        Console.ResetColor();
        new HeaderView().DrawHeader($"{title} v{version}", width);

        // Predefined options for the main menu
        string[] options =
        [
            "0. Exit",
            "1. Option 1", // Placeholder option
            "2. Option 2", // Placeholder option
            "3. Option 3", // Placeholder option
            "4. Option 4", // Placeholder option
            "5. Settings" // Placeholder option
        ];

        // Calculate top padding to center the menu vertically in the console window
        // The "- 6" accounts for header lines, border, and help text lines.
        int topPadding = (Console.WindowHeight - options.Length - 6) / 2;
        topPadding = Math.Max(0, topPadding); // Ensure padding is not negative
        for (int i = 0; i < topPadding; i++) Console.WriteLine();

        // Draw the menu box and options
        string horizontalBorder = new string('─', width - 2); // Character for horizontal border lines

        // Draw top border
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"┌{horizontalBorder}┐");
        Console.ResetColor();

        // Draw each option within the borders
        foreach (var option in options)
        {
            string paddedOptionText = option.PadRight(width - 4); // Pad text to fit within menu width
            Console.ForegroundColor = ConsoleColor.Cyan; // Color for vertical borders and option text
            Console.Write("│ "); // Left vertical border and space
            Console.ResetColor(); // Reset for option text if different color is desired (currently same)
            Console.Write(paddedOptionText); // Write the option text
            Console.ForegroundColor = ConsoleColor.Cyan; // Re-apply color for right border
            Console.WriteLine(" │"); // Space and right vertical border
            Console.ResetColor();
        }

        // Draw bottom border
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"└{horizontalBorder}┘");
        Console.ResetColor();

        // Display navigation hint text
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nUse number keys to select an option. Press 0 to exit.");
        Console.ResetColor();
        Console.Write("\nChoose an option: "); // Prompt for user input
    }

    /// <summary>
    ///     Displays a generic menu with the provided title and options.
    /// </summary>
    /// <param name="title">The title of the menu.</param>
    /// <param name="options">An array of strings representing the menu options.</param>
    /// <param name="width">The width of the menu in characters. Defaults to 50.</param>
    public void ShowMenu(string title, string[] options, int width = 50)
    {
        Console.Clear();
        new HeaderView().DrawHeader(title, width);

        // Calculate top padding for vertical centering
        int topPadding = (Console.WindowHeight - options.Length - 6) / 2;
        topPadding = Math.Max(0, topPadding);
        for (int i = 0; i < topPadding; i++) Console.WriteLine();

        // Draw the menu box and options
        string horizontalBorder = new string('─', width - 2);

        // Draw top border
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"┌{horizontalBorder}┐");
        Console.ResetColor();

        // Draw each option
        foreach (var option in options)
        {
            string paddedOptionText = option.PadRight(width - 4);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("│ ");
            Console.ResetColor();
            Console.Write(paddedOptionText);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        // Draw bottom border
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"└{horizontalBorder}┘");
        Console.ResetColor();

        // Display navigation hint
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nUse number keys to select an option. Press 0 to exit.");
        Console.ResetColor();
        Console.Write("\nChoose an option: ");
    }

    /// <summary>
    ///     Creates and returns a new <see cref="MenuBuilder" /> instance for constructing more complex, interactive menus.
    /// </summary>
    /// <param name="title">The title for the menu to be built.</param>
    /// <param name="version">Optional version string for the menu title. Defaults to an empty string.</param>
    /// <param name="width">The width for the menu to be built. Defaults to 50 characters.</param>
    /// <returns>A new <see cref="MenuBuilder" /> instance.</returns>
    public MenuBuilder CreateMenu(string title, string version = "", int width = 50)
    {
        return new MenuBuilder(title, width, version);
    }
}