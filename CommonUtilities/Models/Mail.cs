namespace CommonUtilities.Models;

public class Mail
{
    public string[] To { get; set; } = [];
    public string[]? Cc { get; set; }
    public string[]? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}