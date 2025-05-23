namespace CommonUtilities.ConsoleUI;

public class MenuView
{
    public void ShowMainMenu(string title, string version, int width = 50)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
        Console.ResetColor();
        new HeaderView().DrawHeader($"{title} v{version}", width);
        string[] options =
        [
            "0. Exit",
            "1. Option 1",
            "2. Option 2",
            "3. Option 3",
            "4. Option 4",
            "5. Settings"
        ];

        // Center menu vertically
        int topPadding = (Console.WindowHeight - options.Length - 6) / 2;
        for (int i = 0; i < topPadding; i++) Console.WriteLine();

        // Draw menu with highlight and box
        string border = new string('─', width - 2);
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"┌{border}┐");
        Console.ResetColor();
        foreach (var option in options)
        {
            string padded = option.PadRight(width - 4);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("│ ");
            Console.ResetColor();
            Console.Write(padded);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"└{border}┘");
        Console.ResetColor();

        // Add navigation hint
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nUse number keys to select an option. Press 0 to exit.");
        Console.ResetColor();
        Console.Write("\nChoose an option: ");
    }

    public void ShowMenu(string title, string[] options, int width = 50)
    {
        Console.Clear();
        new HeaderView().DrawHeader(title, width);

        // Center menu vertically
        int topPadding = (Console.WindowHeight - options.Length - 6) / 2;
        for (int i = 0; i < topPadding; i++) Console.WriteLine();

        // Draw menu with highlight and box
        string border = new string('─', width - 2);
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"┌{border}┐");
        Console.ResetColor();
        foreach (var option in options)
        {
            string padded = option.PadRight(width - 4);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("│ ");
            Console.ResetColor();
            Console.Write(padded);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"└{border}┘");
        Console.ResetColor();

        // Add navigation hint
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nUse number keys to select an option. Press 0 to exit.");
        Console.ResetColor();
        Console.Write("\nChoose an option: ");
    }

    // Method to create more complex menus using the MenuBuilder
    public MenuBuilder CreateMenu(string title, string version = "", int width = 50)
    {
        return new MenuBuilder(title, width, version);
    }
}