namespace CommonUtilities.Models;

public class GoogleMfa
{
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryCode { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}