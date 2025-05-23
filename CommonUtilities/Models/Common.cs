namespace CommonUtilities.Models;

public static class Common
{
    public const string AppName = "JX & KQ Hotel";

    // Validation
    public const string PhoneNumber = @"^(\+)?[0-9]{10,15}$";
    public const int PasswordLength = 8;

    // Error Message
    public const string ErrorMessage = "Invalid {0}.";
    public const string InvalidRequestErrorMessage = "Invalid request.";

    public const string PasswordErrorMessage = "Password must be at least 8 characters long.";
    public const string PasswordDoesNotMatchErrorMessage = "Password does not match.";

    public const string EmailExistsErrorMessage = "Email already exists.";
    public const string PhoneNumberErrorMessage = "Invalid {0}";

    public const string CaptchaErrorMessage = "Captcha is required.";

    public const string PleaseLoginToContinueErrorMessage = "Please login to continue.";

    // WWWRoot Path
    public const string ImagePath = "images/";
    public const string ImageUploadPath = "uploads/";
}

public static class AuthorizationPolicies
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string RequireAdminOrHigher = "RequireAdminOrHigher";
    public const string AuthenticatedUsersOnly = "AuthenticatedUsersOnly";
    public const string UserOnly = "UserOnly";
}

public static class PaymentInit
{
    public const string Currency = "myr";
    public const decimal TaxRate = 8;
    public const decimal ServiceCharge = 10;
}

public static class SweetAlert
{
    public static readonly string Info = "Info";
    public static readonly string Warning = "Warning";
    public static readonly string Success = "Success";
    public static readonly string Danger = "Danger";
}

/**
 * @author: LoveDoLove
 * @description: Email design for OTP, Reset Password, Illegal Login, Password Changed
 */
public static class EmailDesign
{
    public const string TokenSubject = "Token";
    public const string ResetPasswordSubject = "Reset Password";
    public const string IllegalLoginSubject = "Illegal Login";
    public const string PasswordChangedSubject = "Password Changed";
    public const string AccountCreatedSubject = "Account Created";
    public const string LoginAlertSubject = "Login Alert";
    public const string MfaAlertSubject = "MFA Alert";
    public const string ResetMfaSubject = "Reset MFA";
    public const string PaymentReceiptSubject = "Payment Receipt";
    public const string StripePaymentReceiptSubject = "Stripe";
    public const string OrderCancelRequestSubject = "Order Cancel Request";
    public const string OrderRefundedSubject = "Order Refunded";
}