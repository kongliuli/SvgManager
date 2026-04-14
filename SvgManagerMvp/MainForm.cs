using Microsoft.EntityFrameworkCore;
using SvgManagerMvp.Data;
using SvgManagerMvp.Models;
using SvgManagerMvp.Services;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SvgManagerMvp
{
    public partial class MainForm : Form
    {
        private SvgDbContext _context;
        private SvgColorConverter _colorConverter;
        private LocalSvgLoader _localSvgLoader;
        private List<SvgData> _localSvgData;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private ToolStripStatusLabel _statusLabel;

        public MainForm()
        {
            InitializeComponent();
            InitializeStatusStrip();
            _colorConverter = new SvgColorConverter();
            _localSvgLoader = new LocalSvgLoader();
            InitializeDatabase();
            InitializePageSizeComboBox();
            InitializePreview();
            LoadSvgData();
            InitializeModernTheme();
        }

        private void InitializePreview()
        {
            webBrowserPreview.ScriptErrorsSuppressed = true;
            // 初始显示空白预览
            string initialHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <title>SVG预览</title>
    <style>
        body {{
            margin: 0;
            padding: 20px;
            background-color: #f0f2f5;
            overflow: auto;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100%;
        }}
        #placeholder {{
            text-align: center;
            color: #666;
            font-size: 16px;
        }}
    </style>
</head>
<body>
    <div id='placeholder'>
        请选择一个SVG文件查看预览
    </div>
</body>
</html>
";
            webBrowserPreview.DocumentText = initialHtml;
        }

        private void InitializeModernTheme()
        {
            // 设置主窗体背景色
            this.BackColor = Color.FromArgb(240, 242, 245);
            
            // 设置表格样式
            dataGridViewSvg.BackgroundColor = Color.White;
            dataGridViewSvg.GridColor = Color.LightGray;
            dataGridViewSvg.BorderStyle = BorderStyle.None;
            dataGridViewSvg.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewSvg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSvg.MultiSelect = true;
            dataGridViewSvg.EnableHeadersVisualStyles = false;
            
            // 设置表头样式
            dataGridViewSvg.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 102, 204);
            dataGridViewSvg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewSvg.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
            dataGridViewSvg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewSvg.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            
            // 设置行样式
            dataGridViewSvg.RowsDefaultCellStyle.BackColor = Color.White;
            dataGridViewSvg.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dataGridViewSvg.RowsDefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
            dataGridViewSvg.RowsDefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            dataGridViewSvg.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            
            // 设置选中行样式
            dataGridViewSvg.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            dataGridViewSvg.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);
            
            // 添加表格事件处理程序
            dataGridViewSvg.CellFormatting += dataGridViewSvg_CellFormatting;
            dataGridViewSvg.RowPrePaint += dataGridViewSvg_RowPrePaint;
            
            // 设置按钮样式
            SetButtonStyle(buttonPrev);
            SetButtonStyle(buttonNext);
            SetButtonStyle(buttonConvertColors);
            SetButtonStyle(buttonConfig);
            SetButtonStyle(buttonViewAll);
            SetButtonStyle(buttonViewAttributes);
            SetButtonStyle(buttonLoadLocal);
            
            // 设置标签样式
            labelPageInfo.ForeColor = Color.FromArgb(60, 60, 60);
            labelPageInfo.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            labelPageSize.ForeColor = Color.FromArgb(60, 60, 60);
            labelPageSize.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            
            // 设置复选框样式
            checkBoxSelectAll.ForeColor = Color.FromArgb(60, 60, 60);
            checkBoxSelectAll.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            
            // 设置下拉框样式
            comboBoxPageSize.BackColor = Color.White;
            comboBoxPageSize.ForeColor = Color.FromArgb(30, 30, 30);
            comboBoxPageSize.FlatStyle = FlatStyle.Flat;
            comboBoxPageSize.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
        }

        private void SetButtonStyle(Button button)
        {
            button.BackColor = Color.FromArgb(51, 102, 204);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            button.Padding = new Padding(5, 2, 5, 2);
            button.MouseEnter += (sender, e) => {
                button.BackColor = Color.FromArgb(61, 112, 214);
                button.Cursor = Cursors.Hand;
            };
            button.MouseLeave += (sender, e) => {
                button.BackColor = Color.FromArgb(51, 102, 204);
                button.Cursor = Cursors.Default;
            };
        }

        private void dataGridViewSvg_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                e.CellStyle.BackColor = Color.White;
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(245, 247, 250);
            }
        }

        private void dataGridViewSvg_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void InitializePageSizeComboBox()
        {
            comboBoxPageSize.SelectedIndex = 0; // 默认选择10
        }

        private void InitializeStatusStrip()
        {
            var statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.White;
            statusStrip.ForeColor = Color.FromArgb(60, 60, 60);
            statusStrip.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            statusStrip.Padding = new Padding(10, 2, 10, 2);
            
            _statusLabel = new ToolStripStatusLabel();
            _statusLabel.Text = "就绪";
            _statusLabel.Spring = true;
            _statusLabel.Alignment = ToolStripItemAlignment.Left;
            
            var versionLabel = new ToolStripStatusLabel();
            versionLabel.Text = "SVG管理器 v1.0";
            versionLabel.Alignment = ToolStripItemAlignment.Right;
            
            statusStrip.Items.Add(_statusLabel);
            statusStrip.Items.Add(versionLabel);
            
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

                dataGridViewSvg.DataSource = svgList;
                UpdatePagination(totalCount);
                UpdateStatus("数据加载完成");
                // 重置全选复选框状态
                checkBoxSelectAll.Checked = false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"加载SVG数据失败: {ex.Message}");
                MessageBox.Show($"加载SVG数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            // 全选/取消全选所有行
            foreach (DataGridViewRow row in dataGridViewSvg.Rows)
            {
                row.Selected = checkBoxSelectAll.Checked;
            }
        }

        private void comboBoxPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 当分页大小改变时，重新加载数据
            if (int.TryParse(comboBoxPageSize.SelectedItem.ToString(), out int newPageSize))
            {
                _pageSize = newPageSize;
                LoadSvgData(1); // 重置到第一页
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

        private void dataGridViewSvg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var svgData = (SvgData)dataGridViewSvg.Rows[e.RowIndex].DataBoundItem;
                if (svgData != null)
                {
                    var previewForm = new SvgPreviewForm(svgData);
                    previewForm.ShowDialog();
                }
            }
        }

        private async void buttonConvertColors_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRows = dataGridViewSvg.SelectedRows;
                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("请选择要转换颜色的SVG", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int convertedCount = 0;
                int totalRows = selectedRows.Count;
                int currentRow = 0;

                foreach (DataGridViewRow row in selectedRows)
                {
                    currentRow++;
                    var svgData = (SvgData)row.DataBoundItem;
                    if (svgData != null)
                    {
                        string originalContent = svgData.Content;
                        string convertedContent = _colorConverter.NormalizeSvgColors(originalContent);

                        if (originalContent != convertedContent)
                        {
                            UpdateStatus($"正在更新 {currentRow}/{totalRows}: {svgData.Name}");
                            
                            svgData.Content = convertedContent;
                            svgData.UpdatedAt = DateTime.Now;
                            convertedCount++;
                        }
                    }
                }

                if (convertedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    UpdateStatus($"成功转换 {convertedCount} 个SVG的颜色为RGBA格式");
                    MessageBox.Show($"成功转换 {convertedCount} 个SVG的颜色为RGBA格式", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSvgData(_currentPage);
                }
                else
                {
                    UpdateStatus("没有需要转换的颜色");
                    MessageBox.Show("没有需要转换的颜色", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"转换颜色失败: {ex.Message}");
                MessageBox.Show($"转换颜色失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            var configForm = new ConfigForm();
            configForm.ShowDialog();
        }

        private void buttonViewAll_Click(object sender, EventArgs e)
        {
            var allSvgForm = new AllSvgForm();
            allSvgForm.ShowDialog();
        }

        private void buttonViewAttributes_Click(object sender, EventArgs e)
        {
            if (dataGridViewSvg.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewSvg.SelectedRows[0];
                var svgData = (SvgData)selectedRow.DataBoundItem;
                if (svgData != null)
                {
                    var attributeForm = new SvgAttributeForm(svgData);
                    attributeForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("请选择一个SVG文件查看属性", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void buttonLoadLocal_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "选择包含SVG文件的文件夹";
                folderBrowser.ShowNewFolderButton = true;

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowser.SelectedPath;
                    UpdateStatus("正在加载本地SVG文件...");

                    try
                    {
                        _localSvgData = (List<SvgData>)await _localSvgLoader.LoadFromFolderAsync(folderPath);
                        dataGridViewSvg.DataSource = _localSvgData;
                        UpdateStatus($"成功加载 {_localSvgData.Count} 个本地SVG文件");
                        MessageBox.Show($"成功加载 {_localSvgData.Count} 个本地SVG文件", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"加载本地SVG文件失败: {ex.Message}");
                        MessageBox.Show($"加载本地SVG文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dataGridViewSvg_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSvg.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewSvg.SelectedRows[0];
                var svgData = (SvgData)selectedRow.DataBoundItem;
                if (svgData != null)
                {
                    UpdatePreview(svgData);
                }
            }
        }

        // 缓存预览HTML内容，避免重复生成
        private Dictionary<int, string> _previewCache = new Dictionary<int, string>();

        private async void UpdatePreview(SvgData svgData)
        {
            if (svgData == null)
                return;

            try
            {
                // 检查缓存中是否已有该SVG的预览HTML
                if (!_previewCache.TryGetValue(svgData.Id, out string html))
                {
                    // 使用WebBrowser控件显示SVG
                    string svgContent = svgData.Content;
                    // 对SVG内容进行HTML编码，确保特殊字符不会破坏HTML结构
                    svgContent = System.Web.HttpUtility.HtmlEncode(svgContent);
                    // 然后将编码后的内容中的<和>转换回原样，以确保SVG标签能够正确解析
                    svgContent = svgContent.Replace("&lt;", "<").Replace("&gt;", ">");
                    
                    html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>SVG预览 - {System.Web.HttpUtility.HtmlEncode(svgData.Name)}</title>
    <style>
        body {{
            margin: 0;
            padding: 20px;
            background-color: #f0f2f5;
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
</body>
</html>
";
                    
                    // 将生成的HTML缓存起来
                    _previewCache[svgData.Id] = html;
                }

                // 异步更新WebBrowser控件，避免阻塞UI线程
                await Task.Run(() =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        webBrowserPreview.DocumentText = html;
                    });
                });
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
            background-color: #f0f2f5;
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