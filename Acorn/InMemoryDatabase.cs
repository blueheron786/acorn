namespace Acorn;

/// <summary>
/// Basic in-memory key-value database implementation using Dictionary.
/// Not thread-safe - use ThreadSafeDatabase for concurrent access.
/// </summary>
public class InMemoryDatabase<T> : IDatabase<T>
{
    private readonly Dictionary<string, object> _store = new();

    public async Task<T> GetAsync(string key)
    {
        await Task.Yield(); // Make it properly async

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
        return _store.Remove(key);
    }
}
