using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SvgColorNormalizer.Core
{
    public class ColorNormalizer
    {
        public enum TargetColorFormat
        {
            Rgba,
            Hex8
        }

        private static readonly Dictionary<ColorFormat, Regex> ColorPatterns = new()
        {
            [ColorFormat.Rgb] = new Regex(@"rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)", RegexOptions.IgnoreCase),
            [ColorFormat.Rgba] = new Regex(@"rgba\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*([01]?\.?\d*)\s*\)", RegexOptions.IgnoreCase),
            [ColorFormat.Hex6] = new Regex(@"#([0-9a-fA-F]{6})(?!\w)", RegexOptions.IgnoreCase),
            [ColorFormat.Hex8] = new Regex(@"#([0-9a-fA-F]{8})(?!\w)", RegexOptions.IgnoreCase),
            [ColorFormat.Named] = new Regex(@"^(aqua|black|blue|fuchsia|gray|green|lime|maroon|navy|olive|orange|purple|red|silver|teal|white|yellow)$", RegexOptions.IgnoreCase)
        };

        private static readonly Dictionary<string, (int r, int g, int b)> NamedColors = new()
        {
            ["red"] = (255, 0, 0),
            ["green"] = (0, 128, 0),
            ["blue"] = (0, 0, 255),
            ["white"] = (255, 255, 255),
            ["black"] = (0, 0, 0),
            ["gray"] = (128, 128, 128),
            ["grey"] = (128, 128, 128),
            ["silver"] = (192, 192, 192),
            ["maroon"] = (128, 0, 0),
            ["olive"] = (128, 128, 0),
            ["lime"] = (0, 255, 0),
            ["aqua"] = (0, 255, 255),
            ["teal"] = (0, 128, 128),
            ["navy"] = (0, 0, 128),
            ["fuchsia"] = (255, 0, 255),
            ["purple"] = (128, 0, 128),
            ["orange"] = (255, 165, 0)
        };

        public string NormalizeColor(string originalColor, TargetColorFormat targetFormat = TargetColorFormat.Rgba)
        {
            if (string.IsNullOrWhiteSpace(originalColor))
                return originalColor;

            originalColor = originalColor.Trim();

            // 解析原始颜色
            var (r, g, b, a) = ParseColor(originalColor);
            if (r == -1)
                return originalColor;

            // 根据目标格式转换
            return ConvertToTargetFormat(r, g, b, a, targetFormat);
        }

        private (int r, int g, int b, double a) ParseColor(string color)
        {
            // RGB格式
            var rgbMatch = ColorPatterns[ColorFormat.Rgb].Match(color);
            if (rgbMatch.Success)
            {
                int r = int.Parse(rgbMatch.Groups[1].Value);
                int g = int.Parse(rgbMatch.Groups[2].Value);
                int b = int.Parse(rgbMatch.Groups[3].Value);
                return (r, g, b, 1.0);
            }

            // RGBA格式
            var rgbaMatch = ColorPatterns[ColorFormat.Rgba].Match(color);
            if (rgbaMatch.Success)
            {
                int r = int.Parse(rgbaMatch.Groups[1].Value);
                int g = int.Parse(rgbaMatch.Groups[2].Value);
                int b = int.Parse(rgbaMatch.Groups[3].Value);
                double a = double.Parse(rgbaMatch.Groups[4].Value);
                return (r, g, b, a);
            }

            // 6位16进制
            var hex6Match = ColorPatterns[ColorFormat.Hex6].Match(color);
            if (hex6Match.Success)
            {
                int r = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(4, 2), 16);
                return (r, g, b, 1.0);
            }

            // 8位16进制
            var hex8Match = ColorPatterns[ColorFormat.Hex8].Match(color);
            if (hex8Match.Success)
            {
                int r = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(4, 2), 16);
                int alpha = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(6, 2), 16);
                double a = Math.Round(alpha / 255.0, 2);
                return (r, g, b, a);
            }

            // 命名颜色
            var namedMatch = ColorPatterns[ColorFormat.Named].Match(color);
            if (namedMatch.Success)
            {
                string colorName = namedMatch.Groups[1].Value.ToLower();
                if (NamedColors.TryGetValue(colorName, out var rgb))
                {
                    return (rgb.r, rgb.g, rgb.b, 1.0);
                }
            }

            return (-1, -1, -1, 0.0);
        }

        private string ConvertToTargetFormat(int r, int g, int b, double a, TargetColorFormat targetFormat)
        {
            switch (targetFormat)
            {
                case TargetColorFormat.Rgba:
                    // 保持原始透明度
                    return $"rgba({r}, {g}, {b}, {a})";
                
                case TargetColorFormat.Hex8:
                    // 保持原始透明度
                    int alpha = (int)Math.Round(a * 255);
                    return $"#{r:X2}{g:X2}{b:X2}{alpha:X2}";
                
                default:
                    return $"rgba({r}, {g}, {b}, {a})";
            }
        }
    }

    public enum ColorFormat
    {
        Rgb,
        Rgba,
        Hex6,
        Hex8,
        Named
    }
}