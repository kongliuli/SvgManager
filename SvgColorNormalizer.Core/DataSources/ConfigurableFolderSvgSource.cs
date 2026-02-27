using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class ConfigurableFolderSvgSource : ConfigurableDataSourceBase
    {
        public override string SourceType => "本地文件夹";

        public override async Task<IEnumerable<SvgData>> LoadAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            var folderPath = Configuration["FolderPath"];
            var results = new List<SvgData>();

            await Task.Run(() =>
            {
                var svgFiles = Directory.GetFiles(folderPath, "*.svg", SearchOption.AllDirectories);

                foreach (var file in svgFiles)
                {
                    results.Add(new SvgData
                    {
                        Source = file,
                        Content = File.ReadAllText(file),
                        Metadata = new Dictionary<string, object> { ["filePath"] = file }
                    });
                }
            });

            return results;
        }

        public override bool ValidateConfiguration()
        {
            return Configuration.ContainsKey("FolderPath") && Directory.Exists(Configuration["FolderPath"]);
        }

        public override Task InitializeAsync()
        {
            // 本地文件夹数据源不需要初始化
            return Task.CompletedTask;
        }
    }
}
