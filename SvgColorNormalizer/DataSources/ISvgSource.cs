using System.Collections.Generic;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public interface ISvgSource
    {
        string SourceType { get; }
        Task<IEnumerable<SvgData>> LoadAsync();
    }
}