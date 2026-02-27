using System; using System.Collections.Generic; using System.Drawing; using System.Windows.Forms; using SvgColorNormalizer.Core.DataSources;

namespace SvgColorNormalizer.UI
{
    public class DataSourceConfigForm : Form
    {
        private DataSourceConfigManager _configManager;
        private DataGridView _dataSourceGrid;
        private DataGridView _storageGrid;
        private Button _addDataSourceBtn;
        private Button _editDataSourceBtn;
        private Button _deleteDataSourceBtn;
        private Button _addStorageBtn;
        private Button _editStorageBtn;
        private Button _deleteStorageBtn;
        private Button _saveBtn;
        private Button _cancelBtn;

        public DataSourceConfigForm()
        {
            _configManager = new DataSourceConfigManager();
            InitializeComponent();
            LoadConfigData();
        }

        private void LoadConfigData()
        {
            // 加载数据源配置
            _dataSourceGrid.Rows.Clear();
            foreach (var dataSource in _configManager.Config.DataSources)
            {
                _dataSourceGrid.Rows.Add(dataSource.Name, dataSource.Type, string.Join(";", dataSource.Configuration));
            }

            // 加载存储配置
            _storageGrid.Rows.Clear();
            foreach (var storage in _configManager.Config.StorageConfigurations)
            {
                _storageGrid.Rows.Add(storage.Name, storage.Type, string.Join(";", storage.Configuration));
            }
        }

        private void InitializeComponent()
        {
            this.Text = "数据源配置管理";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 主分割容器
            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250
            };

            // 上部分：数据源配置
            var dataSourcePanel = new Panel { Dock = DockStyle.Fill };

            var dataSourceLabel = new Label
            {
                Text = "数据源配置",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font(Font, FontStyle.Bold)
            };

            _dataSourceGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true
            };

            _dataSourceGrid.Columns.Add("Name", "名称");
            _dataSourceGrid.Columns.Add("Type", "类型");
            _dataSourceGrid.Columns.Add("Configuration", "配置");

            var dataSourceButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };

            _addDataSourceBtn = new Button { Text = "添加", Width = 80 };
            _editDataSourceBtn = new Button { Text = "编辑", Width = 80 };
            _deleteDataSourceBtn = new Button { Text = "删除", Width = 80 };

            _addDataSourceBtn.Click += AddDataSource_Click;
            _editDataSourceBtn.Click += EditDataSource_Click;
            _deleteDataSourceBtn.Click += DeleteDataSource_Click;

            dataSourceButtons.Controls.Add(_addDataSourceBtn);
            dataSourceButtons.Controls.Add(_editDataSourceBtn);
            dataSourceButtons.Controls.Add(_deleteDataSourceBtn);

            dataSourcePanel.Controls.Add(_dataSourceGrid);
            dataSourcePanel.Controls.Add(dataSourceButtons);
            dataSourcePanel.Controls.Add(dataSourceLabel);

            // 下部分：存储配置
            var storagePanel = new Panel { Dock = DockStyle.Fill };

            var storageLabel = new Label
            {
                Text = "存储配置",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font(Font, FontStyle.Bold)
            };

            _storageGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true
            };

            _storageGrid.Columns.Add("Name", "名称");
            _storageGrid.Columns.Add("Type", "类型");
            _storageGrid.Columns.Add("Configuration", "配置");

            var storageButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };

            _addStorageBtn = new Button { Text = "添加", Width = 80 };
            _editStorageBtn = new Button { Text = "编辑", Width = 80 };
            _deleteStorageBtn = new Button { Text = "删除", Width = 80 };

            _addStorageBtn.Click += AddStorage_Click;
            _editStorageBtn.Click += EditStorage_Click;
            _deleteStorageBtn.Click += DeleteStorage_Click;

            storageButtons.Controls.Add(_addStorageBtn);
            storageButtons.Controls.Add(_editStorageBtn);
            storageButtons.Controls.Add(_deleteStorageBtn);

            storagePanel.Controls.Add(_storageGrid);
            storagePanel.Controls.Add(storageButtons);
            storagePanel.Controls.Add(storageLabel);

            mainSplit.Panel1.Controls.Add(dataSourcePanel);
            mainSplit.Panel2.Controls.Add(storagePanel);

            // 底部按钮
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };

            _saveBtn = new Button { Text = "保存", Width = 100 };
            _cancelBtn = new Button { Text = "取消", Width = 100 };

            _saveBtn.Click += Save_Click;
            _cancelBtn.Click += Cancel_Click;

            buttonPanel.Controls.Add(_cancelBtn);
            buttonPanel.Controls.Add(_saveBtn);

            bottomPanel.Controls.Add(buttonPanel);

            this.Controls.Add(mainSplit);
            this.Controls.Add(bottomPanel);
        }

        private void AddDataSource_Click(object sender, EventArgs e)
        {
            var editor = new DataSourceEditorForm(null);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                _configManager.Config.DataSources.Add(editor.DataSourceConfig);
                LoadConfigData();
            }
        }

        private void EditDataSource_Click(object sender, EventArgs e)
        {
            if (_dataSourceGrid.SelectedRows.Count == 0) return;

            var selectedRow = _dataSourceGrid.SelectedRows[0];
            var name = selectedRow.Cells["Name"].Value.ToString();
            var dataSourceConfig = _configManager.Config.DataSources.Find(ds => ds.Name == name);

            if (dataSourceConfig != null)
            {
                var editor = new DataSourceEditorForm(dataSourceConfig);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadConfigData();
                }
            }
        }

        private void DeleteDataSource_Click(object sender, EventArgs e)
        {
            if (_dataSourceGrid.SelectedRows.Count == 0) return;

            var selectedRow = _dataSourceGrid.SelectedRows[0];
            var name = selectedRow.Cells["Name"].Value.ToString();

            if (MessageBox.Show($"确定要删除数据源 '{name}' 吗？", "确认删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _configManager.Config.DataSources.RemoveAll(ds => ds.Name == name);
                LoadConfigData();
            }
        }

        private void AddStorage_Click(object sender, EventArgs e)
        {
            var editor = new StorageEditorForm(null);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                _configManager.Config.StorageConfigurations.Add(editor.StorageConfig);
                LoadConfigData();
            }
        }

        private void EditStorage_Click(object sender, EventArgs e)
        {
            if (_storageGrid.SelectedRows.Count == 0) return;

            var selectedRow = _storageGrid.SelectedRows[0];
            var name = selectedRow.Cells["Name"].Value.ToString();
            var storageConfig = _configManager.Config.StorageConfigurations.Find(s => s.Name == name);

            if (storageConfig != null)
            {
                var editor = new StorageEditorForm(storageConfig);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadConfigData();
                }
            }
        }

        private void DeleteStorage_Click(object sender, EventArgs e)
        {
            if (_storageGrid.SelectedRows.Count == 0) return;

            var selectedRow = _storageGrid.SelectedRows[0];
            var name = selectedRow.Cells["Name"].Value.ToString();

            if (MessageBox.Show($"确定要删除存储配置 '{name}' 吗？", "确认删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _configManager.Config.StorageConfigurations.RemoveAll(s => s.Name == name);
                LoadConfigData();
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            _configManager.SaveConfig();
            MessageBox.Show("配置已保存");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public class DataSourceEditorForm : Form
    {
        public DataSourceConfiguration DataSourceConfig { get; private set; }
        private TextBox _nameTextBox;
        private ComboBox _typeComboBox;
        private Panel _configPanel;
        private Dictionary<string, Control> _configControls = new Dictionary<string, Control>();

        public DataSourceEditorForm(DataSourceConfiguration existingConfig)
        {
            DataSourceConfig = existingConfig ?? new DataSourceConfiguration
            {
                Configuration = new Dictionary<string, string>()
            };

            InitializeComponent();
            LoadExistingConfig();
        }

        private void InitializeComponent()
        {
            this.Text = "编辑数据源配置";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                RowStyles = { new RowStyle(SizeType.Absolute, 40), new RowStyle(SizeType.Absolute, 40), new RowStyle(SizeType.Percent, 100) }
            };

            // 名称
            var nameLabel = new Label { Text = "名称:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            _nameTextBox = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(nameLabel, 0, 0);
            layout.Controls.Add(_nameTextBox, 1, 0);

            // 类型
            var typeLabel = new Label { Text = "类型:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            _typeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _typeComboBox.Items.AddRange(new string[] { "本地文件夹", "SQLite数据库", "SQL Server数据库", "PostgreSQL数据库", /* "达梦数据库", */ "API接口" });
            _typeComboBox.SelectedIndexChanged += TypeComboBox_SelectedIndexChanged;
            layout.Controls.Add(typeLabel, 0, 1);
            layout.Controls.Add(_typeComboBox, 1, 1);

            // 配置
            var configLabel = new Label { Text = "配置:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.TopLeft, Padding = new Padding(0, 5, 0, 0) };
            _configPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            layout.Controls.Add(configLabel, 0, 2);
            layout.Controls.Add(_configPanel, 1, 2);

            // 底部按钮
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var flowPanel = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10) };

            var saveBtn = new Button { Text = "保存", Width = 100 };
            var cancelBtn = new Button { Text = "取消", Width = 100 };

            saveBtn.Click += Save_Click;
            cancelBtn.Click += Cancel_Click;

            flowPanel.Controls.Add(cancelBtn);
            flowPanel.Controls.Add(saveBtn);
            buttonPanel.Controls.Add(flowPanel);

            mainPanel.Controls.Add(layout);
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadExistingConfig()
        {
            _nameTextBox.Text = DataSourceConfig.Name;
            if (!string.IsNullOrEmpty(DataSourceConfig.Type))
            {
                _typeComboBox.SelectedItem = DataSourceConfig.Type;
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _configPanel.Controls.Clear();
            _configControls.Clear();

            var selectedType = _typeComboBox.SelectedItem.ToString();
            var configTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true
            };

            switch (selectedType)
            {
                case "本地文件夹":
                    AddConfigRow(configTable, "FolderPath", "文件夹路径:");
                    break;
                case "SQLite数据库":
                    AddConfigRow(configTable, "DatabasePath", "数据库路径:");
                    break;
                case "SQL Server数据库":
                    AddConfigRow(configTable, "Server", "服务器地址:");
                    AddConfigRow(configTable, "Database", "数据库名称:");
                    AddConfigRow(configTable, "User Id", "用户名:");
                    AddConfigRow(configTable, "Password", "密码:");
                    break;
                case "PostgreSQL数据库":
                    AddConfigRow(configTable, "Host", "主机地址:");
                    AddConfigRow(configTable, "Port", "端口:");
                    AddConfigRow(configTable, "Database", "数据库名称:");
                    AddConfigRow(configTable, "Username", "用户名:");
                    AddConfigRow(configTable, "Password", "密码:");
                    break;
                // case "达梦数据库":
                //     AddConfigRow(configTable, "Server", "服务器地址:");
                //     AddConfigRow(configTable, "Port", "端口:");
                //     AddConfigRow(configTable, "Database", "数据库名称:");
                //     AddConfigRow(configTable, "User Id", "用户名:");
                //     AddConfigRow(configTable, "Password", "密码:");
                //     break;
                case "API接口":
                    AddConfigRow(configTable, "ApiUrl", "API地址:");
                    AddConfigRow(configTable, "ApiKey", "API密钥:");
                    break;
            }

            _configPanel.Controls.Add(configTable);

            // 加载现有配置
            foreach (var kvp in DataSourceConfig.Configuration)
            {
                if (_configControls.TryGetValue(kvp.Key, out var control) && control is TextBox textBox)
                {
                    textBox.Text = kvp.Value;
                }
            }
        }

        private void AddConfigRow(TableLayoutPanel table, string key, string labelText)
        {
            var rowIndex = table.RowCount;
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var label = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var textBox = new TextBox { Dock = DockStyle.Fill };

            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(textBox, 1, rowIndex);

            _configControls[key] = textBox;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            DataSourceConfig.Name = _nameTextBox.Text;
            DataSourceConfig.Type = _typeComboBox.SelectedItem.ToString();
            DataSourceConfig.Configuration.Clear();

            foreach (var kvp in _configControls)
            {
                DataSourceConfig.Configuration[kvp.Key] = (kvp.Value as TextBox).Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public class StorageEditorForm : Form
    {
        public StorageConfiguration StorageConfig { get; private set; }
        private TextBox _nameTextBox;
        private ComboBox _typeComboBox;
        private Panel _configPanel;
        private Dictionary<string, Control> _configControls = new Dictionary<string, Control>();

        public StorageEditorForm(StorageConfiguration existingConfig)
        {
            StorageConfig = existingConfig ?? new StorageConfiguration
            {
                Configuration = new Dictionary<string, string>()
            };

            InitializeComponent();
            LoadExistingConfig();
        }

        private void InitializeComponent()
        {
            this.Text = "编辑存储配置";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                RowStyles = { new RowStyle(SizeType.Absolute, 40), new RowStyle(SizeType.Absolute, 40), new RowStyle(SizeType.Percent, 100) }
            };

            // 名称
            var nameLabel = new Label { Text = "名称:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            _nameTextBox = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(nameLabel, 0, 0);
            layout.Controls.Add(_nameTextBox, 1, 0);

            // 类型
            var typeLabel = new Label { Text = "类型:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            _typeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _typeComboBox.Items.AddRange(new string[] { "本地文件夹", "SQLite数据库" });
            _typeComboBox.SelectedIndexChanged += TypeComboBox_SelectedIndexChanged;
            layout.Controls.Add(typeLabel, 0, 1);
            layout.Controls.Add(_typeComboBox, 1, 1);

            // 配置
            var configLabel = new Label { Text = "配置:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.TopLeft, Padding = new Padding(0, 5, 0, 0) };
            _configPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            layout.Controls.Add(configLabel, 0, 2);
            layout.Controls.Add(_configPanel, 1, 2);

            // 底部按钮
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var flowPanel = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10) };

            var saveBtn = new Button { Text = "保存", Width = 100 };
            var cancelBtn = new Button { Text = "取消", Width = 100 };

            saveBtn.Click += Save_Click;
            cancelBtn.Click += Cancel_Click;

            flowPanel.Controls.Add(cancelBtn);
            flowPanel.Controls.Add(saveBtn);
            buttonPanel.Controls.Add(flowPanel);

            mainPanel.Controls.Add(layout);
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadExistingConfig()
        {
            _nameTextBox.Text = StorageConfig.Name;
            if (!string.IsNullOrEmpty(StorageConfig.Type))
            {
                _typeComboBox.SelectedItem = StorageConfig.Type;
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _configPanel.Controls.Clear();
            _configControls.Clear();

            var selectedType = _typeComboBox.SelectedItem.ToString();
            var configTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true
            };

            switch (selectedType)
            {
                case "本地文件夹":
                    AddConfigRow(configTable, "FolderPath", "文件夹路径:");
                    break;
                case "SQLite数据库":
                    AddConfigRow(configTable, "DatabasePath", "数据库路径:");
                    break;
            }

            _configPanel.Controls.Add(configTable);

            // 加载现有配置
            foreach (var kvp in StorageConfig.Configuration)
            {
                if (_configControls.TryGetValue(kvp.Key, out var control) && control is TextBox textBox)
                {
                    textBox.Text = kvp.Value;
                }
            }
        }

        private void AddConfigRow(TableLayoutPanel table, string key, string labelText)
        {
            var rowIndex = table.RowCount;
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var label = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var textBox = new TextBox { Dock = DockStyle.Fill };

            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(textBox, 1, rowIndex);

            _configControls[key] = textBox;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            StorageConfig.Name = _nameTextBox.Text;
            StorageConfig.Type = _typeComboBox.SelectedItem.ToString();
            StorageConfig.Configuration.Clear();

            foreach (var kvp in _configControls)
            {
                StorageConfig.Configuration[kvp.Key] = (kvp.Value as TextBox).Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
