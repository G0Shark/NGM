using System.Text.Json;
using StackExchange.Redis;

namespace Backend.Core.Services;

public interface IRedisCacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _redisDb;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _redisDb = _redis.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _redisDb.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        await _redisDb.StringSetAsync(
            key, 
            JsonSerializer.Serialize(value), 
            expiry ?? TimeSpan.FromMinutes(30)
        );
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDb.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisDb.KeyExistsAsync(key);
    }
}