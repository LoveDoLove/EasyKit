namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Provides functionality to draw a stylized header in the console.
/// </summary>
public class HeaderView
{
    /// <summary>
    ///     Draws a header with the specified title and width.
    /// </summary>
    /// <param name="title">The title to display in the header.</param>
    /// <param name="width">The width of the header. Defaults to 50 characters.</param>
    public void DrawHeader(string title, int width = 50)
    {
        string line = new string('═', width);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(line);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        // Center the title within the specified width
        Console.WriteLine(title.PadLeft((width + title.Length) / 2).PadRight(width));
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(line);
        Console.ResetColor();
    }
}