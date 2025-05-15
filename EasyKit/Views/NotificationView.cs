namespace EasyKit.Views;

public class NotificationView
{
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public static void Show(string message, NotificationType type = NotificationType.Info, int durationMs = 3000,
        bool requireKeyPress = false)
    {
        // Save cursor position
        int originalLeft = Console.CursorLeft;
        int originalTop = Console.CursorTop;

        // Determine colors based on type
        ConsoleColor bgColor, fgColor;
        string icon;

        switch (type)
        {
            case NotificationType.Success:
                bgColor = ConsoleColor.DarkGreen;
                fgColor = ConsoleColor.White;
                icon = "✓";
                break;
            case NotificationType.Warning:
                bgColor = ConsoleColor.DarkYellow;
                fgColor = ConsoleColor.Black;
                icon = "!";
                break;
            case NotificationType.Error:
                bgColor = ConsoleColor.DarkRed;
                fgColor = ConsoleColor.White;
                icon = "✗";
                break;
            case NotificationType.Info:
            default:
                bgColor = ConsoleColor.DarkBlue;
                fgColor = ConsoleColor.White;
                icon = "i";
                break;
        }

        // Calculate banner width and position
        int bannerWidth = Math.Min(Console.WindowWidth - 4, Math.Max(message.Length + 6, 20));
        int leftPosition = (Console.WindowWidth - bannerWidth) / 2;

        // Store console colors
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        // Draw the notification banner
        Console.SetCursorPosition(leftPosition, 0);
        Console.BackgroundColor = bgColor;
        Console.ForegroundColor = fgColor;

        // Draw top border
        Console.Write(new string(' ', bannerWidth));

        // Draw message
        Console.SetCursorPosition(leftPosition, 1);
        string paddedMessage = $" {icon} {message}";
        if (paddedMessage.Length > bannerWidth - 2)
            // Truncate the message if it's too long
            paddedMessage = paddedMessage.Substring(0, bannerWidth - 5) + "...";
        Console.Write(paddedMessage.PadRight(bannerWidth));

        // Draw bottom border
        Console.SetCursorPosition(leftPosition, 2);
        Console.Write(new string(' ', bannerWidth));

        // Reset console colors
        Console.BackgroundColor = originalBg;
        Console.ForegroundColor = originalFg;

        // Add instruction if requiring key press
        if (requireKeyPress)
        {
            Console.SetCursorPosition(leftPosition + (bannerWidth - 17) / 2, 2);
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.Write(" Press any key... ");
            Console.BackgroundColor = originalBg;
            Console.ForegroundColor = originalFg;

            // Wait for key press
            Console.ReadKey(true);
        }
        else
        {
            // Wait for specified duration
            Thread.Sleep(durationMs);
        }

        // Clear the notification area
        for (int i = 0; i < 3; i++)
        {
            Console.SetCursorPosition(leftPosition, i);
            Console.Write(new string(' ', bannerWidth));
        }

        // Restore cursor position
        Console.SetCursorPosition(originalLeft, originalTop);
    }
}