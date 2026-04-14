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
            this.components = new System.ComponentModel.Container();
            this.dataGridViewSvg = new System.Windows.Forms.DataGridView();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelPageInfo = new System.Windows.Forms.Label();
            this.buttonConvertColors = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonViewAll = new System.Windows.Forms.Button();
            this.buttonViewAttributes = new System.Windows.Forms.Button();
            this.buttonLoadLocal = new System.Windows.Forms.Button();
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.comboBoxPageSize = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.webBrowserPreview = new System.Windows.Forms.WebBrowser();
            this.labelPreview = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSvg)).BeginInit();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelTop.SuspendLayout();
            this.tableLayoutPanelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewSvg
            // 
            this.dataGridViewSvg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSvg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSvg.Location = new System.Drawing.Point(3, 34);
            this.dataGridViewSvg.Name = "dataGridViewSvg";
            this.dataGridViewSvg.RowTemplate.Height = 25;
            this.dataGridViewSvg.Size = new System.Drawing.Size(556, 366);
            this.dataGridViewSvg.TabIndex = 0;
            this.dataGridViewSvg.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSvg_CellClick);
            this.dataGridViewSvg.SelectionChanged += new System.EventHandler(this.dataGridViewSvg_SelectionChanged);
            // 
            // buttonPrev
            // 
            this.buttonPrev.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonPrev.Location = new System.Drawing.Point(3, 3);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(64, 20);
            this.buttonPrev.TabIndex = 1;
            this.buttonPrev.Text = "上一页";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonNext.Location = new System.Drawing.Point(73, 3);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(64, 20);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "下一页";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelPageInfo
            // 
            this.labelPageInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPageInfo.AutoSize = true;
            this.labelPageInfo.Location = new System.Drawing.Point(143, 5);
            this.labelPageInfo.Name = "labelPageInfo";
            this.labelPageInfo.Size = new System.Drawing.Size(89, 15);
            this.labelPageInfo.TabIndex = 3;
            this.labelPageInfo.Text = "第 1 页，共 0 页";
            // 
            // buttonConvertColors
            // 
            this.buttonConvertColors.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonConvertColors.Location = new System.Drawing.Point(263, 3);
            this.buttonConvertColors.Name = "buttonConvertColors";
            this.buttonConvertColors.Size = new System.Drawing.Size(150, 20);
            this.buttonConvertColors.TabIndex = 4;
            this.buttonConvertColors.Text = "转换颜色";
            this.buttonConvertColors.UseVisualStyleBackColor = true;
            this.buttonConvertColors.Click += new System.EventHandler(this.buttonConvertColors_Click);
            // 
            // buttonConfig
            // 
            this.buttonConfig.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonConfig.Location = new System.Drawing.Point(429, 3);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(64, 20);
            this.buttonConfig.TabIndex = 5;
            this.buttonConfig.Text = "配置";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // buttonViewAll
            // 
            this.buttonViewAll.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonViewAll.Location = new System.Drawing.Point(509, 3);
            this.buttonViewAll.Name = "buttonViewAll";
            this.buttonViewAll.Size = new System.Drawing.Size(74, 20);
            this.buttonViewAll.TabIndex = 6;
            this.buttonViewAll.Text = "查看全部";
            this.buttonViewAll.UseVisualStyleBackColor = true;
            this.buttonViewAll.Click += new System.EventHandler(this.buttonViewAll_Click);
            // 
            // buttonViewAttributes
            // 
            this.buttonViewAttributes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonViewAttributes.Location = new System.Drawing.Point(589, 3);
            this.buttonViewAttributes.Name = "buttonViewAttributes";
            this.buttonViewAttributes.Size = new System.Drawing.Size(64, 20);
            this.buttonViewAttributes.TabIndex = 7;
            this.buttonViewAttributes.Text = "属性";
            this.buttonViewAttributes.UseVisualStyleBackColor = true;
            this.buttonViewAttributes.Click += new System.EventHandler(this.buttonViewAttributes_Click);
            // 
            // buttonLoadLocal
            // 
            this.buttonLoadLocal.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonLoadLocal.Location = new System.Drawing.Point(499, 3);
            this.buttonLoadLocal.Name = "buttonLoadLocal";
            this.buttonLoadLocal.Size = new System.Drawing.Size(74, 20);
            this.buttonLoadLocal.TabIndex = 8;
            this.buttonLoadLocal.Text = "本地导入";
            this.buttonLoadLocal.UseVisualStyleBackColor = true;
            this.buttonLoadLocal.Click += new System.EventHandler(this.buttonLoadLocal_Click);
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(3, 6);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(49, 19);
            this.checkBoxSelectAll.TabIndex = 7;
            this.checkBoxSelectAll.Text = "全选";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.CheckedChanged += new System.EventHandler(this.checkBoxSelectAll_CheckedChanged);
            // 
            // labelPageSize
            // 
            this.labelPageSize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelPageSize.AutoSize = true;
            this.labelPageSize.Location = new System.Drawing.Point(263, 10);
            this.labelPageSize.Name = "labelPageSize";
            this.labelPageSize.Size = new System.Drawing.Size(59, 15);
            this.labelPageSize.TabIndex = 8;
            this.labelPageSize.Text = "分页大小:";
            // 
            // comboBoxPageSize
            // 
            this.comboBoxPageSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPageSize.FormattingEnabled = true;
            this.comboBoxPageSize.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100"});
            this.comboBoxPageSize.Location = new System.Drawing.Point(333, 6);
            this.comboBoxPageSize.Name = "comboBoxPageSize";
            this.comboBoxPageSize.Size = new System.Drawing.Size(94, 23);
            this.comboBoxPageSize.TabIndex = 9;
            this.comboBoxPageSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxPageSize_SelectedIndexChanged);
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelTop, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.dataGridViewSvg, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelBottom, 0, 2);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(562, 432);
            this.tableLayoutPanelMain.TabIndex = 10;
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.ColumnCount = 2;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Controls.Add(this.checkBoxSelectAll, 0, 0);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 1;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(556, 25);
            this.tableLayoutPanelTop.TabIndex = 11;
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.ColumnCount = 10;
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelBottom.Controls.Add(this.buttonPrev, 0, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonNext, 1, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.labelPageInfo, 2, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.labelPageSize, 3, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.comboBoxPageSize, 4, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonConvertColors, 5, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonConfig, 6, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonLoadLocal, 7, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonViewAll, 8, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonViewAttributes, 9, 0);
            this.tableLayoutPanelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(3, 403);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.RowCount = 1;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(556, 26);
            this.tableLayoutPanelBottom.TabIndex = 12;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.None;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.tableLayoutPanelMain);
            this.splitContainerMain.Panel1MinSize = 300;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.webBrowserPreview);
            this.splitContainerMain.Panel2.Controls.Add(this.labelPreview);
            this.splitContainerMain.Panel2MinSize = 200;
            this.splitContainerMain.Size = new System.Drawing.Size(924, 432);
            this.splitContainerMain.SplitterDistance = 562;
            this.splitContainerMain.TabIndex = 11;
            // 
            // webBrowserPreview
            // 
            this.webBrowserPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserPreview.Location = new System.Drawing.Point(0, 25);
            this.webBrowserPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPreview.Name = "webBrowserPreview";
            this.webBrowserPreview.Size = new System.Drawing.Size(414, 407);
            this.webBrowserPreview.TabIndex = 1;
            // 
            // labelPreview
            // 
            this.labelPreview.AutoSize = true;
            this.labelPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelPreview.Location = new System.Drawing.Point(0, 0);
            this.labelPreview.Name = "labelPreview";
            this.labelPreview.Size = new System.Drawing.Size(44, 15);
            this.labelPreview.TabIndex = 0;
            this.labelPreview.Text = "预览";
            this.labelPreview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 432);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "MainForm";
            this.Text = "SVG管理器";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSvg)).EndInit();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.tableLayoutPanelBottom.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dataGridViewSvg;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelPageInfo;
        private System.Windows.Forms.Button buttonConvertColors;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonViewAll;
        private System.Windows.Forms.Button buttonViewAttributes;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
        private System.Windows.Forms.Label labelPageSize;
        private System.Windows.Forms.ComboBox comboBoxPageSize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.WebBrowser webBrowserPreview;
        private System.Windows.Forms.Label labelPreview;
        private System.Windows.Forms.Button buttonLoadLocal;
    }
}