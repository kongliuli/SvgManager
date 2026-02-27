using System; using System.Collections.Generic; using System.IO; using System.Text.Json; using System.Threading.Tasks;

namespace SvgColorNormalizer.Core.DataSources
{
    public class DataSourceConfigManager
    {
        private readonly string _configFilePath;

        public DataSourceConfig Config { get; private set; }

        public DataSourceConfigManager(string configFilePath = "data_source_config.json")
        {
            _configFilePath = Path.IsPathRooted(configFilePath) 
                ? configFilePath 
                : Path.Combine(Directory.GetCurrentDirectory(), configFilePath);
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                var jsonContent = File.ReadAllText(_configFilePath);
                Config = JsonSerializer.Deserialize<DataSourceConfig>(jsonContent);
            }
            else
            {
                Config = new DataSourceConfig
                {
                    DataSources = new List<DataSourceConfiguration>(),
                    StorageConfigurations = new List<StorageConfiguration>()
                };
                SaveConfig();
            }
        }

        public void SaveConfig()
        {
            var jsonContent = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, jsonContent);
        }

        public IConfigurableDataSource CreateDataSource(string dataSourceName)
        {
            var config = Config.DataSources.Find(ds => ds.Name == dataSourceName);
            if (config == null)
                throw new Exception($"未找到名称为 {dataSourceName} 的数据源配置");

            IConfigurableDataSource dataSource = config.Type switch
            {
                "本地文件夹" => new ConfigurableFolderSvgSource(),
                "SQLite数据库" => new ConfigurableSqliteSvgSource(),
                "SQL Server数据库" => new ConfigurableSqlServerSvgSource(),
                "PostgreSQL数据库" => new ConfigurablePostgreSqlSvgSource(),
                // "达梦数据库" => new ConfigurableDmSvgSource(), // 达梦数据库驱动需要从官方获取并手动安装
                "API接口" => new ConfigurableApiSvgSource(),
                _ => throw new Exception($"不支持的数据源类型: {config.Type}")
            };

            dataSource.Name = config.Name;
            dataSource.Configuration = config.Configuration;
            return dataSource;
        }

        public ISvgStorage CreateStorage(string storageName)
        {
            var config = Config.StorageConfigurations.Find(s => s.Name == storageName);
            if (config == null)
                throw new Exception($"未找到名称为 {storageName} 的存储配置");

            ISvgStorage storage = config.Type switch
            {
                "本地文件夹" => new FolderSvgStorage(config.Configuration["FolderPath"]),
                "SQLite数据库" => new SqliteSvgStorage(config.Configuration["DatabasePath"]),
                _ => throw new Exception($"不支持的存储类型: {config.Type}")
            };

            return storage;
        }

        public async Task<IEnumerable<Models.SvgData>> LoadDataFromSource(string dataSourceName)
        {
            var dataSource = CreateDataSource(dataSourceName);
            await dataSource.InitializeAsync();
            return await dataSource.LoadAsync();
        }

        public async Task SaveDataToStorage(string storageName, IEnumerable<Models.SvgData> svgDataList)
        {
            var storage = CreateStorage(storageName);
            await storage.SaveAsync(svgDataList);
        }
    }

    public class DataSourceConfig
    {
        public List<DataSourceConfiguration> DataSources { get; set; }
        public List<StorageConfiguration> StorageConfigurations { get; set; }
    }

    public class DataSourceConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Configuration { get; set; }
    }

    public class StorageConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Configuration { get; set; }
    }
}
