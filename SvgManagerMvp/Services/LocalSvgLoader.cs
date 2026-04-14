using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SvgManagerMvp.Models;

namespace SvgManagerMvp.Services
{
    public class LocalSvgLoader
    {
        public async Task<IEnumerable<SvgData>> LoadFromFolderAsync(string folderPath)
        {
            var results = new List<SvgData>();

            await Task.Run(() =>
            {
                var svgFiles = Directory.GetFiles(folderPath, "*.svg", SearchOption.AllDirectories);

                foreach (var file in svgFiles)
                {
                    results.Add(new SvgData
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Content = File.ReadAllText(file),
                        Category = Path.GetDirectoryName(file)?.Replace(folderPath, "").Trim('\\'),
                        CreatedAt = File.GetCreationTime(file),
                        UpdatedAt = File.GetLastWriteTime(file)
                    });
                }
            });

            return results;
        }
    }
}