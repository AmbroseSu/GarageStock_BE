using StackExchange.Redis;

namespace Service.Impl;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public Task<bool> SetStringAsync(string key, string value, TimeSpan ttl)
        => _db.StringSetAsync(key, value, ttl);

    public async Task<string?> GetStringAsync(string key)
    {
        var v = await _db.StringGetAsync(key);
        return v.HasValue ? v.ToString() : null;
    }

    public Task<bool> DeleteAsync(string key)
        => _db.KeyDeleteAsync(key);
}