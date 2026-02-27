using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class SqliteSvgStorage : ISvgStorage
    {
        private readonly string _connectionString;

        public SqliteSvgStorage(string databasePath)
        {
            if (!Path.IsPathRooted(databasePath))
            {
                databasePath = Path.Combine(Directory.GetCurrentDirectory(), databasePath);
            }

            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connectionString = $"Data Source={databasePath}";
            InitializeDatabase().Wait();
        }

        public string StorageType => "SQLite数据库";

        public async Task SaveAsync(IEnumerable<SvgData> svgDataList)
        {
            foreach (var svgData in svgDataList)
            {
                await SaveAsync(svgData);
            }
        }

        public async Task SaveAsync(SvgData svgData)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            if (svgData.Metadata.TryGetValue("dbId", out var dbIdObj) && dbIdObj is int dbId)
            {
                // 更新现有记录
                command.CommandText = "UPDATE svg_assets SET svg_content = @content WHERE id = @id";
                command.Parameters.AddWithValue("@content", svgData.Content);
                command.Parameters.AddWithValue("@id", dbId);
            }
            else
            {
                // 插入新记录
                command.CommandText = "INSERT INTO svg_assets (name, svg_content) VALUES (@name, @content)";
                command.Parameters.AddWithValue("@name", svgData.Source);
                command.Parameters.AddWithValue("@content", svgData.Content);
            }

            await command.ExecuteNonQueryAsync();
        }

        private async Task InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
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
    }
}
