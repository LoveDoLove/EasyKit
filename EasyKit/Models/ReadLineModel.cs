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

/// <summary>
///     Represents the parameters for a console read line operation,
///     including the prompt question, a default value for cancellation,
///     and whether empty input is allowed.
/// </summary>
public class ReadLineModel
{
    /// <summary>
    ///     Gets or sets the question or prompt message to display to the user.
    /// </summary>
    /// <value>The question string. Defaults to <see cref="string.Empty" />.</value>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the default value to be used if the user cancels the input.
    /// </summary>
    /// <value>The default value string. Defaults to <see cref="string.Empty" />.</value>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether empty or whitespace input is allowed.
    /// </summary>
    /// <value>True if empty input is allowed; otherwise, false. Defaults to false.</value>
    public bool AllowedEmpty { get; set; } = false;
}