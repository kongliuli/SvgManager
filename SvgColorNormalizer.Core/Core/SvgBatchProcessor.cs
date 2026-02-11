using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core
{
    public class SvgBatchProcessor
    {
        private readonly SvgProcessor _processor = new();

        public async Task<BatchProcessResult> ProcessBatchAsync(IEnumerable<SvgData> svgDataList, ColorNormalizer.TargetColorFormat targetFormat = ColorNormalizer.TargetColorFormat.Rgba)
        {
            var result = new BatchProcessResult
            {
                TotalCount = svgDataList.Count(),
                StartTime = DateTime.Now,
                IndividualResults = new List<SvgProcessResult>()
            };

            foreach (var svgData in svgDataList)
            {
                var processResult = _processor.ProcessSvg(svgData.Content, targetFormat);
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
}