namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Represents a keyboard shortcut with an associated action.
/// </summary>
public class KeyboardShortcut
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyboardShortcut" /> class.
    /// </summary>
    /// <param name="key">The console key that triggers the shortcut.</param>
    /// <param name="description">A description of what the shortcut does.</param>
    /// <param name="handler">The action to execute when the shortcut is triggered.</param>
    public KeyboardShortcut(ConsoleKey key, string description, Action handler)
    {
        Key = key;
        Description = description;
        Handler = handler;
    }

    /// <summary>
    ///     Gets the console key that triggers the shortcut.
    /// </summary>
    public ConsoleKey Key { get; }

    /// <summary>
    ///     Gets the description of what the shortcut does.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the action to execute when the shortcut is triggered.
    /// </summary>
    public Action Handler { get; }
}