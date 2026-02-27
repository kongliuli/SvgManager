using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class FolderSvgStorage : ISvgStorage
    {
        private readonly string _folderPath;

        public FolderSvgStorage(string folderPath)
        {
            _folderPath = folderPath;
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        public string StorageType => "本地文件夹";

        public async Task SaveAsync(IEnumerable<SvgData> svgDataList)
        {
            foreach (var svgData in svgDataList)
            {
                await SaveAsync(svgData);
            }
        }

        public async Task SaveAsync(SvgData svgData)
        {
            var fileName = Path.GetFileName(svgData.Source) ?? $"{Path.GetRandomFileName()}.svg";
            var filePath = Path.Combine(_folderPath, fileName);

            await File.WriteAllTextAsync(filePath, svgData.Content);
        }
    }
}
