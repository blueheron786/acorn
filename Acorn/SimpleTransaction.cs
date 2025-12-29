namespace Acorn;

/// <summary>
/// Simple transaction implementation that applies all changes atomically on commit.
/// </summary>
public class SimpleTransaction : ITransaction
{
    private readonly Dictionary<string, object> _changes = new();
    private readonly HashSet<string> _deletions = new();
    private bool _committed = false;

    /// <summary>
    /// Gets the pending changes dictionary.
    /// </summary>
    protected Dictionary<string, object> Changes => _changes;

    /// <summary>
    /// Gets the pending deletions set.
    /// </summary>
    protected HashSet<string> Deletions => _deletions;

    public async Task SetAsync<T>(string key, T value)
    {
        await Task.Yield();
        _changes[key] = (object?)value ?? throw new ArgumentNullException(nameof(value));
        _deletions.Remove(key);
    }

    public async Task DeleteAsync(string key)
    {
        await Task.Yield();
        _deletions.Add(key);
        _changes.Remove(key);
    }

    /// <summary>
    /// Commits the transaction by applying changes to the provided database.
    /// </summary>
    /// <param name="applyChanges">Function that applies a set operation.</param>
    /// <param name="applyDeletions">Function that applies a delete operation.</param>
    public async Task CommitAsync(
        Func<string, object, Task> applyChanges,
        Func<string, Task> applyDeletions)
    {
        if (_committed) return;

        foreach (var change in _changes)
        {
            await applyChanges(change.Key, change.Value);
        }

        foreach (var deletion in _deletions)
        {
            await applyDeletions(deletion);
        }

        _committed = true;
    }

    public async Task CommitAsync()
    {
        await Task.Yield();
        _committed = true;
    }

    public async Task RollbackAsync()
    {
        await Task.Yield();
        _changes.Clear();
        _deletions.Clear();
    }
}
