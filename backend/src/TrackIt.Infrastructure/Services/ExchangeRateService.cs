using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrackIt.Application.Interfaces;

namespace TrackIt.Infrastructure.Services;

public class ExchangeRateSettings
{
    public string AppId { get; set; } = "";
    public string BaseUrl { get; set; } = "https://openexchangerates.org/api/";
}

public class ExchangeRateService(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<ExchangeRateSettings> settings,
    ILogger<ExchangeRateService> logger
) : IExchangeRateService
{
    private const string CacheKeyPrefix = "exchange_rate_";
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default)
    {
        if (from == to) return 1m;
        var rates = await GetRatesAsync(to, [from], ct);
        return rates.GetValueOrDefault(from, 1m);
    }

    public async Task<Dictionary<string, decimal>> GetRatesAsync(
        string baseCurrency,
        IEnumerable<string> targetCurrencies,
        CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{baseCurrency}";

        if (cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cachedRates) && cachedRates is not null)
        {
            logger.LogDebug("Exchange rates served from cache for base {Base}", baseCurrency);
            return cachedRates;
        }

        try
        {
            var url = $"latest.json?app_id={settings.Value.AppId}&base={baseCurrency}";
            var response = await httpClient.GetFromJsonAsync<ExchangeRateResponse>(url, ct)
                ?? throw new InvalidOperationException("Empty response from exchange rate API.");

            var result = response.Rates;
            result[baseCurrency] = 1m;

            cache.Set(cacheKey, result, CacheDuration);
            logger.LogInformation("Exchange rates fetched from API for base {Base}", baseCurrency);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch exchange rates for {Base}. Using fallback rates.", baseCurrency);
            return targetCurrencies.ToDictionary(c => c, _ => 1m);
        }
    }

    private record ExchangeRateResponse(string Base, Dictionary<string, decimal> Rates);
}
