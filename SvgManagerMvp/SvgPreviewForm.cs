using SvgManagerMvp.Models;

namespace SvgManagerMvp
{
    public partial class SvgPreviewForm : Form
    {
        public SvgPreviewForm(SvgData svgData)
        {
            InitializeComponent();
            LoadSvgPreview(svgData);
        }

        private void LoadSvgPreview(SvgData svgData)
        {
            if (svgData == null)
                return;

            this.Text = $"SVG预览 - {svgData.Name}";

            // 使用WebBrowser控件显示SVG
            string svgContent = svgData.Content;
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>SVG Preview</title>
    <style>
        body {{ margin: 0; padding: 20px; }}
        svg {{ max-width: 100%; max-height: 100%; }}
    </style>
</head>
<body>
    {svgContent}
</body>
</html>
";

            webBrowserPreview.DocumentText = html;
        }
    }
}