using System.Collections.Generic;
using System.Threading.Tasks;

namespace SvgColorNormalizer.Core.DataSources
{
    public abstract class ConfigurableDataSourceBase : IConfigurableDataSource
    {
        public string Name { get; set; }
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();
        public abstract string SourceType { get; }

        public abstract Task<IEnumerable<Models.SvgData>> LoadAsync();
        public abstract bool ValidateConfiguration();
        public abstract Task InitializeAsync();
    }
}
