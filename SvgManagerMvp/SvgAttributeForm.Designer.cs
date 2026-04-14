namespace SvgManagerMvp
{
    partial class SvgAttributeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelSvgName = new System.Windows.Forms.Label();
            this.listViewAttributes = new System.Windows.Forms.ListView();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelSvgName
            // 
            this.labelSvgName.AutoSize = true;
            this.labelSvgName.Location = new System.Drawing.Point(12, 9);
            this.labelSvgName.Name = "labelSvgName";
            this.labelSvgName.Size = new System.Drawing.Size(35, 13);
            this.labelSvgName.TabIndex = 0;
            this.labelSvgName.Text = "Name";
            // 
            // listViewAttributes
            // 
            this.listViewAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAttributes.Location = new System.Drawing.Point(12, 35);
            this.listViewAttributes.Name = "listViewAttributes";
            this.listViewAttributes.Size = new System.Drawing.Size(426, 300);
            this.listViewAttributes.TabIndex = 1;
            this.listViewAttributes.UseCompatibleStateImageBehavior = false;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(363, 341);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "关闭";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // SvgAttributeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 376);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewAttributes);
            this.Controls.Add(this.labelSvgName);
            this.Name = "SvgAttributeForm";
            this.Text = "SVG 属性查看";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSvgName;
        private System.Windows.Forms.ListView listViewAttributes;
        private System.Windows.Forms.Button buttonClose;
    }
}