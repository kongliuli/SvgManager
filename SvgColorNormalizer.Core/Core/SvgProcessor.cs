using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core
{
    public class SvgProcessor
    {
        private readonly ColorNormalizer _colorNormalizer = new();
        private static readonly string[] ColorAttributes =
        {
            "fill", "stroke", "stop-color", "color",
            "flood-color", "lighting-color"
        };

        public SvgProcessResult ProcessSvg(string svgContent, ColorNormalizer.TargetColorFormat targetFormat = ColorNormalizer.TargetColorFormat.Rgba)
        {
            var result = new SvgProcessResult();

            try
            {
                var doc = XDocument.Parse(svgContent);
                var colorChanges = new List<ColorChange>();

                var elementsWithColor = doc.Descendants()
                    .Where(e => e.Attributes().Any(a => ColorAttributes.Contains(a.Name.LocalName, StringComparer.OrdinalIgnoreCase)));

                foreach (var element in elementsWithColor)
                {
                    foreach (var attr in element.Attributes().Where(a => ColorAttributes.Contains(a.Name.LocalName, StringComparer.OrdinalIgnoreCase)))
                    {
                        string originalColor = attr.Value;
                        string normalizedColor = _colorNormalizer.NormalizeColor(originalColor, targetFormat);

                        if (normalizedColor != originalColor)
                        {
                            attr.Value = normalizedColor;
                            colorChanges.Add(new ColorChange
                            {
                                Original = originalColor,
                                Normalized = normalizedColor,
                                Element = element.Name.LocalName,
                                Attribute = attr.Name.LocalName
                            });
                            result.ModifiedCount++;
                        }
                    }
                }

                result.NormalizedSvg = doc.ToString();
                result.ColorChanges = colorChanges;
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}