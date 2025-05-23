using System.Text.Json;
using CommonUtilities.Interfaces;
using CommonUtilities.Models.Response.CfCaptcha;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace CommonUtilities.Services;

public class CfCaptchaService : ICfCaptchaService
{
    private const string CfCaptchaUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
    private const string CfCaptchaHtml = "<div class='cf-turnstile' data-sitekey='CF_CAPTCHA_SITE_KEY'></div>";
    private const string CfTurnstileResponse = "cf-turnstile-response";
    private static readonly HttpClient httpClient = new();

    private readonly CfCaptcha _cfCaptcha;


    public CfCaptchaService(CfCaptcha cfCaptcha)
    {
        _cfCaptcha = cfCaptcha;
    }

    public async Task<bool> VerifyCaptchaAsync(string token)
    {
        FormUrlEncodedContent content = new([
            new KeyValuePair<string, string>("secret", _cfCaptcha.SecretKey),
            new KeyValuePair<string, string>("response", token)
        ]);
        HttpResponseMessage response = await httpClient.PostAsync(CfCaptchaUrl, content);
        response.EnsureSuccessStatusCode(); // Throw if not successful
        string responseContent = await response.Content.ReadAsStringAsync();
        CfCaptchaResponse? captchaResponse = JsonSerializer.Deserialize<CfCaptchaResponse>(responseContent);
        return captchaResponse != null && captchaResponse.Success;
    }


    public IHtmlContent GetCaptchaHtml()
    {
        return new HtmlString(CfCaptchaHtml.Replace("CF_CAPTCHA_SITE_KEY", _cfCaptcha.SiteKey));
    }

    public bool IsCaptchaResponseValid(HttpRequest request)
    {
        string? turnstileResponse = request.Form[CfTurnstileResponse];
        if (string.IsNullOrEmpty(turnstileResponse)) return false;
        return VerifyCaptchaAsync(turnstileResponse).GetAwaiter().GetResult();
    }
}