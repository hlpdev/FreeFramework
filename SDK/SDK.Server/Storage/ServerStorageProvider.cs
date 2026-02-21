using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using FreeFramework.SDK.Runtime;
using FreeFramework.SDK.Storage;
using MySqlConnector;

namespace FreeFramework.SDK.Server.Storage;

public sealed class ServerStorageProvider : IStorageProvider {
    private readonly string _connectionString;
    
    public ServerStorageProvider(string connectionString) {
        _connectionString = connectionString;

        using var connection = new MySqlConnection(_connectionString);
        try {
            connection.Open();
        } catch (Exception) {
            Debug.WriteLine("FATAL: Failed to connect to the MariaDb / MySql instance!");
            throw;
        }
        
        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              CREATE TABLE IF NOT EXISTS ff_storage (
                                  module VARCHAR(128) NOT NULL,
                                  storage_key VARCHAR(255) NOT NULL,
                                  payload LONGBLOB NOT NULL,
                                  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                  PRIMARY KEY (module, storage_key)
                              );
                              """;

        command.ExecuteNonQuery();
    }
    
    private static string NormalizeKey(string key) => key.Replace('/', '-').Replace('\\', '-').Replace(':', '-');
    
    public T Load<T>(IModule callingModule, string key) where T : IBinarySerializable, new() {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              SELECT payload
                              FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              LIMIT 1
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));

        using MySqlDataReader reader = command.ExecuteReader();

        if (!reader.Read()) {
            throw new FileNotFoundException("Storage entry not found");
        }

        var bytes = (byte[])reader["payload"];

        using var ms = new MemoryStream(bytes);
        using var br = new BinaryReader(ms);

        var instance = new T();
        instance.Read(br);

        return instance;
    }

    public async Task<T> LoadAsync<T>(IModule callingModule, string key) where T : IBinarySerializable, new() {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              SELECT payload
                              FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              LIMIT 1
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));
        
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync()) {
            throw new FileNotFoundException("Storage entry not found");
        }

        var bytes = (byte[])reader["payload"];
        
        using var ms = new MemoryStream(bytes);
        using var br = new BinaryReader(ms);

        var instance = new T();
        instance.Read(br);

        return instance;
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
        using var bw = new BinaryWriter(ms, Encoding.UTF8, true);
        instance.Write(bw);

        byte[] bytes = ms.ToArray();

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              INSERT INTO ff_storage (module, storage_key, payload)
                              VALUES (@module, @key, @payload)
                              ON DUPLICATE KEY UPDATE payload = @payload
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));
        command.Parameters.AddWithValue("@payload", bytes);

        command.ExecuteNonQuery();
    }

    public async Task SaveAsync<T>(IModule callingModule, string key, T instance) where T : IBinarySerializable {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8, true);
        instance.Write(bw);

        byte[] bytes = ms.ToArray();
        
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              INSERT INTO ff_storage (module, storage_key, payload)
                              VALUES (@module, @key, @payload)
                              ON DUPLICATE KEY UPDATE payload = @payload
                              """;
        
        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));
        command.Parameters.AddWithValue("@payload", bytes);
        
        await command.ExecuteNonQueryAsync();
    }

    public bool Exists(IModule callingModule, string key) {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              SELECT 1
                              FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              LIMIT 1
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));

        return command.ExecuteScalar() is not null;
    }

    public async Task<bool> ExistsAsync(IModule callingModule, string key) {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              SELECT 1
                              FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              LIMIT 1
                              """;
        
        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));
        
        object? result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public void Delete(IModule callingModule, string key) {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              DELETE FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));

        command.ExecuteNonQuery();
    }

    public async Task DeleteAsync(IModule callingModule, string key) {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
                              DELETE FROM ff_storage
                              WHERE module = @module
                                AND storage_key = @key
                              """;

        command.Parameters.AddWithValue("@module", callingModule.Name);
        command.Parameters.AddWithValue("@key", NormalizeKey(key));
        
        await command.ExecuteNonQueryAsync();
    }
}