using System;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using FreeFramework.SDK.Runtime;
using FreeFramework.SDK.Storage;

namespace FreeFramework.SDK.Client.Storage;

public sealed class ClientStorageProvider : IStorageProvider {
    private static string NormalizeKey(string key) => key.Replace('/', '-').Replace('\\', '-').Replace(':', '-');
    
    private static string GetObjectPath(IModule callingModule, string key) {
        return $"__FF:{callingModule.Name}/{Constants.FFVersion}/{NormalizeKey(key)}.dat";
    }

    public T Load<T>(IModule callingModule, string key) where T : IBinarySerializable, new() {
        string path = GetObjectPath(callingModule, key);
        string? baseData = API.GetResourceKvpString(path);

        if (baseData is null) {
            throw new FileNotFoundException($"Storage file \"{path}\" was not found.");
        }
        
        byte[] data = Convert.FromBase64String(baseData);
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        
        var instance = new T();
        instance.Read(reader);

        return instance;
    }

    public Task<T> LoadAsync<T>(IModule callingModule, string key) where T : IBinarySerializable, new() {
        return Task.FromResult(Load<T>(callingModule, key));
    }

    public T Load<T>(IModule callingModule, string key, T fallback) where T : IBinarySerializable, new() {
        try {
            return Load<T>(callingModule, key);
        } catch (FileNotFoundException) {
            return fallback;
        }
    }

    public async Task<T> LoadAsync<T>(IModule callingModule, string key, T fallback) where T : IBinarySerializable, new() {
        try {
            return await LoadAsync<T>(callingModule, key);
        } catch (FileNotFoundException) {
            return fallback;
        }
    }

    public void Save<T>(IModule callingModule, string key, T instance) where T : IBinarySerializable {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        instance.Write(writer);

        byte[] data = ms.ToArray();
        string baseData = Convert.ToBase64String(data);
        
        string path = GetObjectPath(callingModule, key);
        API.SetResourceKvp(path, baseData);
    }

    public Task SaveAsync<T>(IModule callingModule, string key, T instance) where T : IBinarySerializable {
        Save(callingModule, key, instance);
        return Task.CompletedTask;
    }

    public bool Exists(IModule callingModule, string key) {
        string path = GetObjectPath(callingModule, key);
        return API.GetResourceKvpString(path) is not null;
    }

    public Task<bool> ExistsAsync(IModule callingModule, string key) {
        return Task.FromResult(Exists(callingModule, key));
    }

    public void Delete(IModule callingModule, string key) {
        if (!Exists(callingModule, key)) {
            return;
        }

        string path = GetObjectPath(callingModule, key);
        API.DeleteResourceKvp(path);
    }

    public Task DeleteAsync(IModule callingModule, string key) {
        Delete(callingModule, key);
        return Task.CompletedTask;
    }
}