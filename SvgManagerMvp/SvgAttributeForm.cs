using SvgManagerMvp.Models;
using SvgManagerMvp.Services;
using System.Drawing;
using System.Windows.Forms;

namespace SvgManagerMvp
{
    public partial class SvgAttributeForm : Form
    {
        private SvgData _svgData;
        private SvgAttributeService _attributeService;

        public SvgAttributeForm(SvgData svgData)
        {
            InitializeComponent();
            _svgData = svgData;
            _attributeService = new SvgAttributeService();
            InitializeModernTheme();
            LoadAttributes();
        }

        private void InitializeModernTheme()
        {
            // 设置窗体样式
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Text = "SVG 属性查看";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 设置标签样式
            labelSvgName.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            labelSvgName.ForeColor = Color.FromArgb(30, 30, 30);
            labelSvgName.Padding = new Padding(10, 10, 10, 5);

            // 设置属性列表样式
            listViewAttributes.View = View.Details;
            listViewAttributes.GridLines = true;
            listViewAttributes.FullRowSelect = true;
            listViewAttributes.MultiSelect = false;
            listViewAttributes.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            // 添加列
            listViewAttributes.Columns.Add("属性", 120);
            listViewAttributes.Columns.Add("值", 300);

            // 设置列表视图样式
            listViewAttributes.BackColor = Color.White;
            listViewAttributes.ForeColor = Color.FromArgb(30, 30, 30);
            listViewAttributes.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);

            // 设置关闭按钮样式
            buttonClose.BackColor = Color.FromArgb(51, 102, 204);
            buttonClose.ForeColor = Color.White;
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.FlatAppearance.BorderSize = 0;
            buttonClose.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            buttonClose.Padding = new Padding(5, 2, 5, 2);
            buttonClose.MouseEnter += (sender, e) => {
                buttonClose.BackColor = Color.FromArgb(61, 112, 214);
                buttonClose.Cursor = Cursors.Hand;
            };
            buttonClose.MouseLeave += (sender, e) => {
                buttonClose.BackColor = Color.FromArgb(51, 102, 204);
                buttonClose.Cursor = Cursors.Default;
            };
        }

        private void LoadAttributes()
        {
            if (_svgData == null)
                return;

            // 设置 SVG 名称
            labelSvgName.Text = _svgData.Name;

            // 提取属性
            var attributes = _attributeService.ExtractAttributes(_svgData.Content);

            // 清空列表
            listViewAttributes.Items.Clear();

            // 添加属性项
            AddAttributeItem("名称", _svgData.Name);
            AddAttributeItem("宽度", attributes.Width);
            AddAttributeItem("高度", attributes.Height);
            AddAttributeItem("ViewBox", attributes.ViewBox);
            AddAttributeItem("元素数量", attributes.ElementCount.ToString());
            AddAttributeItem("颜色数量", attributes.Colors.Count.ToString());

            // 添加颜色信息
            if (attributes.Colors.Count > 0)
            {
                AddAttributeItem("颜色列表", string.Empty);
                foreach (var color in attributes.Colors)
                {
                    AddAttributeItem("", color);
                }
            }

            // 调整列宽
            listViewAttributes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void AddAttributeItem(string name, string value)
        {
            var item = new ListViewItem(name);
            item.SubItems.Add(value);
            listViewAttributes.Items.Add(item);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}