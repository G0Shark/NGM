using System.Text.Json;
using StackExchange.Redis;

namespace NGMapi.Infrastructure.Services;

public interface IRedisCacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiry = null);
    void Remove(string key);
    bool Exists(string key);
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

    public T Get<T>(string key)
    {
        var value = _redisDb.StringGet(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
    }

    public void Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        _redisDb.StringSet(
            key, 
            JsonSerializer.Serialize(value), 
            expiry ?? TimeSpan.FromMinutes(30)
        );
    }

    public void Remove(string key)
    {
        _redisDb.KeyDeleteAsync(key);
    }

    public bool Exists(string key)
    {
        return _redisDb.KeyExists(key);
    }
}