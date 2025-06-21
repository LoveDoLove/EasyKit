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

using EasyKit.Models;

namespace EasyKit.Services;

public class ConsoleService
{
    /// <summary>
    ///     ConsoleService constructor using the new Config class.
    /// </summary>
    public ConsoleService(Config config)
    {
        Config = config;
    }

    // Add a public property to access the config
    public Config Config { get; }

    public void WriteInfo(string message)
    {
        // Get text color from config
        var textColorObj = Config.Get("text_color", "");
        var bgColorObj = Config.Get("background_color", "");

        // If specific colors are set, use them
        ConsoleColor? textColor = null;
        ConsoleColor? bgColor = null;
        if (textColorObj != null && Enum.TryParse(textColorObj.ToString(), true, out ConsoleColor parsedTextColor))
            textColor = parsedTextColor;
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor parsedBgColor))
            bgColor = parsedBgColor;

        // Force visible text if both are black
        if (textColor == ConsoleColor.Black && bgColor == ConsoleColor.Black)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        else
        {
            if (textColor.HasValue)
                Console.ForegroundColor = textColor.Value;
            else
                // Removed color_scheme logic, default to White
                Console.ForegroundColor = ConsoleColor.White;

            if (bgColor.HasValue)
                Console.BackgroundColor = bgColor.Value;
        }

        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteError(string message)
    {
        // Get error text color from config or default to red
        var errorColorObj = Config.Get("error_color", "Red");
        if (errorColorObj != null && Enum.TryParse(errorColorObj.ToString(), true, out ConsoleColor errorColor))
            Console.ForegroundColor = errorColor;
        else
            Console.ForegroundColor = ConsoleColor.Red;

        // Use background color if specified
        var bgColorObj = Config.Get("background_color", "");
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor bgColor))
            Console.BackgroundColor = bgColor;

        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteSuccess(string message)
    {
        // Get success text color from config or default to green
        var successColorObj = Config.Get("success_color", "Green");
        if (successColorObj != null && Enum.TryParse(successColorObj.ToString(), true, out ConsoleColor successColor))
            Console.ForegroundColor = successColor;
        else
            Console.ForegroundColor = ConsoleColor.Green;

        // Use background color if specified
        var bgColorObj = Config.Get("background_color", "");
        if (bgColorObj != null && Enum.TryParse(bgColorObj.ToString(), true, out ConsoleColor bgColor))
            Console.BackgroundColor = bgColor;

        Console.WriteLine(message);
        Console.ResetColor();
    }
}