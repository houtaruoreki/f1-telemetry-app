namespace F1TelemetryApp.Services;

using System.Collections.Concurrent;

/// <summary>
/// In-memory cache service implementation.
/// Thread-safe caching with automatic expiration.
/// </summary>
public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            // Check if entry has expired
            if (entry.ExpiresAt > DateTime.UtcNow)
            {
                return (T?)entry.Value;
            }
            else
            {
                // Remove expired entry
                _cache.TryRemove(key, out _);
            }
        }

        return default;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var entry = new CacheEntry
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.Add(expiration)
        };

        _cache[key] = entry;
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public bool Exists(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                // Remove expired entry
                _cache.TryRemove(key, out _);
            }
        }

        return false;
    }

    /// <summary>
    /// Internal class to store cached values with expiration
    /// </summary>
    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
