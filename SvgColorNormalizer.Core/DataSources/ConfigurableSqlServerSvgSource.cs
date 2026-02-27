using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class ConfigurableSqlServerSvgSource : ConfigurableDataSourceBase
    {
        private string ConnectionString => GetConnectionString();

        public override string SourceType => "SQL Server数据库";

        public override async Task<IEnumerable<SvgData>> LoadAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            var results = new List<SvgData>();

            using var connection = new SqlConnection(ConnectionString);
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
            return Configuration.ContainsKey("Server") && 
                   Configuration.ContainsKey("Database") &&
                   Configuration.ContainsKey("User Id") &&
                   Configuration.ContainsKey("Password");
        }

        public override async Task InitializeAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[svg_assets]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[svg_assets](
                        [id] [int] IDENTITY(1,1) NOT NULL,
                        [name] [nvarchar](255) NOT NULL,
                        [svg_content] [nvarchar](max) NOT NULL,
                        [created_at] [datetime] DEFAULT GETDATE(),
                        CONSTRAINT [PK_svg_assets] PRIMARY KEY CLUSTERED ([id] ASC)
                    );
                END
            ";

            await command.ExecuteNonQueryAsync();
        }

        private string GetConnectionString()
        {
            return $"Server={Configuration["Server"]};Database={Configuration["Database"]};User Id={Configuration["User Id"]};Password={Configuration["Password"]};";
        }
    }
}
