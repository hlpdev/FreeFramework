using System.IO;
using System.Threading.Tasks;
using FreeFramework.SDK.Runtime;

namespace FreeFramework.SDK.Storage;

/// <summary>
/// Used for storing key-value pairs in any context (server or client).
/// </summary>
public interface IStorageProvider {
    /// <summary>
    /// Loads the value stored at the specified key.
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <typeparam name="T">The value type</typeparam>
    /// <returns>The object stored at the specified key</returns>
    /// <exception cref="FileNotFoundException">Throws when there is no value at the specified key</exception>
    T Load<T>(IModule callingModule, string key) where T : IBinarySerializable, new();
    
    /// <summary>
    /// Loads the value stored at the specified key.
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <typeparam name="T">The value type</typeparam>
    /// <returns>The object stored at the specified key</returns>
    /// <exception cref="FileNotFoundException">Throws when there is no value at the specified key</exception>
    Task<T> LoadAsync<T>(IModule callingModule, string key) where T : IBinarySerializable, new();
    
    /// <summary>
    /// Loads the value stored at the specified key.
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <param name="fallback">A fallback value to return if the key doesn't exist</param>
    /// <typeparam name="T">The value type</typeparam>
    /// <returns>The object stored at the specified key</returns>
    T Load<T>(IModule callingModule, string key, T fallback) where T : IBinarySerializable, new();
    
    /// <summary>
    /// Loads the value stored at the specified key.
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <param name="fallback">A fallback value to return if the key doesn't exist</param>
    /// <typeparam name="T">The value type</typeparam>
    /// <returns>The object stored at the specified key</returns>
    Task<T> LoadAsync<T>(IModule callingModule, string key, T fallback) where T : IBinarySerializable, new();
    
    /// <summary>
    /// Saves the value provided to the specified key
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <param name="instance">The object to store</param>
    /// <typeparam name="T">The value type</typeparam>
    void Save<T>(IModule callingModule, string key, T instance) where T : IBinarySerializable;
    
    /// <summary>
    /// Saves the value provided to the specified key
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <param name="instance">The object to store</param>
    /// <typeparam name="T">The value type</typeparam>
    Task SaveAsync<T>(IModule callingModule, string key, T instance) where T : IBinarySerializable;
    
    /// <summary>
    /// Checks if the specified key has a value
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <returns>True if the key has a value</returns>
    bool Exists(IModule callingModule, string key);
    
    /// <summary>
    /// Checks if the specified key has a value
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    /// <returns>True if the key has a value</returns>
    Task<bool> ExistsAsync(IModule callingModule, string key);
    
    /// <summary>
    /// Removes the value from the specified key
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    void Delete(IModule callingModule, string key);
    
    /// <summary>
    /// Removes the value from the specified key
    /// </summary>
    /// <param name="callingModule">The module that is calling this method</param>
    /// <param name="key">The storage key</param>
    Task DeleteAsync(IModule callingModule, string key);
}