using System.Text.Json.Serialization;

namespace CommonUtilities.Models.Response.IpInfo;

public class IpInfoResponse
{
    [JsonPropertyName("ip")] public string? Ip { get; set; } = string.Empty;

    [JsonPropertyName("city")] public string? City { get; set; } = string.Empty;

    [JsonPropertyName("region")] public string? Region { get; set; } = string.Empty;

    [JsonPropertyName("country")] public string? Country { get; set; } = string.Empty;

    [JsonPropertyName("loc")] public string? Loc { get; set; } = string.Empty;

    [JsonPropertyName("org")] public string? Org { get; set; } = string.Empty;

    [JsonPropertyName("postal")] public string? Postal { get; set; } = string.Empty;

    [JsonPropertyName("timezone")] public string? Timezone { get; set; } = string.Empty;
}