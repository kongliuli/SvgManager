using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SvgColorNormalizer.Core
{
    public class ColorNormalizer
    {
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

        public string NormalizeColor(string originalColor)
        {
            if (string.IsNullOrWhiteSpace(originalColor))
                return originalColor;

            originalColor = originalColor.Trim();

            // 已经是RGB格式
            if (ColorPatterns[ColorFormat.Rgb].IsMatch(originalColor) ||
                ColorPatterns[ColorFormat.Rgba].IsMatch(originalColor))
                return originalColor;

            // 6位16进制 → rgb
            var hex6Match = ColorPatterns[ColorFormat.Hex6].Match(originalColor);
            if (hex6Match.Success)
            {
                int r = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex6Match.Groups[1].Value.Substring(4, 2), 16);
                return $"rgb({r}, {g}, {b})";
            }

            // 8位16进制 → rgba
            var hex8Match = ColorPatterns[ColorFormat.Hex8].Match(originalColor);
            if (hex8Match.Success)
            {
                int r = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(4, 2), 16);
                int a = Convert.ToInt32(hex8Match.Groups[1].Value.Substring(6, 2), 16);
                double alpha = Math.Round(a / 255.0, 2);
                return alpha == 1 ? $"rgb({r}, {g}, {b})" : $"rgba({r}, {g}, {b}, {alpha})";
            }

            // 命名颜色 → rgb
            var namedMatch = ColorPatterns[ColorFormat.Named].Match(originalColor);
            if (namedMatch.Success)
            {
                string colorName = namedMatch.Groups[1].Value.ToLower();
                if (NamedColors.TryGetValue(colorName, out var rgb))
                {
                    return $"rgb({rgb.r}, {rgb.g}, {rgb.b})";
                }
            }

            return originalColor;
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