using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public class FolderSvgSource : ISvgSource
    {
        private readonly string _folderPath;

        public FolderSvgSource(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string SourceType => "本地文件夹";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();

            await Task.Run(() =>
            {
                var svgFiles = Directory.GetFiles(_folderPath, "*.svg", SearchOption.AllDirectories);

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
    }
}