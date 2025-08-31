using System.Globalization;
using System.Text.Json;
using StackExchange.Redis;
using VBorsa_API.Application.Abstractions.Services.Redis;

namespace VBorsa_API.Infrastructure.Repositories.Redis;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(ConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task AddAsync(decimal price, long retrievedAt, string symbol, string provider, string source)
    {
        var key = $"quotes:{symbol}";
        var value = $"{price}:{retrievedAt}:{source}:{provider}";
        await _db.ListLeftPushAsync(key, value);
        await _db.ListTrimAsync(key, 0, 999);
    }

    public async Task UpdateMaxOrLow(decimal price, string symbol)
    {
        var key = $"metrics:{symbol}";
        var hashEntries = await _db.HashGetAllAsync(key);

        decimal currentHigh = 0;
        decimal currentLow = decimal.MaxValue;

        if (hashEntries.Any())
        {
            var highEntry = hashEntries.FirstOrDefault(x => x.Name == "high");
            if (highEntry.Value.HasValue)
            {
                currentHigh = decimal.Parse(highEntry.Value!);
            }

            var lowEntry = hashEntries.FirstOrDefault(x => x.Name == "low");
            if (lowEntry.Value.HasValue)
            {
                currentLow = decimal.Parse(lowEntry.Value!);
            }
        }
        var updatedHigh = price > currentHigh ? price : currentHigh;
        var updatedLow = price < currentLow ? price : currentLow;

        var hashFields = new HashEntry[]
        {
            new HashEntry("last", price.ToString(CultureInfo.CurrentCulture)),
            new HashEntry("high", updatedHigh.ToString(CultureInfo.CurrentCulture)),
            new HashEntry("low", updatedLow.ToString(CultureInfo.CurrentCulture))
        };

        await _db.HashSetAsync(key, hashFields);
    }

    public async Task<decimal> GetLatestPriceBySymbol(string requestSymbol)
    {
        var key = $"quotes:{requestSymbol}";
        var latestQuoteString = await _db.ListGetByIndexAsync(key, 0);
        if (!latestQuoteString.HasValue) return 0m;
        var parts = latestQuoteString.ToString().Split(':');
        if (parts.Length > 0 && decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price))
        {
            return price;
        }
        return 0m;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue)
        {
            return default; 
        }
        return JsonSerializer.Deserialize<T>(value!);    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return await _db.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }
}