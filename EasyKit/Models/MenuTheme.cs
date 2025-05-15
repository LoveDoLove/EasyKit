namespace EasyKit.Models;

public static class MenuTheme
{
    // Predefined color schemes
    public static class ColorScheme
    {
        public static (ConsoleColor border, ConsoleColor highlight, ConsoleColor title, ConsoleColor text, ConsoleColor
            help) Dark =
                (ConsoleColor.DarkCyan, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.White,
                    ConsoleColor.DarkGray);

        public static (ConsoleColor border, ConsoleColor highlight, ConsoleColor title, ConsoleColor text, ConsoleColor
            help) Light =
                (ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.DarkYellow, ConsoleColor.Black,
                    ConsoleColor.DarkGray);

        public static (ConsoleColor border, ConsoleColor highlight, ConsoleColor title, ConsoleColor text, ConsoleColor
            help) Forest =
                (ConsoleColor.DarkGreen, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.White,
                    ConsoleColor.DarkGray);

        public static (ConsoleColor border, ConsoleColor highlight, ConsoleColor title, ConsoleColor text, ConsoleColor
            help) Ruby =
                (ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.White,
                    ConsoleColor.DarkGray);

        public static (ConsoleColor border, ConsoleColor highlight, ConsoleColor title, ConsoleColor text, ConsoleColor
            help) Purple =
                (ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White,
                    ConsoleColor.DarkGray);
    }

    // Predefined border styles
    public static class BorderStyle
    {
        public static (string topLeft, string topRight, string bottomLeft, string bottomRight, string horizontal, string
            vertical) Single =
                ("┌", "┐", "└", "┘", "─", "│");

        public static (string topLeft, string topRight, string bottomLeft, string bottomRight, string horizontal, string
            vertical) Double =
                ("╔", "╗", "╚", "╝", "═", "║");

        public static (string topLeft, string topRight, string bottomLeft, string bottomRight, string horizontal, string
            vertical) Rounded =
                ("╭", "╮", "╰", "╯", "─", "│");

        public static (string topLeft, string topRight, string bottomLeft, string bottomRight, string horizontal, string
            vertical) Block =
                ("▛", "▜", "▙", "▟", "▀", "▌");
    }
}