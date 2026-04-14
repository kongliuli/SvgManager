using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web;
using System.Windows.Forms;
using SvgManagerMvp.Models;

namespace SvgManagerMvp
{
    [ComVisible(true)]
    public partial class SvgPreviewForm : Form
    {
        private int rotation = 0;
        private int zoom = 100;
        private string backgroundColor = "#ffffff";

        public SvgPreviewForm(SvgData svgData)
        {
            InitializeComponent();
            // 启用脚本和外部通知
            webBrowserPreview.ScriptErrorsSuppressed = true;
            webBrowserPreview.ObjectForScripting = this;
            InitializeEvents();
            LoadSvgPreview(svgData);
        }

        private void InitializeEvents()
        {
            btnReset.Click += BtnReset_Click;
            btnRotate.Click += BtnRotate_Click;
            trackBarZoom.ValueChanged += TrackBarZoom_ValueChanged;
            btnBackgroundColor.Click += BtnBackgroundColor_Click;
            webBrowserPreview.DocumentCompleted += WebBrowserPreview_DocumentCompleted;
        }

        private void WebBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // 注入鼠标滚轮事件处理的JavaScript
            if (webBrowserPreview.Document != null)
            {
                webBrowserPreview.Document.InvokeScript("execScript", new object[] {
                    @"document.addEventListener('wheel', function(e) {
                        if (e.ctrlKey) {
                            e.preventDefault();
                            var delta = e.deltaY > 0 ? -10 : 10;
                            window.external.Notify('zoom:' + delta);
                        }
                    });",
                    "JavaScript"
                });
            }
        }

        // 处理来自JavaScript的通知
        public void Notify(string message)
        {
            if (message.StartsWith("zoom:"))
            {
                int delta = int.Parse(message.Substring(5));
                zoom = Math.Max(10, Math.Min(500, zoom + delta));
                trackBarZoom.Value = zoom;
                labelZoom.Text = $"{zoom}%";
                UpdateZoom();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            rotation = 0;
            zoom = 100;
            backgroundColor = "#ffffff";
            trackBarZoom.Value = 100;
            labelZoom.Text = "100%";
            UpdatePreview();
        }

        private void BtnRotate_Click(object sender, EventArgs e)
        {
            rotation = (rotation + 90) % 360;
            UpdatePreview();
        }

        private void TrackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            zoom = trackBarZoom.Value;
            labelZoom.Text = $"{zoom}%";
            UpdateZoom();
        }

        private void BtnBackgroundColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                backgroundColor = ColorTranslator.ToHtml(colorDialog1.Color);
                UpdatePreview();
            }
        }

        private void UpdateZoom()
        {
            if (webBrowserPreview.Document != null)
            {
                webBrowserPreview.Document.InvokeScript("execScript", new object[] {
                    $"document.getElementById('svgContainer').style.transform = 'scale({zoom / 100}) rotate({rotation}deg)';",
                    "JavaScript"
                });
            }
        }

        private void UpdatePreview()
        {
            if (webBrowserPreview.Document != null)
            {
                webBrowserPreview.Document.InvokeScript("execScript", new object[] {
                    $"document.body.style.backgroundColor = '{backgroundColor}';",
                    "JavaScript"
                });
                webBrowserPreview.Document.InvokeScript("execScript", new object[] {
                    $"document.getElementById('svgContainer').style.transform = 'scale({zoom / 100}) rotate({rotation}deg)';",
                    "JavaScript"
                });
            }
        }

        private void LoadSvgPreview(SvgData svgData)
        {
            if (svgData == null)
                return;

            this.Text = $"SVG预览 - {svgData.Name}";

            try
            {
                // 使用WebBrowser控件显示SVG
                string svgContent = svgData.Content;
                // 对SVG内容进行HTML编码，确保特殊字符不会破坏HTML结构
                svgContent = System.Web.HttpUtility.HtmlEncode(svgContent);
                // 然后将编码后的内容中的<和>转换回原样，以确保SVG标签能够正确解析
                svgContent = svgContent.Replace("&lt;", "<").Replace("&gt;", ">");
                
                string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>SVG Preview</title>
    <style>
        body {{
            margin: 0;
            padding: 20px;
            background-color: {backgroundColor};
            overflow: auto;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100%;
        }}
        #svgContainer {{
            transition: transform 0.2s ease;
            transform-origin: center;
        }}
        svg {{
            max-width: 100%;
            max-height: 100%;
        }}
    </style>
</head>
<body>
    <div id='svgContainer'>
        {svgContent}
    </div>
    <script>
        // 确保SVG容器在加载时正确显示
        window.onload = function() {{
            var container = document.getElementById('svgContainer');
            container.style.transform = 'scale(1) rotate(0deg)';
        }};
    </script>
</body>
</html>
";

                webBrowserPreview.DocumentText = html;
            }
            catch (Exception ex)
            {
                // 显示错误信息
                string errorHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <title>预览错误</title>
    <style>
        body {{
            margin: 0;
            padding: 20px;
            background-color: {backgroundColor};
            overflow: auto;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100%;
        }}
        #errorContainer {{
            text-align: center;
            color: #ff0000;
        }}
    </style>
</head>
<body>
    <div id='errorContainer'>
        <h2>预览失败</h2>
        <p>无法显示SVG文件: {System.Web.HttpUtility.HtmlEncode(ex.Message)}</p>
    </div>
</body>
</html>
";
                webBrowserPreview.DocumentText = errorHtml;
            }
        }
    }
}