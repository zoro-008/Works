namespace Machine
{
    partial class FraOutput
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
            this.components = new System.ComponentModel.Container();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btAction = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.Location = new System.Drawing.Point(0, 0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(103, 40);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "CYLINDER NAME";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btAction
            // 
            this.btAction.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btAction.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btAction.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btAction.Location = new System.Drawing.Point(0, 40);
            this.btAction.Margin = new System.Windows.Forms.Padding(0);
            this.btAction.Name = "btAction";
            this.btAction.Size = new System.Drawing.Size(103, 85);
            this.btAction.TabIndex = 5;
            this.btAction.Text = "LEFT";
            this.btAction.UseVisualStyleBackColor = true;
            this.btAction.Click += new System.EventHandler(this.btAction_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FraOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(103, 127);
            this.Controls.Add(this.btAction);
            this.Controls.Add(this.lbTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FraOutput";
            this.Text = "FraOutput";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FraOutput_FormClosing);
            this.Shown += new System.EventHandler(this.FraOutput_Shown);
            this.VisibleChanged += new System.EventHandler(this.FraOutput_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btAction;
        private System.Windows.Forms.Timer tmUpdate;
    }
}