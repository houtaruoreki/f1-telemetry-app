namespace F1TelemetryApp.Services;

/// <summary>
/// Service interface for caching data in memory.
/// Provides generic caching with expiration support.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or default if not found or expired</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    /// <typeparam name="T">Type of value to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Cache expiration duration</param>
    void Set<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    /// <param name="key">Cache key to remove</param>
    void Remove(string key);

    /// <summary>
    /// Clears all cached values
    /// </summary>
    void Clear();

    /// <summary>
    /// Checks if a key exists and is not expired
    /// </summary>
    /// <param name="key">Cache key to check</param>
    /// <returns>True if key exists and not expired</returns>
    bool Exists(string key);
}
