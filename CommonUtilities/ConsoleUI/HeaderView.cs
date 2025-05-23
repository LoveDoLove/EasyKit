namespace CommonUtilities.ConsoleUI;

public class HeaderView
{
    public void DrawHeader(string title, int width = 50)
    {
        string line = new string('═', width);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(line);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(title.PadLeft((width + title.Length) / 2).PadRight(width));
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(line);
        Console.ResetColor();
    }
}