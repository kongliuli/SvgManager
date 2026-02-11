# SVG颜色标准化工具 - 开发指南

## 项目概述

这是一个基于 .NET 6 WinForms 的 SVG 颜色标准化工具，支持从多种数据源（本地文件夹、SQLite、达梦数据库）加载 SVG 文件，并自动将所有颜色格式统一转换为 RGB/RGBA 格式。

## 核心功能

- 多数据源支持：本地文件夹、SQLite数据库、达梦数据库
- 颜色格式统一：rgb/rgba、6位/8位16进制、命名颜色 → 统一转为 rgb/rgba
- 批量处理能力：支持大规模文件处理
- 可视化界面：实时查看处理进度和颜色变更详情
- 详细报告：导出处理结果和颜色变更记录

## 技术栈

- .NET 6.0
- WinForms
- System.Data.SQLite
- System.Xml.Linq
- DmProvider（达梦数据库，可选）

## 项目结构

```
SvgColorNormalizer/
├── Models/                    # 数据模型
│   ├── SvgData.cs
│   ├── SvgProcessResult.cs
│   └── ColorChange.cs
├── Core/                      # 核心处理逻辑
│   ├── ColorNormalizer.cs     # 颜色转换器
│   ├── SvgProcessor.cs        # SVG处理引擎
│   └── SvgBatchProcessor.cs   # 批量处理器
├── DataSources/               # 数据源适配器
│   ├── ISvgSource.cs          # 数据源接口
│   ├── FolderSvgSource.cs     # 文件夹数据源
│   ├── SqliteSvgSource.cs     # SQLite数据源
│   └── DmSvgSource.cs         # 达梦数据库数据源
├── UI/                        # 界面层
│   ├── MainForm.cs            # 主窗体
│   ├── MainForm.Designer.cs   # 窗体设计器（可选）
│   └── Program.cs             # 程序入口
├── Test/                      # 测试数据
│   ├── sample1.svg
│   ├── sample2.svg
│   └── test.sql
├── SvgColorNormalizer.csproj  # 项目文件
└── README.md                  # 本文件
```

## 开发步骤

### 第一步：创建项目

```bash
# 创建新的 WinForms 项目
dotnet new winforms -n SvgColorNormalizer
cd SvgColorNormalizer

# 创建目录结构
mkdir Models Core DataSources UI Test
```

### 第二步：安装 NuGet 包

```bash
# 安装 SQLite 支持
dotnet add package System.Data.SQLite

# 如果需要达梦数据库支持
# dotnet add package DmProvider
```

### 第三步：创建数据模型

创建 `Models/SvgData.cs`:
```csharp
namespace SvgColorNormalizer.Models
{
    public class SvgData
    {
        public string Source { get; set; }
        public string Content { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
```

创建 `Models/SvgProcessResult.cs`:
```csharp
using System.Collections.Generic;

namespace SvgColorNormalizer.Models
{
    public class SvgProcessResult
    {
        public bool Success { get; set; }
        public string NormalizedSvg { get; set; }
        public List<ColorChange> ColorChanges { get; set; } = new();
        public int ModifiedCount { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
```

创建 `Models/ColorChange.cs`:
```csharp
namespace SvgColorNormalizer.Models
{
    public class ColorChange
    {
        public string Original { get; set; }
        public string Normalized { get; set; }
        public string Element { get; set; }
        public string Attribute { get; set; }
    }
}
```

### 第四步：实现核心处理逻辑

创建 `Core/ColorNormalizer.cs`:
```csharp
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
```

创建 `Core/SvgProcessor.cs`:
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SvgColorNormalizer.Models;

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

        public SvgProcessResult ProcessSvg(string svgContent)
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
                        string normalizedColor = _colorNormalizer.NormalizeColor(originalColor);

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
```

创建 `Core/SvgBatchProcessor.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.Core
{
    public class SvgBatchProcessor
    {
        private readonly SvgProcessor _processor = new();

        public async Task<BatchProcessResult> ProcessBatchAsync(IEnumerable<SvgData> svgDataList)
        {
            var result = new BatchProcessResult
            {
                TotalCount = svgDataList.Count(),
                StartTime = DateTime.Now,
                IndividualResults = new List<SvgProcessResult>()
            };

            foreach (var svgData in svgDataList)
            {
                var processResult = _processor.ProcessSvg(svgData.Content);
                processResult.Source = svgData.Source;
                processResult.Metadata = svgData.Metadata;

                result.IndividualResults.Add(processResult);

                if (processResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.ErrorMessage += $"\n{svgData.Source}: {processResult.ErrorMessage}";
                }
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }
    }

    public class BatchProcessResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public List<SvgProcessResult> IndividualResults { get; set; }
        public string ErrorMessage { get; set; }
    }
}
```

### 第五步：实现数据源适配器

创建 `DataSources/ISvgSource.cs`:
```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public interface ISvgSource
    {
        string SourceType { get; }
        Task<IEnumerable<SvgData>> LoadAsync();
    }
}
```

创建 `DataSources/FolderSvgSource.cs`:
```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public class FolderSvgSource : ISvgSource
    {
        private readonly string _folderPath;

        public FolderSvgSource(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string SourceType => "本地文件夹";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();

            await Task.Run(() =>
            {
                var svgFiles = Directory.GetFiles(_folderPath, "*.svg", SearchOption.AllDirectories);

                foreach (var file in svgFiles)
                {
                    results.Add(new SvgData
                    {
                        Source = file,
                        Content = File.ReadAllText(file),
                        Metadata = new Dictionary<string, object> { ["filePath"] = file }
                    });
                }
            });

            return results;
        }
    }
}
```

创建 `DataSources/SqliteSvgSource.cs`:
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public class SqliteSvgSource : ISvgSource
    {
        private readonly string _dbPath;

        public SqliteSvgSource(string dbPath)
        {
            _dbPath = dbPath;
        }

        public string SourceType => "SQLite数据库";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();
            var connectionString = $"Data Source={_dbPath};Version=3;";

            await Task.Run(() =>
            {
                using var conn = new SQLiteConnection(connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, svg_content FROM svg_assets";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new SvgData
                    {
                        Source = $"SQLite:{reader.GetInt32(0)}",
                        Content = reader.GetString(1),
                        Metadata = new Dictionary<string, object> { ["dbId"] = reader.GetInt32(0) }
                    });
                }
            });

            return results;
        }
    }
}
```

创建 `DataSources/DmSvgSource.cs`:
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dm;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.DataSources
{
    public class DmSvgSource : ISvgSource
    {
        private readonly string _connectionString;

        public DmSvgSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string SourceType => "达梦数据库";

        public async Task<IEnumerable<SvgData>> LoadAsync()
        {
            var results = new List<SvgData>();

            await Task.Run(() =>
            {
                using var conn = new DmConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, svg_content FROM svg_assets WHERE is_active = 1";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new SvgData
                    {
                        Source = $"DM:{reader.GetInt32(0)}",
                        Content = reader.GetString(1),
                        Metadata = new Dictionary<string, object> { ["dbId"] = reader.GetInt32(0) }
                    });
                }
            });

            return results;
        }
    }
}
```

### 第六步：创建用户界面

创建 `UI/MainForm.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SvgColorNormalizer.Core;
using SvgColorNormalizer.DataSources;
using SvgColorNormalizer.Models;

namespace SvgColorNormalizer.UI
{
    public partial class MainForm : Form
    {
        private List<SvgData> _loadedSvgData = new();
        private List<SvgProcessResult> _processResults = new();
        private SvgProcessor _processor = new();

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

        private async void Process_Click(object sender, EventArgs e)
        {
            if (_loadedSvgData.Count == 0)
            {
                MessageBox.Show("请先加载SVG文件");
                return;
            }

            _statusLabel.Text = "正在处理...";

            var batchProcessor = new SvgBatchProcessor();
            var batchResult = await batchProcessor.ProcessBatchAsync(_loadedSvgData);
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

            var processBtn = new ToolStripButton("批量处理");
            processBtn.Click += Process_Click;

            var exportBtn = new ToolStripButton("导出报告");
            exportBtn.Click += Export_Click;

            toolStrip.Items.AddRange(new ToolStripItem[] { loadFolderBtn, processBtn, exportBtn });

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
```

创建 `UI/Program.cs`:
```csharp
using System;
using System.Windows.Forms;
using SvgColorNormalizer.UI;

namespace SvgColorNormalizer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
```

### 第七步：配置项目文件

编辑 `SvgColorNormalizer.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Data.SQLite" Version="1.0.118" />
  </ItemGroup>

  <!-- 达梦数据库包（根据实际情况添加）-->
  <ItemGroup Condition="'$(IncludeDm)' == 'true'">
    <PackageReference Include="DmProvider" Version="8.1.2.192" />
  </ItemGroup>
</Project>
```

### 第八步：创建测试数据

创建 `Test/sample1.svg`:
```xml
<svg width="200" height="200" xmlns="http://www.w3.org/2000/svg">
  <!-- 命名颜色 -->
  <rect x="10" y="10" width="80" height="80" fill="red" stroke="blue" stroke-width="2"/>
  <!-- 6位16进制 -->
  <rect x="110" y="10" width="80" height="80" fill="#FF5733" stroke="#00FF00"/>
  <!-- 8位16进制 -->
  <circle cx="50" cy="150" r="40" fill="#FF000080"/>
  <!-- RGB格式 -->
  <rect x="110" y="110" width="80" height="80" fill="rgb(128, 64, 192)"/>
  <!-- RGBA格式 -->
  <circle cx="150" cy="150" r="30" fill="rgba(255, 165, 0, 0.5)"/>
</svg>
```

创建 `Test/sample2.svg`:
```xml
<svg width="300" height="200" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="0%">
      <stop offset="0%" style="stop-color:#ff0000;stop-opacity:1" />
      <stop offset="50%" style="stop-color:rgb(0,255,0);stop-opacity:1" />
      <stop offset="100%" style="stop-color:rgba(0,0,255,0.5);stop-opacity:1" />
    </linearGradient>
  </defs>
  <rect x="10" y="10" width="280" height="80" fill="url(#grad1)" stroke="#333" stroke-width="2"/>
  <circle cx="150" cy="150" r="40" fill="yellow" stroke="#FFA500" stroke-width="3"/>
  <rect x="50" y="130" width="200" height="50" fill="#0080FF80" stroke="#000000"/>
</svg>
```

创建 `Test/test.sql`:
```sql
-- 创建测试数据库
CREATE TABLE svg_assets (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    svg_content TEXT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 插入测试数据
INSERT INTO svg_assets (name, svg_content) VALUES
('示例图形1', '<svg width="100" height="100" xmlns="http://www.w3.org/2000/svg">
    <rect x="10" y="10" width="80" height="80" fill="#FF0000" stroke="#00FF00"/>
</svg>'),

('示例图形2', '<svg width="150" height="100" xmlns="http://www.w3.org/2000/svg">
    <circle cx="50" cy="50" r="40" fill="rgb(255, 128, 0)" stroke="#0000FF"/>
    <rect x="100" y="20" width="40" height="60" fill="#FF000080"/>
</svg>'),

('复杂渐变', '<svg width="200" height="100" xmlns="http://www.w3.org/2000/svg">
    <defs>
        <linearGradient id="grad">
            <stop offset="0%" stop-color="#FF0000"/>
            <stop offset="100%" stop-color="#0000FF"/>
        </linearGradient>
    </defs>
    <rect x="10" y="10" width="180" height="80" fill="url(#grad)"/>
</svg>');
```

## 编译和运行

### 编译项目

```bash
# 在项目根目录执行
dotnet build
```

### 运行项目

```bash
# 运行程序
dotnet run
```

### 创建发布版本

```bash
# 创建单文件发布
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 功能使用说明

### 1. 加载SVG文件

- 点击"加载文件夹"按钮
- 选择包含SVG文件的文件夹
- 程序会自动扫描并加载所有.svg文件

### 2. 批量处理

- 点击"批量处理"按钮
- 程序会自动处理所有已加载的SVG文件
- 将所有颜色格式统一转换为RGB/RGBA格式
- 处理完成后会显示统计信息

### 3. 查看处理结果

- 在文件列表中点击任意一行
- 右侧会显示该文件的颜色变更详情
- 预览面板会显示处理后的SVG代码

### 4. 导出报告

- 点击"导出报告"按钮
- 选择保存位置和文件格式
- 程序会生成详细的处理报告

## 颜色转换规则

| 原始格式 | 目标格式 | 示例 |
|---------|---------|------|
| 命名颜色 | rgb() | red → rgb(255, 0, 0) |
| 6位16进制 | rgb() | #FF5733 → rgb(255, 87, 51) |
| 8位16进制 | rgba() | #FF000080 → rgba(255, 0, 0, 0.5) |
| rgb() | rgb() | rgb(128, 64, 192) → rgb(128, 64, 192) |
| rgba() | rgba() | rgba(255, 165, 0, 0.5) → rgba(255, 165, 0, 0.5) |

## 扩展功能

### 添加新的颜色格式支持

在 `ColorNormalizer.cs` 中添加新的正则表达式和转换逻辑。

### 添加新的数据源

1. 创建新的类实现 `ISvgSource` 接口
2. 在界面中添加对应的加载按钮

### 添加达梦数据库支持

1. 取消项目文件中达梦包的注释
2. 重新编译项目
3. 在界面中添加达梦数据库连接配置

## 常见问题

### Q: 编译时提示找不到SQLite

A: 运行 `dotnet add package System.Data.SQLite` 安装SQLite包

### Q: 达梦数据库连接失败

A: 确保已安装达梦数据库客户端并正确配置连接字符串

### Q: 处理大文件时程序卡顿

A: 建议将大文件分批处理，或使用异步处理方式

## 技术支持

如遇到问题，请检查：
1. .NET 6.0 SDK 是否正确安装
2. NuGet包是否完整安装
3. 测试文件格式是否正确

## 许可证

本项目仅供学习和参考使用。
