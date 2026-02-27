namespace SvgManagerMvp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridViewSvg = new System.Windows.Forms.DataGridView();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelPageInfo = new System.Windows.Forms.Label();
            this.buttonConvertColors = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonViewAll = new System.Windows.Forms.Button();
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.comboBoxPageSize = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSvg)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSvg
            // 
            this.dataGridViewSvg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSvg.Location = new System.Drawing.Point(12, 40);
            this.dataGridViewSvg.Name = "dataGridViewSvg";
            this.dataGridViewSvg.RowTemplate.Height = 25;
            this.dataGridViewSvg.Size = new System.Drawing.Size(800, 360);
            this.dataGridViewSvg.TabIndex = 0;
            this.dataGridViewSvg.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSvg_CellClick);
            // 
            // buttonPrev
            // 
            this.buttonPrev.Location = new System.Drawing.Point(12, 406);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(75, 23);
            this.buttonPrev.TabIndex = 1;
            this.buttonPrev.Text = "上一页";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(93, 406);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "下一页";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelPageInfo
            // 
            this.labelPageInfo.AutoSize = true;
            this.labelPageInfo.Location = new System.Drawing.Point(174, 410);
            this.labelPageInfo.Name = "labelPageInfo";
            this.labelPageInfo.Size = new System.Drawing.Size(89, 15);
            this.labelPageInfo.TabIndex = 3;
            this.labelPageInfo.Text = "第 1 页，共 0 页";
            // 
            // buttonConvertColors
            // 
            this.buttonConvertColors.Location = new System.Drawing.Point(490, 406);
            this.buttonConvertColors.Name = "buttonConvertColors";
            this.buttonConvertColors.Size = new System.Drawing.Size(155, 23);
            this.buttonConvertColors.TabIndex = 4;
            this.buttonConvertColors.Text = "转换选中SVG颜色为RGBA";
            this.buttonConvertColors.UseVisualStyleBackColor = true;
            this.buttonConvertColors.Click += new System.EventHandler(this.buttonConvertColors_Click);
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(651, 406);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonConfig.TabIndex = 5;
            this.buttonConfig.Text = "配置";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // buttonViewAll
            // 
            this.buttonViewAll.Location = new System.Drawing.Point(732, 406);
            this.buttonViewAll.Name = "buttonViewAll";
            this.buttonViewAll.Size = new System.Drawing.Size(75, 23);
            this.buttonViewAll.TabIndex = 6;
            this.buttonViewAll.Text = "查看全部";
            this.buttonViewAll.UseVisualStyleBackColor = true;
            this.buttonViewAll.Click += new System.EventHandler(this.buttonViewAll_Click);
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(12, 12);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(49, 19);
            this.checkBoxSelectAll.TabIndex = 7;
            this.checkBoxSelectAll.Text = "全选";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.CheckedChanged += new System.EventHandler(this.checkBoxSelectAll_CheckedChanged);
            // 
            // labelPageSize
            // 
            this.labelPageSize.AutoSize = true;
            this.labelPageSize.Location = new System.Drawing.Point(300, 410);
            this.labelPageSize.Name = "labelPageSize";
            this.labelPageSize.Size = new System.Drawing.Size(59, 15);
            this.labelPageSize.TabIndex = 8;
            this.labelPageSize.Text = "分页大小:";
            // 
            // comboBoxPageSize
            // 
            this.comboBoxPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPageSize.FormattingEnabled = true;
            this.comboBoxPageSize.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100"});
            this.comboBoxPageSize.Location = new System.Drawing.Point(365, 407);
            this.comboBoxPageSize.Name = "comboBoxPageSize";
            this.comboBoxPageSize.Size = new System.Drawing.Size(100, 23);
            this.comboBoxPageSize.TabIndex = 9;
            this.comboBoxPageSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxPageSize_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 441);
            this.Controls.Add(this.comboBoxPageSize);
            this.Controls.Add(this.labelPageSize);
            this.Controls.Add(this.checkBoxSelectAll);
            this.Controls.Add(this.buttonViewAll);
            this.Controls.Add(this.buttonConfig);
            this.Controls.Add(this.buttonConvertColors);
            this.Controls.Add(this.labelPageInfo);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonPrev);
            this.Controls.Add(this.dataGridViewSvg);
            this.Name = "MainForm";
            this.Text = "SVG管理器";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSvg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridView dataGridViewSvg;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelPageInfo;
        private System.Windows.Forms.Button buttonConvertColors;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonViewAll;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
        private System.Windows.Forms.Label labelPageSize;
        private System.Windows.Forms.ComboBox comboBoxPageSize;
    }
}