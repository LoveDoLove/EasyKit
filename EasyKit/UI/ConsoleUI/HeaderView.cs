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