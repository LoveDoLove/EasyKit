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