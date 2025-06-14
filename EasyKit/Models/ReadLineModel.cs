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