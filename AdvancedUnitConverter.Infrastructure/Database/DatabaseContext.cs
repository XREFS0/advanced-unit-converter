using System.Data;
using Microsoft.Data.Sqlite;

namespace AdvancedUnitConverter.Infrastructure.Database
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string? dbPath = null)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolder = Path.Combine(appData, "AdvancedUnitConverter");
                Directory.CreateDirectory(appFolder);
                dbPath = Path.Combine(appFolder, "unit_converter.db");
            }

            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ConversionHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Category INTEGER NOT NULL,
                    FromUnitId TEXT NOT NULL,
                    FromUnitName TEXT NOT NULL,
                    FromUnitSymbol TEXT NOT NULL,
                    ToUnitId TEXT NOT NULL,
                    ToUnitName TEXT NOT NULL,
                    ToUnitSymbol TEXT NOT NULL,
                    InputValue REAL NOT NULL,
                    ResultValue REAL NOT NULL,
                    FormattedResult TEXT NOT NULL,
                    Timestamp TEXT NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_History_Timestamp ON ConversionHistory(Timestamp DESC);
                CREATE INDEX IF NOT EXISTS IX_History_Category ON ConversionHistory(Category);

                CREATE TABLE IF NOT EXISTS Favorites (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Category INTEGER NOT NULL,
                    FromUnitId TEXT NOT NULL,
                    FromUnitName TEXT NOT NULL,
                    FromUnitSymbol TEXT NOT NULL,
                    ToUnitId TEXT NOT NULL,
                    ToUnitName TEXT NOT NULL,
                    ToUnitSymbol TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UNIQUE(Category, FromUnitId, ToUnitId)
                );
            ";
            cmd.ExecuteNonQuery();
        }
    }
}
