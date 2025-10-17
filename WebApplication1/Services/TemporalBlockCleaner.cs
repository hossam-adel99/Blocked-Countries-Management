using Microsoft.Extensions.Hosting;

namespace WebApplication1.Services
{
    public class TemporalBlockCleaner : BackgroundService
    {
        private readonly IBlockedCountryService _blockedSrv;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public TemporalBlockCleaner(IBlockedCountryService blockedSrv)
        {
            _blockedSrv = blockedSrv;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Clean expired temporal blocks by enumerating and checking
                var toRemove = _blockedSrv.GetBlockedCountries()
                    .Where(c => c.TemporalUntilUtc.HasValue && c.TemporalUntilUtc.Value <= DateTime.UtcNow)
                    .Select(c => c.CountryCode)
                    .ToList();

                foreach (var code in toRemove)
                    _blockedSrv.TryRemoveBlockedCountry(code);

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }

}
