using System.Collections.Concurrent;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class BlockedCountryService : IBlockedCountryService
    {
        // keyed by uppercase country code
        private readonly ConcurrentDictionary<string, BlockedCountry> _store = new();

        public bool TryBlockCountry(string code, string name)
        {
            code = code.ToUpperInvariant();
            return _store.TryAdd(code, new BlockedCountry { CountryCode = code, CountryName = name });
        }

        public bool TryRemoveBlockedCountry(string code)
        {
            code = code.ToUpperInvariant();
            return _store.TryRemove(code, out _);
        }

        public IEnumerable<BlockedCountry> GetBlockedCountries()
            => _store.Values.OrderBy(c => c.CountryCode);

        public bool IsCountryBlocked(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            code = code.ToUpperInvariant();
            if (_store.TryGetValue(code, out var item))
            {
                // if temporal, check expiry
                if (item.TemporalUntilUtc.HasValue && item.TemporalUntilUtc.Value <= DateTime.UtcNow)
                {
                    // expired -> remove
                    _store.TryRemove(code, out _);
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool TryTemporalBlock(string code, string name, int durationMinutes, out string? error)
        {
            error = null;
            code = code.ToUpperInvariant();
            if (durationMinutes < 1 || durationMinutes > 1440)
            {
                error = "durationMinutes must be between 1 and 1440";
                return false;
            }

            if (_store.TryGetValue(code, out var existing))
            {
                // if existing temporal and not expired -> conflict
                if (existing.TemporalUntilUtc.HasValue && existing.TemporalUntilUtc > DateTime.UtcNow)
                {
                    error = "Country is already temporarily blocked";
                    return false;
                }
                // if permanently blocked, also treat as conflict
                if (!existing.TemporalUntilUtc.HasValue)
                {
                    error = "Country is already permanently blocked";
                    return false;
                }
            }

            var until = DateTime.UtcNow.AddMinutes(durationMinutes);
            var bc = new BlockedCountry { CountryCode = code, CountryName = name, TemporalUntilUtc = until };
            _store.AddOrUpdate(code, bc, (_, __) => bc);
            return true;
        }
    }

}
