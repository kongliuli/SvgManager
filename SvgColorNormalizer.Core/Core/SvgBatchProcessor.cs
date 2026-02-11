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

            var tasks = svgDataList.Select(async svgData =>
            {
                await Task.Yield(); // 让出当前线程
                var processResult = _processor.ProcessSvg(svgData.Content, targetFormat);
                processResult.Source = svgData.Source;
                processResult.Metadata = svgData.Metadata;
                return processResult;
            });

            var processResults = await Task.WhenAll(tasks);

            foreach (var processResult in processResults)
            {
                result.IndividualResults.Add(processResult);

                if (processResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.ErrorMessage += $"\n{processResult.Source}: {processResult.ErrorMessage}";
                }
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }
    }
}