using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IGeoLocationService
    {
        Task<IpLookupResponse?> LookupIpAsync(string ip);
    }
}
