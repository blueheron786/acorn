using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Acorn;

/// <summary>
/// Thread-safe database with indexing and pattern matching capabilities.
/// </summary>
public class IndexedDatabase<T> : ThreadSafeDatabase<T>
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _indexes = new();

    /// <summary>
    /// Adds a key to an index for fast lookups.
    /// </summary>
    /// <param name="indexName">The name of the index (e.g., "age", "city").</param>
    /// <param name="key">The key to index.</param>
    /// <param name="indexValue">The value to index by (e.g., "25", "NYC").</param>
    public async Task AddToIndexAsync(string indexName, string key, string indexValue)
    {
        await Task.Yield();

        if (!_indexes.ContainsKey(indexName))
        {
            _indexes[indexName] = new HashSet<string>();
        }

        _indexes[indexName].Add($"{indexValue}:{key}");
    }

    /// <summary>
    /// Retrieves keys from an index by value.
    /// </summary>
    /// <param name="indexName">The name of the index to query.</param>
    /// <param name="indexValue">The value to search for.</param>
    /// <returns>A set of keys matching the index value.</returns>
    public async Task<HashSet<string>> GetFromIndexAsync(string indexName, string indexValue)
    {
        await Task.Yield();

        if (_indexes.TryGetValue(indexName, out var index))
        {
            return new HashSet<string>(
                index.Where(entry => entry.StartsWith($"{indexValue}:"))
                     .Select(entry => entry.Split(':')[1])
            );
        }

        return new HashSet<string>();
    }

    /// <summary>
    /// Finds keys matching a wildcard pattern.
    /// </summary>
    /// <param name="pattern">Pattern with * as wildcard (e.g., "user:*:name").</param>
    /// <returns>List of keys matching the pattern.</returns>
    public async Task<List<string>> FindKeysAsync(string pattern)
    {
        await Task.Yield();

        // Convert wildcard pattern to regex
        var regex = new Regex("^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$");
        return _store.Keys.Where(key => regex.IsMatch(key)).ToList();
    }
}
