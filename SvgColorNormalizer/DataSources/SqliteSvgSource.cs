using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public class SqliteSvgSource : ISvgSource
    {
        private readonly string _dbPath;

        public SqliteSvgSource(string dbPath)
        {
            _dbPath = dbPath;
        }

        public string SourceType => "SQLite数据库";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();
            var connectionString = $"Data Source={_dbPath};Version=3;";

            await Task.Run(() =>
            {
                using var conn = new SQLiteConnection(connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, svg_content FROM svg_assets";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new SvgData
                    {
                        Source = $"SQLite:{reader.GetInt32(0)}",
                        Content = reader.GetString(1),
                        Metadata = new Dictionary<string, object> { ["dbId"] = reader.GetInt32(0) }
                    });
                }
            });

            return results;
        }
    }
}