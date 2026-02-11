namespace SvgColorNormalizer.Core.Models
{
    public class ColorChange
    {
        public string Original { get; set; }
        public string Normalized { get; set; }
        public string Element { get; set; }
        public string Attribute { get; set; }
    }
}