using Microsoft.Data.Sqlite;
using System.IO;

public class DatabaseService : IDisposable
{
    private readonly string _dbPath;
    private SqliteConnection _connection;


    public DatabaseService()
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.db");
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try {
            // Verify file exists
            if (!File.Exists(_dbPath))
                throw new FileNotFoundException($"Database file not found: {_dbPath}");

            // Verify read/write access
            if (!HasWriteAccess(_dbPath))
                throw new UnauthorizedAccessException($"No write access to database file: {_dbPath}");

            // Open connection
            _connection = new SqliteConnection($"Data Source={_dbPath}");
            _connection.Open();
        }
        catch (Exception) {
            throw;
        }
    }

    private static bool HasWriteAccess(string path)
    {
        try
        {
            using var fs = File.Create(Path.Combine(Path.GetDirectoryName(path), Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<object[]>> ExecuteSelectAllAsync(string tableName)
    {
        var results = new List<object[]>();

        using var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            object[] row = new object[reader.FieldCount];
            reader.GetValues(row);
            results.Add(row);
        }

        return results;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _connection?.Dispose();
    }
}