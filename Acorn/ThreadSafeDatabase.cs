using System.Collections.Concurrent;

namespace Acorn;

/// <summary>
/// Thread-safe in-memory database using ConcurrentDictionary.
/// Safe for concurrent access by multiple threads.
/// </summary>
public class ThreadSafeDatabase<T> : IDatabase<T>
{
    protected readonly ConcurrentDictionary<string, object> _store = new();

    public async Task<T> GetAsync(string key)
    {
        await Task.Yield();

        if (_store.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        throw new KeyNotFoundException($"Key '{key}' not found");
    }

    public async Task SetAsync(string key, T value)
    {
        await Task.Yield();
        _store[key] = (object?)value ?? throw new ArgumentNullException(nameof(value));
    }

    public async Task<bool> DeleteAsync(string key)
    {
        await Task.Yield();
        return _store.TryRemove(key, out _);
    }
}
