namespace EasyKit.UI.ConsoleUI;

/// <summary>
///     Provides functionality to display temporary notifications at the top of the console.
/// </summary>
public class NotificationView
{
    /// <summary>
    ///     Defines the types of notifications, which determine the color scheme and icon.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        ///     Informational message.
        /// </summary>
        Info,

        /// <summary>
        ///     Success message.
        /// </summary>
        Success,

        /// <summary>
        ///     Warning message.
        /// </summary>
        Warning,

        /// <summary>
        ///     Error message.
        /// </summary>
        Error
    }

    /// <summary>
    ///     Shows a notification banner at the top of the console.
    /// </summary>
    /// <param name="message">The message to display in the notification.</param>
    /// <param name="type">The type of notification (Info, Success, Warning, Error). Defaults to Info.</param>
    /// <param name="durationMs">
    ///     The duration in milliseconds to display the notification if
    ///     <paramref name="requireKeyPress" /> is false. Defaults to 3000ms.
    /// </param>
    /// <param name="requireKeyPress">
    ///     If true, the notification will persist until a key is pressed. If false, it will
    ///     disappear after <paramref name="durationMs" />. Defaults to false.
    /// </param>
    public static void Show(string message, NotificationType type = NotificationType.Info, int durationMs = 3000,
        bool requireKeyPress = false)
    {
        // Save original cursor position to restore it after displaying the notification
        int originalLeft = Console.CursorLeft;
        int originalTop = Console.CursorTop;

        // Determine background color, foreground color, and icon based on the notification type
        ConsoleColor bgColor, fgColor;
        string icon;

        switch (type)
        {
            case NotificationType.Success:
                bgColor = ConsoleColor.DarkGreen;
                fgColor = ConsoleColor.White;
                icon = "✓"; // Check mark icon for success
                break;
            case NotificationType.Warning:
                bgColor = ConsoleColor.DarkYellow;
                fgColor = ConsoleColor.Black;
                icon = "!"; // Exclamation mark icon for warning
                break;
            case NotificationType.Error:
                bgColor = ConsoleColor.DarkRed;
                fgColor = ConsoleColor.White;
                icon = "✗"; // Cross mark icon for error
                break;
            case NotificationType.Info:
            default: // Default to Info type
                bgColor = ConsoleColor.DarkBlue;
                fgColor = ConsoleColor.White;
                icon = "i"; // 'i' icon for information
                break;
        }

        // Calculate the width of the notification banner
        // It should be at most the console width minus some padding,
        // and at least the message length plus icon/padding, or a minimum of 20 characters.
        int bannerWidth = Math.Min(Console.WindowWidth - 4, Math.Max(message.Length + 6, 20));
        // Calculate the horizontal position to center the banner
        int leftPosition = (Console.WindowWidth - bannerWidth) / 2;

        // Store original console foreground and background colors to restore them later
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        // Set console colors for the notification banner
        Console.BackgroundColor = bgColor;
        Console.ForegroundColor = fgColor;

        // Draw the notification banner (3 lines high)
        // Top line (acts as top border and background)
        Console.SetCursorPosition(leftPosition, 0);
        Console.Write(new string(' ', bannerWidth));

        // Middle line with icon and message
        Console.SetCursorPosition(leftPosition, 1);
        string paddedMessage = $" {icon} {message}"; // Add icon and leading/trailing spaces
        if (paddedMessage.Length > bannerWidth) // Check if message exceeds banner width
            // Truncate the message and add ellipsis if it's too long
            paddedMessage = paddedMessage.Substring(0, bannerWidth - 3) + "...";
        Console.Write(paddedMessage.PadRight(bannerWidth)); // Pad to fill banner width

        // Bottom line (acts as bottom border and background)
        Console.SetCursorPosition(leftPosition, 2);
        Console.Write(new string(' ', bannerWidth));


        // If notification requires a key press to dismiss
        if (requireKeyPress)
        {
            // Display "Press any key..." message within the banner's bottom line
            string keyPressPrompt = " Press any key... ";
            int promptLeft = leftPosition + (bannerWidth - keyPressPrompt.Length) / 2;
            promptLeft = Math.Max(leftPosition, promptLeft); // Ensure it doesn't go left of banner

            Console.SetCursorPosition(promptLeft, 2); // Position on the bottom line of the banner
            // Temporarily set colors for the prompt text itself
            Console.BackgroundColor = bgColor; // Keep banner background
            Console.ForegroundColor = fgColor; // Keep banner foreground
            Console.Write(keyPressPrompt);

            // Reset console colors before waiting for key press
            Console.BackgroundColor = originalBg;
            Console.ForegroundColor = originalFg;
            Console.ReadKey(true); // Wait for any key press
        }
        else
        {
            // Reset console colors before sleeping
            Console.BackgroundColor = originalBg;
            Console.ForegroundColor = originalFg;
            Thread.Sleep(durationMs); // Wait for the specified duration
        }

        // Reset console colors fully if not already done (e.g. if requireKeyPress was false)
        Console.BackgroundColor = originalBg;
        Console.ForegroundColor = originalFg;

        // Clear the notification area by overwriting it with spaces
        for (int i = 0; i < 3; i++) // Loop through the 3 lines of the banner
        {
            Console.SetCursorPosition(leftPosition, i);
            Console.Write(new string(' ', bannerWidth));
        }

        // Restore the original cursor position
        Console.SetCursorPosition(originalLeft, originalTop);
    }
}