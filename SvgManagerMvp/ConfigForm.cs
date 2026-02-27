using SvgManagerMvp.Data;

namespace SvgManagerMvp
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            LoadConnectionString();
        }

        private void LoadConnectionString()
        {
            textBoxConnectionString.Text = ConfigManager.GetConnectionString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = textBoxConnectionString.Text;
                ConfigManager.SaveConnectionString(connectionString);
                MessageBox.Show("连接字符串保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}