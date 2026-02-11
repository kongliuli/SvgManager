using System.Collections.Generic;

namespace SvgColorNormalizer.Models
{
    public class SvgData
    {
        public string Source { get; set; }
        public string Content { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}