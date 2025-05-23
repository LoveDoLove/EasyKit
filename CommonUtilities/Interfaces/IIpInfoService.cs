using CommonUtilities.Models.Response.IpInfo;
using Microsoft.AspNetCore.Http;

namespace CommonUtilities.Interfaces;

public interface IIpInfoService
{
    Task<IpInfoResponse?> GetIpInfo(HttpContext context);
    string GetClientIp(HttpContext context);
}