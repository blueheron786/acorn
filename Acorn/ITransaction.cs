namespace Acorn;

/// <summary>
/// Interface for database transactions that support atomic operations.
/// </summary>
public interface ITransaction
{
    /// <summary>
    /// Queues a value to be set when the transaction commits.
    /// </summary>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Queues a key to be deleted when the transaction commits.
    /// </summary>
    Task DeleteAsync(string key);

    /// <summary>
    /// Commits all changes atomically to the database.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Discards all pending changes without applying them.
    /// </summary>
    Task RollbackAsync();
}
