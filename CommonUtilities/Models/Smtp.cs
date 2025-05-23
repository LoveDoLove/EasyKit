namespace CommonUtilities.Models;

public class Smtp
{
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
}