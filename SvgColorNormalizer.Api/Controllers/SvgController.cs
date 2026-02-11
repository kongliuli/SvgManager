using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using SvgColorNormalizer.Core;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SvgController : ControllerBase
    {
        private readonly SvgProcessor _svgProcessor = new();
        private readonly SvgBatchProcessor _batchProcessor = new();

        [HttpPost("normalize")]
        public IActionResult NormalizeSvg([FromBody] SvgNormalizeRequest request)
        {
            try
            {
                var targetFormat = request.TargetFormat == "hex8" 
                    ? ColorNormalizer.TargetColorFormat.Hex8 
                    : ColorNormalizer.TargetColorFormat.Rgba;

                var result = _svgProcessor.ProcessSvg(request.SvgContent, targetFormat);

                if (result.Success)
                {
                    return Ok(new SvgNormalizeResponse
                    {
                        Success = true,
                        NormalizedSvg = result.NormalizedSvg,
                        ColorChanges = result.ColorChanges,
                        ModifiedCount = result.ModifiedCount
                    });
                }
                else
                {
                    return BadRequest(new SvgNormalizeResponse
                    {
                        Success = false,
                        ErrorMessage = result.ErrorMessage
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SvgNormalizeResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("normalize-batch")]
        public async Task<IActionResult> NormalizeSvgBatch([FromBody] SvgBatchNormalizeRequest request)
        {
            try
            {
                var targetFormat = request.TargetFormat == "hex8" 
                    ? ColorNormalizer.TargetColorFormat.Hex8 
                    : ColorNormalizer.TargetColorFormat.Rgba;

                var svgDataList = request.SvgItems.Select(item => new SvgData
                {
                    Source = item.Id,
                    Content = item.Content,
                    Metadata = item.Metadata
                }).ToList();

                var result = await _batchProcessor.ProcessBatchAsync(svgDataList, targetFormat);

                return Ok(new SvgBatchNormalizeResponse
                {
                    Success = true,
                    TotalCount = result.TotalCount,
                    SuccessCount = result.SuccessCount,
                    FailedCount = result.FailedCount,
                    Duration = result.Duration.TotalSeconds,
                    Results = result.IndividualResults.Select(r => new SvgBatchResultItem
                    {
                        Id = r.Source,
                        Success = r.Success,
                        NormalizedSvg = r.NormalizedSvg,
                        ColorChanges = r.ColorChanges,
                        ModifiedCount = r.ModifiedCount,
                        ErrorMessage = r.ErrorMessage
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new SvgBatchNormalizeResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }

    public class SvgNormalizeRequest
    {
        public string SvgContent { get; set; }
        public string TargetFormat { get; set; } = "rgba";
    }

    public class SvgNormalizeResponse
    {
        public bool Success { get; set; }
        public string NormalizedSvg { get; set; }
        public List<ColorChange> ColorChanges { get; set; }
        public int ModifiedCount { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SvgBatchNormalizeRequest
    {
        public List<SvgBatchItem> SvgItems { get; set; }
        public string TargetFormat { get; set; } = "rgba";
    }

    public class SvgBatchItem
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class SvgBatchNormalizeResponse
    {
        public bool Success { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public double Duration { get; set; }
        public List<SvgBatchResultItem> Results { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SvgBatchResultItem
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public string NormalizedSvg { get; set; }
        public List<ColorChange> ColorChanges { get; set; }
        public int ModifiedCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}