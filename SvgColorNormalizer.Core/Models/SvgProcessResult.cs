using System.Collections.Generic;

namespace SvgColorNormalizer.Core.Models
{
    public class SvgProcessResult
    {
        public bool Success { get; set; }
        public string? NormalizedSvg { get; set; }
        public List<ColorChange> ColorChanges { get; set; } = new();
        public int ModifiedCount { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class BatchProcessResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public System.TimeSpan Duration { get; set; }
        public List<SvgProcessResult>? IndividualResults { get; set; }
        public string? ErrorMessage { get; set; }
    }
}