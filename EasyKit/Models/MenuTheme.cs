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