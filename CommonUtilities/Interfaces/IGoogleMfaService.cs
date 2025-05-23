namespace CommonUtilities.Interfaces;

public interface IGoogleMfaService
{
    GoogleMfa GenerateMfa(string issuer, string email);
    bool ValidateMfa(string secretKey, string code);
}