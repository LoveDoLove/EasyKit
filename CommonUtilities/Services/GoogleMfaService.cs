using CommonUtilities.Interfaces;
using Google.Authenticator;
using Microsoft.Extensions.Logging;

namespace CommonUtilities.Services;

public class GoogleMfaService : IGoogleMfaService
{
    private readonly ILogger<GoogleMfaService> _logger;
    private readonly TwoFactorAuthenticator _twoFactorAuthenticator = new();

    public GoogleMfaService(ILogger<GoogleMfaService> logger)
    {
        _logger = logger;
    }

    public GoogleMfa GenerateMfa(string issuer, string email)
    {
        string secretKey = Guid.NewGuid().ToString().Replace("-", "")[..10];

        SetupCode? setupInfo = _twoFactorAuthenticator.GenerateSetupCode(issuer, email, secretKey, false);

        GoogleMfa googleMfa = new()
        {
            QrCodeUrl = setupInfo.QrCodeSetupImageUrl,
            ManualEntryCode = setupInfo.ManualEntryKey,
            SecretKey = secretKey
        };
        return googleMfa;
    }

    public bool ValidateMfa(string secretKey, string code)
    {
        return _twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, code);
    }
}