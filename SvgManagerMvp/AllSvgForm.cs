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
            this.Resize += new System.EventHandler(this.AllSvgForm_Resize);
        }

        private void AllSvgForm_Resize(object sender, EventArgs e)
        {
            // 调整flowLayoutPanel的大小以适应窗口
            flowLayoutPanelSvg.Size = new System.Drawing.Size(
                this.ClientSize.Width - 24, // 24是左右边距
                this.ClientSize.Height - 80 // 80是上下边距和状态栏高度
            );
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
                // 使用SQLite数据库进行测试
                var options = new DbContextOptionsBuilder<SvgDbContext>()
                    .UseSqlite("Data Source=svg_test.db")
                    .Options;

                _context = new SvgDbContext(options);
                _context.Database.EnsureCreated();
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
        private Color _originalBackColor;

        public SvgPreviewControl(SvgData svgData)
        {
            _svgData = svgData;
            InitializeComponent();
            LoadSvg();
            _originalBackColor = this.BackColor;
            this.MouseEnter += new System.EventHandler(this.SvgPreviewControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.SvgPreviewControl_MouseLeave);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void InitializeComponent()
        {
            this.webBrowserSvg = new System.Windows.Forms.WebBrowser();
            this.labelName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowserSvg
            // 
            this.webBrowserSvg.Location = new System.Drawing.Point(12, 36);
            this.webBrowserSvg.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserSvg.Name = "webBrowserSvg";
            this.webBrowserSvg.Size = new System.Drawing.Size(180, 180);
            this.webBrowserSvg.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoEllipsis = true;
            this.labelName.Location = new System.Drawing.Point(12, 12);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(180, 18);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SvgPreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.webBrowserSvg);
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "SvgPreviewControl";
            this.Size = new System.Drawing.Size(204, 230);
            this.ResumeLayout(false);
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
        body {{ margin: 0; padding: 0; background-color: transparent; }}
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

        private void SvgPreviewControl_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void SvgPreviewControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = _originalBackColor;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }
    }
}