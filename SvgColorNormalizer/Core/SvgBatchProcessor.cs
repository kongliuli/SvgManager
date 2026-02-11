using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.Core
{
    public class SvgBatchProcessor
    {
        private readonly SvgProcessor _processor = new();

        public async Task<BatchProcessResult> ProcessBatchAsync(IEnumerable<SvgData> svgDataList)
        {
            var result = new BatchProcessResult
            {
                TotalCount = svgDataList.Count(),
                StartTime = DateTime.Now,
                IndividualResults = new List<SvgProcessResult>()
            };

            foreach (var svgData in svgDataList)
            {
                var processResult = _processor.ProcessSvg(svgData.Content);
                processResult.Source = svgData.Source;
                processResult.Metadata = svgData.Metadata;

                result.IndividualResults.Add(processResult);

                if (processResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.ErrorMessage += $"\n{svgData.Source}: {processResult.ErrorMessage}";
                }
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }
    }

    public class BatchProcessResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public List<SvgProcessResult> IndividualResults { get; set; }
        public string ErrorMessage { get; set; }
    }
}