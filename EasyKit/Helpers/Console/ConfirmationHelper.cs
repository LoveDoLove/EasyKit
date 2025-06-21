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

namespace EasyKit.Helpers.Console;

/// <summary>
///     Provides confirmation prompts for actions and admin elevation.
/// </summary>
public class ConfirmationHelper
{
    /// <summary>
    ///     ConfirmationService constructor using the new Config class.
    /// </summary>
    public ConfirmationHelper()
    {
    }

    /// <summary>
    ///     Prompts the user to confirm an action.
    /// </summary>
    public bool ConfirmAction(string message, bool defaultYes = true)
    {
        return ConfirmYesNo(message, defaultYes);
    }

    /// <summary>
    ///     Prompts the user for a yes/no confirmation.
    /// </summary>
    private bool ConfirmYesNo(string message, bool defaultYes = true)
    {
        System.Console.Write($"{message} [{(defaultYes ? "Y/n" : "y/N")}]: ");
        var input = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            return defaultYes;
        input = input.Trim().ToLower();
        return defaultYes
            ? !(input == "n" || input == "no")
            : input == "y" || input == "yes";
    }
}