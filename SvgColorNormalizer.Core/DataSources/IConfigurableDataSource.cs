using System.Collections.Generic;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public interface IConfigurableDataSource : ISvgSource
    {
        string Name { get; set; }
        Dictionary<string, string> Configuration { get; set; }
        bool ValidateConfiguration();
        Task InitializeAsync();
    }
}
