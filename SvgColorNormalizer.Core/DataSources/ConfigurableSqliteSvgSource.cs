using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class ConfigurableSqliteSvgSource : ConfigurableDataSourceBase
    {
        private string ConnectionString => GetConnectionString();

        public override string SourceType => "SQLite数据库";

        public override async Task<IEnumerable<SvgData>> LoadAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            var results = new List<SvgData>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name, svg_content FROM svg_assets";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string content = reader.GetString(2);

                results.Add(new SvgData
                {
                    Source = name,
                    Content = content,
                    Metadata = new Dictionary<string, object>
                    {
                        ["dbId"] = id,
                        ["connectionString"] = ConnectionString
                    }
                });
            }

            return results;
        }

        public override bool ValidateConfiguration()
        {
            return Configuration.ContainsKey("DatabasePath");
        }

        public override async Task InitializeAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS svg_assets (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    svg_content TEXT NOT NULL,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );
            ";

            await command.ExecuteNonQueryAsync();
        }

        private string GetConnectionString()
        {
            var databasePath = Configuration["DatabasePath"];
            if (!Path.IsPathRooted(databasePath))
            {
                databasePath = Path.Combine(Directory.GetCurrentDirectory(), databasePath);
            }

            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return $"Data Source={databasePath}";
        }
    }
}
