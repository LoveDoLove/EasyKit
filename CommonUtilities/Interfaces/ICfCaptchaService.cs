using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace CommonUtilities.Interfaces;

public interface ICfCaptchaService
{
    Task<bool> VerifyCaptchaAsync(string token); // Changed to async
    IHtmlContent GetCaptchaHtml();
    bool IsCaptchaResponseValid(HttpRequest request); // This method might need to become async too for consistency
}