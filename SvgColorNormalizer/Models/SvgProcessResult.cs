using System.Collections.Generic;

namespace SvgColorNormalizer.Models
{
    public class SvgProcessResult
    {
        public bool Success { get; set; }
        public string NormalizedSvg { get; set; }
        public List<ColorChange> ColorChanges { get; set; } = new();
        public int ModifiedCount { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}