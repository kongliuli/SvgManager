using Microsoft.EntityFrameworkCore;
using SvgManagerMvp.Data;
using SvgManagerMvp.Models;

namespace SvgManagerMvp
{
    public partial class AllSvgForm : Form
    {
        private SvgDbContext _context;
        private int _currentPage = 1;
        private const int _pageSize = 20;
        private ToolStripStatusLabel _statusLabel;

        public AllSvgForm()
        {
            InitializeComponent();
            InitializeStatusStrip();
            InitializeDatabase();
            LoadSvgData();
        }

        private void InitializeStatusStrip()
        {
            var statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel();
            _statusLabel.Text = "就绪";
            statusStrip.Items.Add(_statusLabel);
            this.Controls.Add(statusStrip);
            statusStrip.Dock = DockStyle.Bottom;
        }

        private void InitializeDatabase()
        {
            try
            {
                var connectionString = ConfigManager.GetConnectionString();
                var options = new DbContextOptionsBuilder<SvgDbContext>()
                    .UseSqlServer(connectionString) // 注意：实际部署时需要替换为UseDm
                    .Options;

                _context = new SvgDbContext(options);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库初始化失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadSvgData(int page = 1)
        {
            try
            {
                _currentPage = page;
                var totalCount = await _context.IconResources.CountAsync();
                var svgList = await _context.IconResources
                    .OrderBy(s => s.Id)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

                flowLayoutPanelSvg.Controls.Clear();
                foreach (var svgData in svgList)
                {
                    var svgPreviewControl = new SvgPreviewControl(svgData);
                    flowLayoutPanelSvg.Controls.Add(svgPreviewControl);
                }

                UpdatePagination(totalCount);
                UpdateStatus("数据加载完成");
            }
            catch (Exception ex)
            {
                UpdateStatus($"加载SVG数据失败: {ex.Message}");
                MessageBox.Show($"加载SVG数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePagination(int totalCount)
        {
            int totalPages = (int)Math.Ceiling((double)totalCount / _pageSize);
            labelPageInfo.Text = $"第 {_currentPage} 页，共 {totalPages} 页";
            buttonPrev.Enabled = _currentPage > 1;
            buttonNext.Enabled = _currentPage < totalPages;
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.Text = message;
            Application.DoEvents();
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                LoadSvgData(_currentPage - 1);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            LoadSvgData(_currentPage + 1);
        }
    }

    public class SvgPreviewControl : UserControl
    {
        private WebBrowser webBrowserSvg;
        private Label labelName;
        private SvgData _svgData;

        public SvgPreviewControl(SvgData svgData)
        {
            _svgData = svgData;
            InitializeComponent();
            LoadSvg();
        }

        private void InitializeComponent()
        {
            this.webBrowserSvg = new System.Windows.Forms.WebBrowser();
            this.labelName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowserSvg
            // 
            this.webBrowserSvg.Location = new System.Drawing.Point(10, 30);
            this.webBrowserSvg.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserSvg.Name = "webBrowserSvg";
            this.webBrowserSvg.Size = new System.Drawing.Size(150, 150);
            this.webBrowserSvg.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(10, 10);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(39, 15);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // SvgPreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.webBrowserSvg);
            this.Name = "SvgPreviewControl";
            this.Size = new System.Drawing.Size(170, 190);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadSvg()
        {
            if (_svgData == null)
                return;

            labelName.Text = _svgData.Name;

            string svgContent = _svgData.Content;
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>SVG Preview</title>
    <style>
        body {{ margin: 0; padding: 0; }}
        svg {{ width: 100%; height: 100%; }}
    </style>
</head>
<body>
    {svgContent}
</body>
</html>
";

            webBrowserSvg.DocumentText = html;
        }
    }
}