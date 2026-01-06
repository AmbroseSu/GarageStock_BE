namespace Service;

public interface IRedisService
{
    Task<bool> SetStringAsync(string key, string value, TimeSpan ttl);
    Task<string?> GetStringAsync(string key);
    Task<bool> DeleteAsync(string key);
}