using System.Text.Json;

namespace Acorn;

/// <summary>
/// Thread-safe database with persistence to disk using JSON serialization.
/// Auto-saves on every write operation.
/// </summary>
public class PersistentDatabase<T> : ThreadSafeDatabase<T>
{
    private readonly string _filePath;

    /// <summary>
    /// Creates a persistent database that loads from and saves to the specified file.
    /// </summary>
    /// <param name="filePath">Path to the JSON file for persistence.</param>
    public PersistentDatabase(string filePath)
    {
        _filePath = filePath;
        LoadFromDisk();
    }

    /// <summary>
    /// Saves the entire database to disk as JSON.
    /// </summary>
    public async Task SaveToDiskAsync()
    {
        await Task.Yield();

        var data = new Dictionary<string, object>(_store);
        var json = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private void LoadFromDisk()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data != null)
            {
                foreach (var kvp in data)
                {
                    _store[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    public new async Task SetAsync(string key, T value)
    {
        await base.SetAsync(key, value);
        await SaveToDiskAsync(); // Auto-save on every change
    }
}
