using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class SqliteSvgSource : ISvgSource
    {
        private readonly string _connectionString;

        public SqliteSvgSource(string databasePath)
        {
            // 确保数据库文件路径正确
            if (!Path.IsPathRooted(databasePath))
            {
                databasePath = Path.Combine(Directory.GetCurrentDirectory(), databasePath);
            }

            // 确保目录存在
            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connectionString = $"Data Source={databasePath}";
        }

        public string SourceType => "SQLite数据库";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();

            using var connection = new SqliteConnection(_connectionString);
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
                        ["connectionString"] = _connectionString
                    }
                });
            }

            return results;
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

        public async Task InitializeDatabase()
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

        public async Task InsertSampleData()
        {
            var sampleSvgs = new List<(string name, string content)>
            {
                ("红色矩形", "<svg width=\"100\" height=\"100\"><rect x=\"10\" y=\"10\" width=\"80\" height=\"80\" fill=\"rgba(255,0,0,0.8)\"/></svg>"),
                ("绿色圆形", "<svg width=\"100\" height=\"100\"><circle cx=\"50\" cy=\"50\" r=\"40\" fill=\"#00FF00\"/></svg>"),
                ("蓝色三角形", "<svg width=\"100\" height=\"100\"><polygon points=\"50,10 90,90 10,90\" fill=\"rgba(0,0,255,0.6)\"/></svg>"),
                ("黄色椭圆", "<svg width=\"100\" height=\"100\"><ellipse cx=\"50\" cy=\"50\" rx=\"40\" ry=\"25\" fill=\"#FFFF00\"/></svg>"),
                ("紫色文本", "<svg width=\"100\" height=\"100\"><text x=\"50\" y=\"60\" font-size=\"20\" text-anchor=\"middle\" fill=\"#800080\">SVG</text></svg>"),
                ("橙色路径", "<svg width=\"100\" height=\"100\"><path d=\"M10,10 L90,10 L90,90 L10,90 Z\" fill=\"rgba(255,165,0,0.7)\"/></svg>"),
                ("青色线条", "<svg width=\"100\" height=\"100\"><line x1=\"10\" y1=\"10\" x2=\"90\" y2=\"90\" stroke=\"#00FFFF\" stroke-width=\"3\"/></svg>"),
                ("灰色圆角矩形", "<svg width=\"100\" height=\"100\"><rect x=\"10\" y=\"10\" width=\"80\" height=\"80\" rx=\"10\" ry=\"10\" fill=\"#808080\"/></svg>"),
                ("粉色心形", "<svg width=\"100\" height=\"100\"><path d=\"M50,10 C20,10 10,40 10,60 C10,80 30,90 50,90 C70,90 90,80 90,60 C90,40 80,10 50,10 Z\" fill=\"rgba(255,105,180,0.9)\"/></svg>"),
                ("棕色星形", "<svg width=\"100\" height=\"100\"><polygon points=\"50,10 61,39 92,39 68,61 79,90 50,70 21,90 32,61 8,39 39,39\" fill=\"#A0522D\"/></svg>"),
                ("白色雪花", "<svg width=\"100\" height=\"100\"><path d=\"M50,10 L50,90 M10,50 L90,50 M29,29 L71,71 M29,71 L71,29\" stroke=\"#FFFFFF\" stroke-width=\"3\" fill=\"none\"/></svg>"),
                ("黑色正方形", "<svg width=\"100\" height=\"100\"><rect x=\"25\" y=\"25\" width=\"50\" height=\"50\" fill=\"#000000\"/></svg>"),
                ("绿色叶子", "<svg width=\"100\" height=\"100\"><path d=\"M50,10 C30,30 30,60 50,90 C70,60 70,30 50,10 Z M50,10 C60,20 60,40 50,60 C40,40 40,20 50,10 Z\" fill=\"#228B22\"/></svg>"),
                ("蓝色波浪", "<svg width=\"100\" height=\"100\"><path d=\"M0,70 Q25,50 50,70 T100,70 L100,100 L0,100 Z\" fill=\"rgba(135,206,235,0.8)\"/></svg>"),
                ("紫色六边形", "<svg width=\"100\" height=\"100\"><polygon points=\"50,10 85,35 85,65 50,90 15,65 15,35\" fill=\"#9370DB\"/></svg>")
            };

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            foreach (var (name, content) in sampleSvgs)
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO svg_assets (name, svg_content) VALUES (@name, @content)";
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@content", content);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}