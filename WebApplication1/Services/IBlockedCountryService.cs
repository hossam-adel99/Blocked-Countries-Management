using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IBlockedCountryService
    {
        bool TryBlockCountry(string code, string name);
        bool TryRemoveBlockedCountry(string code);
        IEnumerable<BlockedCountry> GetBlockedCountries();
        bool IsCountryBlocked(string code);
        bool TryTemporalBlock(string code, string name, int durationMinutes, out string? error);
    }
}
