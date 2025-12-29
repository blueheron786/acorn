using System.Collections.Concurrent;

namespace Acorn;

/// <summary>
/// In-memory database with expiration support for caching scenarios.
/// </summary>
public class ExpiringDatabase<T> : IDatabase<T>
{
    private readonly ConcurrentDictionary<string, CacheItem> _store = new();

    /// <summary>
    /// Stores a value with an optional expiration time.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">Optional time-to-live. If null, the value never expires.</param>
    public async Task SetAsync(string key, T value, TimeSpan? expiry = null)
    {
        await Task.Yield();

        var item = new CacheItem
        {
            Value = (object?)value ?? throw new ArgumentNullException(nameof(value)),
            ExpiresAt = expiry.HasValue ? DateTime.Now.Add(expiry.Value) : DateTime.MaxValue
        };

        _store[key] = item;
    }

    public async Task<T> GetAsync(string key)
    {
        await Task.Yield();

        if (_store.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _store.TryRemove(key, out _);
                throw new KeyNotFoundException($"Key '{key}' has expired");
            }

            return (T)item.Value;
        }

        throw new KeyNotFoundException($"Key '{key}' not found");
    }

    Task IDatabase<T>.SetAsync(string key, T value)
    {
        return SetAsync(key, value, null);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        await Task.Yield();
        return _store.TryRemove(key, out _);
    }

    private class CacheItem
    {
        public object Value { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.Now > ExpiresAt;
    }
}
