using System.Collections.Generic;
using Npgsql;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class ConfigurablePostgreSqlSvgSource : ConfigurableDataSourceBase
    {
        private string ConnectionString => GetConnectionString();

        public override string SourceType => "PostgreSQL数据库";

        public override async Task<IEnumerable<SvgData>> LoadAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            var results = new List<SvgData>();

            using var connection = new NpgsqlConnection(ConnectionString);
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
            return Configuration.ContainsKey("Host") && 
                   Configuration.ContainsKey("Port") &&
                   Configuration.ContainsKey("Database") &&
                   Configuration.ContainsKey("Username") &&
                   Configuration.ContainsKey("Password");
        }

        public override async Task InitializeAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS svg_assets (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    svg_content TEXT NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ";

            await command.ExecuteNonQueryAsync();
        }

        private string GetConnectionString()
        {
            return $"Host={Configuration["Host"]};Port={Configuration["Port"]};Database={Configuration["Database"]};Username={Configuration["Username"]};Password={Configuration["Password"]};";
        }
    }
}
