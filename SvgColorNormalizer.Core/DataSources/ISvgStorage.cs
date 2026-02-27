using System.Collections.Generic;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public interface ISvgStorage
    {
        string StorageType { get; }
        Task SaveAsync(IEnumerable<SvgData> svgDataList);
        Task SaveAsync(SvgData svgData);
    }
}
