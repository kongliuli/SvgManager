using System.Text.RegularExpressions;

namespace SvgManagerMvp.Services
{
    public class SvgAttributeService
    {
        public SvgAttributes ExtractAttributes(string svgContent)
        {
            if (string.IsNullOrWhiteSpace(svgContent))
                return new SvgAttributes();

            var attributes = new SvgAttributes();

            // 移除注释，避免匹配到注释中的内容
            string cleanedSvg = RemoveComments(svgContent);

            // 提取尺寸信息
            ExtractDimensions(cleanedSvg, attributes);

            // 提取元素数量
            attributes.ElementCount = CountElements(cleanedSvg);

            // 提取颜色信息
            ExtractColors(cleanedSvg, attributes);

            return attributes;
        }

        private string RemoveComments(string svgContent)
        {
            // 移除 XML 注释
            return Regex.Replace(svgContent, @"<!--[\s\S]*?-->", string.Empty);
        }

        private void ExtractDimensions(string svgContent, SvgAttributes attributes)
        {
            // 尝试从 svg 标签提取 width 和 height
            var svgMatch = Regex.Match(svgContent, @"<svg[^>]*?width=(""|')([^""']+)(""|')[^>]*?height=(""|')([^""']+)(""|')", RegexOptions.IgnoreCase);
            if (svgMatch.Success)
            {
                attributes.Width = svgMatch.Groups[2].Value;
                attributes.Height = svgMatch.Groups[5].Value;
            }

            // 如果没有找到 width 和 height，尝试从 viewBox 提取
            if (string.IsNullOrEmpty(attributes.Width) || string.IsNullOrEmpty(attributes.Height))
            {
                var viewBoxMatch = Regex.Match(svgContent, @"viewBox=(""|')([^""']+)(""|')", RegexOptions.IgnoreCase);
                if (viewBoxMatch.Success)
                {
                    string viewBox = viewBoxMatch.Groups[2].Value;
                    string[] parts = viewBox.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        attributes.ViewBox = viewBox;
                        attributes.Width = parts[2]; // viewBox 的第三个值是宽度
                        attributes.Height = parts[3]; // viewBox 的第四个值是高度
                    }
                }
            }
        }

        private int CountElements(string svgContent)
        {
            // 匹配所有 SVG 元素标签
            var matches = Regex.Matches(svgContent, @"<([a-z][a-z0-9]*)[^>]*>", RegexOptions.IgnoreCase);
            return matches.Count;
        }

        private void ExtractColors(string svgContent, SvgAttributes attributes)
        {
            // 提取所有颜色值
            var colorPatterns = new List<Regex>
            {
                new Regex(@"fill=(""|')([^""']+)(""|')", RegexOptions.IgnoreCase),
                new Regex(@"stroke=(""|')([^""']+)(""|')", RegexOptions.IgnoreCase),
                new Regex(@"stop-color=(""|')([^""']+)(""|')", RegexOptions.IgnoreCase)
            };

            foreach (var pattern in colorPatterns)
            {
                var matches = pattern.Matches(svgContent);
                foreach (Match match in matches)
                {
                    string color = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(color) && color != "none" && color != "transparent")
                    {
                        attributes.Colors.Add(color);
                    }
                }
            }

            // 去重
            attributes.Colors = attributes.Colors.Distinct().ToList();
        }
    }

    public class SvgAttributes
    {
        public string Width { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
        public string ViewBox { get; set; } = string.Empty;
        public int ElementCount { get; set; } = 0;
        public List<string> Colors { get; set; } = new List<string>();
    }
}