namespace Acorn;

/// <summary>
/// Core interface for key-value database operations.
/// </summary>
/// <typeparam name="T">The type of value to store and retrieve.</typeparam>
public interface IDatabase<T>
{
    /// <summary>
    /// Retrieves a value by its key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
    Task<T> GetAsync(string key);

    /// <summary>
    /// Stores or updates a value with the specified key.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The value to store.</param>
    Task SetAsync(string key, T value);

    /// <summary>
    /// Deletes a key-value pair from the database.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    /// <returns>True if the key was deleted, false if it didn't exist.</returns>
    Task<bool> DeleteAsync(string key);
}
