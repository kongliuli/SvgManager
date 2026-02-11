using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SvgColorNormalizer.Core;
using SvgColorNormalizer.Core.DataSources;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.UI
{
    public partial class MainForm : Form
    {
        private List<SvgData> _loadedSvgData = new();
        private List<SvgProcessResult> _processResults = new();
        private SvgProcessor _processor = new();
        private ComboBox _colorFormatComboBox;

        public MainForm()
        {
            InitializeComponent();
        }

        private async void LoadFolder_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _statusLabel.Text = "正在加载...";

                try
                {
                    var source = new FolderSvgSource(dialog.SelectedPath);
                    _loadedSvgData = (await source.LoadAsync()).ToList();

                    _fileGrid.Rows.Clear();

                    foreach (var svgData in _loadedSvgData)
                    {
                        _fileGrid.Rows.Add(
                            Path.GetFileName(svgData.Source),
                            "待处理",
                            0
                        );
                    }

                    _statusLabel.Text = $"已加载 {_loadedSvgData.Count} 个SVG文件";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载失败: {ex.Message}");
                    _statusLabel.Text = "加载失败";
                }
            }
        }

        private async void LoadDatabase_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "SQLite数据库文件|*.db|所有文件|*.*",
                Title = "选择SQLite数据库文件"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _statusLabel.Text = "正在加载数据库...";

                try
                {
                    var source = new SqliteSvgSource(dialog.FileName);
                    _loadedSvgData = (await source.LoadAsync()).ToList();

                    _fileGrid.Rows.Clear();

                    foreach (var svgData in _loadedSvgData)
                    {
                        _fileGrid.Rows.Add(
                            svgData.Source,
                            "待处理",
                            0
                        );
                    }

                    _statusLabel.Text = $"已从数据库加载 {_loadedSvgData.Count} 个SVG文件";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载失败: {ex.Message}");
                    _statusLabel.Text = "加载失败";
                }
            }
        }

        private async void Process_Click(object sender, EventArgs e)
        {
            if (_loadedSvgData.Count == 0)
            {
                MessageBox.Show("请先加载SVG文件");
                return;
            }

            _statusLabel.Text = "正在处理...";

            // 获取选择的颜色格式
            var targetFormat = (ColorNormalizer.TargetColorFormat)_colorFormatComboBox.SelectedIndex;

            var batchProcessor = new SvgBatchProcessor();
            var batchResult = await batchProcessor.ProcessBatchAsync(_loadedSvgData, targetFormat);
            _processResults = batchResult.IndividualResults;

            for (int i = 0; i < _fileGrid.Rows.Count && i < batchResult.IndividualResults.Count; i++)
            {
                var result = batchResult.IndividualResults[i];
                var row = _fileGrid.Rows[i];

                if (result.Success)
                {
                    row.Cells["Status"].Value = "已完成";
                    row.Cells["ModifiedCount"].Value = result.ModifiedCount;
                }
                else
                {
                    row.Cells["Status"].Value = "失败";
                    row.Cells["ModifiedCount"].Value = 0;
                }
            }

            _statusLabel.Text = $"处理完成: 成功 {batchResult.SuccessCount}, 失败 {batchResult.FailedCount}, 耗时 {batchResult.Duration.TotalSeconds:F2}秒";
        }

        private void FileGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_fileGrid.SelectedRows.Count == 0) return;

            var row = _fileGrid.SelectedRows[0];
            int index = _fileGrid.Rows.IndexOf(row);

            if (index >= 0 && index < _processResults.Count)
            {
                var result = _processResults[index];
                _previewTextBox.Text = result.NormalizedSvg ?? _loadedSvgData[index].Content;

                _colorGrid.Rows.Clear();
                foreach (var change in result.ColorChanges)
                {
                    _colorGrid.Rows.Add(
                        change.Element,
                        change.Attribute,
                        change.Original,
                        change.Normalized
                    );
                }
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt|CSV文件|*.csv",
                DefaultExt = "txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var writer = new StreamWriter(saveDialog.FileName);
                    writer.WriteLine("SVG颜色标准化报告");
                    writer.WriteLine($"导出时间: {DateTime.Now}");
                    writer.WriteLine($"总计文件: {_fileGrid.Rows.Count}");
                    writer.WriteLine();

                    for (int i = 0; i < _fileGrid.Rows.Count && i < _processResults.Count; i++)
                    {
                        var row = _fileGrid.Rows[i];
                        var result = _processResults[i];

                        writer.WriteLine($"文件: {row.Cells["Source"].Value}");
                        writer.WriteLine($"状态: {row.Cells["Status"].Value}");
                        writer.WriteLine($"修改数: {row.Cells["ModifiedCount"].Value}");

                        foreach (var change in result.ColorChanges)
                        {
                            writer.WriteLine($"  {change.Element}.{change.Attribute}: {change.Original} → {change.Normalized}");
                        }

                        writer.WriteLine();
                    }

                    MessageBox.Show("报告导出成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}");
                }
            }
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            if (_processResults.Count == 0)
            {
                MessageBox.Show("没有处理结果可保存");
                return;
            }

            _statusLabel.Text = "正在保存...";
            int savedCount = 0;

            try
            {
                for (int i = 0; i < _processResults.Count; i++)
                {
                    var result = _processResults[i];
                    if (!result.Success)
                        continue;

                    // 根据Metadata保存
                    if (result.Metadata != null)
                    {
                        // 保存文件
                        if (result.Metadata.TryGetValue("filePath", out var filePathObj) && filePathObj is string filePath)
                        {
                            File.WriteAllText(filePath, result.NormalizedSvg);
                            savedCount++;
                        }
                        // 保存到数据库
                        else if (result.Metadata.TryGetValue("dbId", out var dbIdObj) && result.Metadata.TryGetValue("connectionString", out var connectionStringObj) && connectionStringObj is string connectionString)
                        {
                            var sqliteSource = new SqliteSvgSource(connectionString);
                            var svgData = new SvgData
                            {
                                Source = result.Source,
                                Content = result.NormalizedSvg,
                                Metadata = result.Metadata
                            };
                            await sqliteSource.SaveAsync(svgData);
                            savedCount++;
                        }
                    }
                }

                _statusLabel.Text = $"保存完成: 成功保存 {savedCount} 个文件";
                MessageBox.Show($"成功保存 {savedCount} 个文件");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}");
                _statusLabel.Text = "保存失败";
            }
        }

        // 控件字段
        private DataGridView _fileGrid;
        private DataGridView _colorGrid;
        private TextBox _previewTextBox;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;

        private void InitializeComponent()
        {
            this.Text = "SVG颜色标准化工具";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 顶部工具栏
            var toolStrip = new ToolStrip();

            var loadFolderBtn = new ToolStripButton("加载文件夹");
            loadFolderBtn.Click += LoadFolder_Click;

            var loadDbBtn = new ToolStripButton("加载数据库");
            loadDbBtn.Click += LoadDatabase_Click;

            var processBtn = new ToolStripButton("批量处理");
            processBtn.Click += Process_Click;

            var saveBtn = new ToolStripButton("保存修改");
            saveBtn.Click += Save_Click;

            var exportBtn = new ToolStripButton("导出报告");
            exportBtn.Click += Export_Click;

            // 颜色格式选择
            var colorFormatLabel = new ToolStripLabel("颜色格式:");
            _colorFormatComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };
            _colorFormatComboBox.Items.AddRange(new string[] { "RGBA", "Hex8" });
            _colorFormatComboBox.SelectedIndex = 0;
            var colorFormatHost = new ToolStripControlHost(_colorFormatComboBox);

            toolStrip.Items.AddRange(new ToolStripItem[] { loadFolderBtn, loadDbBtn, processBtn, saveBtn, exportBtn, new ToolStripSeparator(), colorFormatLabel, colorFormatHost });

            // 主分割容器
            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };

            // 上部分：文件列表
            _fileGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            _fileGrid.Columns.Add("Source", "来源");
            _fileGrid.Columns.Add("Status", "状态");
            _fileGrid.Columns.Add("ModifiedCount", "修改数");

            _fileGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _fileGrid.SelectionChanged += FileGrid_SelectionChanged;

            // 下部分：颜色变更详情 + 预览
            var bottomSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400
            };

            // 颜色变更列表
            _colorGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true
            };

            _colorGrid.Columns.Add("Element", "元素");
            _colorGrid.Columns.Add("Attribute", "属性");
            _colorGrid.Columns.Add("Original", "原始颜色");
            _colorGrid.Columns.Add("Normalized", "标准化颜色");

            // 预览面板
            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            var previewLabel = new Label
            {
                Text = "SVG预览",
                Dock = DockStyle.Top,
                Height = 25
            };

            _previewTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            previewPanel.Controls.Add(_previewTextBox);
            previewPanel.Controls.Add(previewLabel);

            bottomSplit.Panel1.Controls.Add(_colorGrid);
            bottomSplit.Panel2.Controls.Add(previewPanel);

            mainSplit.Panel1.Controls.Add(_fileGrid);
            mainSplit.Panel2.Controls.Add(bottomSplit);

            // 状态栏
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel("就绪");
            _statusStrip.Items.Add(_statusLabel);

            this.Controls.Add(mainSplit);
            this.Controls.Add(toolStrip);
            this.Controls.Add(_statusStrip);
        }
    }
}
