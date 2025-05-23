using System.Text.Json.Serialization;

namespace CommonUtilities.Models.Response.CfCaptcha;

public class CfCaptchaResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
}