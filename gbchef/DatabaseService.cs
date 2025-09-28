using gbchef.Models;
using Microsoft.Data.Sqlite;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO;

public class DatabaseService : IDisposable
{
    private readonly string _dbPath;
    private SqliteConnection _connection;


    public DatabaseService()
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.db");
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

    public async Task<IEnumerable<Tuple<int,int>>> ExecuteSelectRecipeSlotMapByIngredientId(int id)
    {
        var results = new List<Tuple<int, int>>();
        
        using var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM recipe_item_map WHERE item_id = {id}";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            object[] row = new object[reader.FieldCount];
            reader.GetValues(row);

            int recipeId = Convert.ToInt32(row[0]);
            int ingredientId = Convert.ToInt32(row[1]);
            int in1 = Convert.ToInt32(row[2]);
            int in2 = Convert.ToInt32(row[3]);
            int in3 = Convert.ToInt32(row[4]);
            int in4 = Convert.ToInt32(row[5]);
            
            var slots = new List<int> { in1, in2, in3, in4 };
            for (var i = 0; i <= 3; i++)
            {
                if (slots[i] == 1)
                {
                    results.Add(new Tuple<int, int>(recipeId, i+1));
                }
            }
        }
        
        return results;
    }
}