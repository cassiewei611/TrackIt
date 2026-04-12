using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrackIt.Application.Interfaces;

namespace TrackIt.Infrastructure.Services;

// Uses Frankfurter API (api.frankfurter.app) — free, no API key required, ECB data.
// Internally always fetches with USD as base, then computes cross-rates.
public class ExchangeRateService(
    HttpClient httpClient,
    IMemoryCache cache,
    ILogger<ExchangeRateService> logger
) : IExchangeRateService
{
    private const string UsdRatesCacheKey = "fx_usd_base";
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(4);

    public async Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default)
    {
        if (from == to) return 1m;
        var rates = await GetRatesAsync(to, [from], ct);
        return rates.GetValueOrDefault(from, 1m);
    }

    // Returns a dictionary where each value is:
    //   "how much 1 unit of that source currency is worth in targetCurrency"
    // e.g. GetRatesAsync("CNY", ["USD", "EUR"]) → {"USD": 7.27, "EUR": 7.9}
    public async Task<Dictionary<string, decimal>> GetRatesAsync(
        string targetCurrency,
        IEnumerable<string> sourceCurrencies,
        CancellationToken ct = default)
    {
        var usdRates = await GetUsdBaseRatesAsync(ct);

        // usdRates["CNY"] = 7.27 means 1 USD = 7.27 CNY
        var targetPerUsd = usdRates.GetValueOrDefault(targetCurrency, 1m);

        var result = new Dictionary<string, decimal>();
        foreach (var src in sourceCurrencies)
        {
            var srcPerUsd = usdRates.GetValueOrDefault(src, 1m);
            // 1 src = (targetPerUsd / srcPerUsd) target
            result[src] = srcPerUsd == 0 ? 1m : targetPerUsd / srcPerUsd;
        }

        return result;
    }

    private async Task<Dictionary<string, decimal>> GetUsdBaseRatesAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(UsdRatesCacheKey, out Dictionary<string, decimal>? cached) && cached is not null)
            return cached;

        try
        {
            var response = await httpClient.GetFromJsonAsync<FrankfurterResponse>("latest?from=USD", ct)
                ?? throw new InvalidOperationException("Empty response from Frankfurter API.");

            var rates = response.Rates;
            rates["USD"] = 1m;

            cache.Set(UsdRatesCacheKey, rates, CacheDuration);
            logger.LogInformation("Exchange rates fetched from Frankfurter (USD base)");

            return rates;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch exchange rates. Using 1:1 fallback.");
            return new Dictionary<string, decimal> { ["USD"] = 1m };
        }
    }

    private record FrankfurterResponse(string Base, Dictionary<string, decimal> Rates);
}
