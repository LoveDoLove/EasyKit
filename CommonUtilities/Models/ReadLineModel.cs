namespace CommonUtilities.Models;

public class ReadLineModel
{
    public string question { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
    public bool allowedEmpty { get; set; } = false;
}