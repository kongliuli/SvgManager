using Microsoft.EntityFrameworkCore;
using SvgManagerMvp.Data;
using SvgManagerMvp.Models;
using SvgManagerMvp.Services;

namespace SvgManagerMvp
{
    public partial class MainForm : Form
    {
        private SvgDbContext _context;
        private SvgColorConverter _colorConverter;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private ToolStripStatusLabel _statusLabel;

        public MainForm()
        {
            InitializeComponent();
            InitializeStatusStrip();
            _colorConverter = new SvgColorConverter();
            InitializeDatabase();
            InitializePageSizeComboBox();
            LoadSvgData();
        }

        private void InitializePageSizeComboBox()
        {
            comboBoxPageSize.SelectedIndex = 0; // 默认选择10
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
    }
}