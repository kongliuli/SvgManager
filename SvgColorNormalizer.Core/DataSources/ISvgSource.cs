using System.Collections.Generic;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public interface ISvgSource
    {
        string SourceType { get; }
        Task<IEnumerable<SvgData>> LoadAsync();
    }
}