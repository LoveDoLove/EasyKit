using System.Text.Json;
using CommonUtilities.Interfaces;
using CommonUtilities.Models.Response.IpInfo;
using Microsoft.AspNetCore.Http;

namespace CommonUtilities.Services;

public class IpInfoService : IIpInfoService
{
    private const string IpV4InfoUrl = "https://ipinfo.io/";
    private const string IpV6InfoUrl = "https://v6.ipinfo.io/";
    private readonly IpInfo _ipInfo;

    public IpInfoService(IpInfo ipInfo)
    {
        _ipInfo = ipInfo;
    }

    public async Task<IpInfoResponse?> GetIpInfo(HttpContext context)
    {
        string clientIp = GetClientIp(context);

        HttpClient client = new HttpClient();
        string url = clientIp.Contains(":") ? IpV6InfoUrl : IpV4InfoUrl;
        HttpResponseMessage response = await client.GetAsync($"{url}{clientIp}?token={_ipInfo.Token}");
        string responseContent = await response.Content.ReadAsStringAsync();

        IpInfoResponse? ipInfo = JsonSerializer.Deserialize<IpInfoResponse>(responseContent);
        return ipInfo;
    }

    public string GetClientIp(HttpContext context)
    {
        string? ipAddress = "UNKNOWN";

        string[] headersToCheck =
        {
            "CF-Connecting-IP",
            "True-Client-IP",
            "HTTP_CLIENT_IP",
            "HTTP_X_FORWARDED_FOR",
            "HTTP_X_FORWARDED",
            "HTTP_FORWARDED_FOR",
            "HTTP_FORWARDED",
            "REMOTE_ADDR",
            "X-Forwarded-For"
        };

        foreach (string header in headersToCheck)
        {
            string? headerValue = context.Request.Headers[header].FirstOrDefault();

            if (string.IsNullOrEmpty(headerValue)) continue;

            ipAddress = headerValue.Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(ipAddress)) break;
        }

        if (string.IsNullOrEmpty(ipAddress)) ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (ipAddress == "::1") ipAddress = "127.0.0.1";

        return ipAddress ?? "UNKNOWN";
    }
}