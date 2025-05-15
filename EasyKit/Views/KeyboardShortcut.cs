namespace EasyKit.Views;

public class KeyboardShortcut
{
    public KeyboardShortcut(ConsoleKey key, string description, Action handler)
    {
        Key = key;
        Description = description;
        Handler = handler;
    }

    public ConsoleKey Key { get; }
    public string Description { get; }
    public Action Handler { get; }
}