namespace VBorsa_API.Application.Abstractions.Services.Redis;

public interface IRedisService
{
    Task AddAsync(decimal price, long retrievedAt, string symbol, string provider, string source);
    Task UpdateMaxOrLow(decimal price, string symbol);
    Task<decimal> GetLatestPriceBySymbol(string requestSymbol);
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
}