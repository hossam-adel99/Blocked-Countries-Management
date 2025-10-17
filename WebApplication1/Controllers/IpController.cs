using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IGeoLocationService _geo;
        private readonly IBlockedCountryService _blocked;
        private readonly ILogService _logs;

        public IpController(IGeoLocationService geo, IBlockedCountryService blocked, ILogService logs)
        {
            _geo = geo; _blocked = blocked; _logs = logs;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
        {
            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ipAddress == "::1")
                ipAddress = "156.209.158.64";

            if (!IPAddress.TryParse(ipAddress, out _)) return BadRequest("Invalid IP");

            var info = await _geo.LookupIpAsync(ipAddress);
            if (info == null) return StatusCode(502, "Geo lookup failed");
            return Ok(info);
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ip == "::1") 
                ip = "156.209.158.64";

            if (string.IsNullOrEmpty(ip) || !System.Net.IPAddress.TryParse(ip, out _))
                return BadRequest("Cannot determine caller IP");

            var info = await _geo.LookupIpAsync(ip);
            if (info == null) return StatusCode(502, "Geo lookup failed");

            var blocked = _blocked.IsCountryBlocked(info.CountryCode ?? string.Empty);

            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();  

            // log attempt
            _logs.Add(new LogEntry
            {
                Ip = ip,
                TimestampUtc = DateTime.UtcNow,
                CountryCode = info.CountryCode ?? "Unknown",
                Blocked = blocked,
                UserAgent = Request.Headers["User-Agent"].ToString()
            });

            return Ok(new { Ip = ip, Country = info.Country, CountryCode = info.CountryCode, Blocked = blocked });
        }
    }

}
