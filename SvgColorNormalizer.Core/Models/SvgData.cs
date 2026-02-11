using System.Collections.Generic;

namespace SvgColorNormalizer.Core.Models
{
    public class SvgData
    {
        public string? Source { get; set; }
        public string? Content { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}