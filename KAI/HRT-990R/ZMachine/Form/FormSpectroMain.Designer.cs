namespace Machine
{
    partial class FormSpectroMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSpectroMain));
            this.pnBase = new System.Windows.Forms.Panel();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbStat = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnMainTitle.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.Color.MistyRose;
            this.pnBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBase.Location = new System.Drawing.Point(0, 105);
            this.pnBase.Name = "pnBase";
            this.pnBase.Size = new System.Drawing.Size(1280, 919);
            this.pnBase.TabIndex = 40;
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnMainTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnMainTitle.BackgroundImage")));
            this.pnMainTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMainTitle.Controls.Add(this.panel2);
            this.pnMainTitle.Controls.Add(this.panel1);
            this.pnMainTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMainTitle.Location = new System.Drawing.Point(0, 0);
            this.pnMainTitle.Name = "pnMainTitle";
            this.pnMainTitle.Size = new System.Drawing.Size(1280, 105);
            this.pnMainTitle.TabIndex = 39;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.lbStat);
            this.panel2.Location = new System.Drawing.Point(412, 11);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(455, 78);
            this.panel2.TabIndex = 50;
            // 
            // lbStat
            // 
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            this.lbStat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStat.Font = new System.Drawing.Font("맑은 고딕", 40F, System.Drawing.FontStyle.Bold);
            this.lbStat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbStat.Location = new System.Drawing.Point(0, 0);
            this.lbStat.Name = "lbStat";
            this.lbStat.Size = new System.Drawing.Size(455, 78);
            this.lbStat.TabIndex = 46;
            this.lbStat.Text = "SPECTROMETER";
            this.lbStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(102, 95);
            this.panel1.TabIndex = 56;
            // 
            // FormSpectroMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 1024);
            this.Controls.Add(this.pnBase);
            this.Controls.Add(this.pnMainTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSpectroMain";
            this.Text = "FormSpectroMain";
            this.VisibleChanged += new System.EventHandler(this.FormSpectroMain_VisibleChanged);
            this.pnMainTitle.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnBase;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Panel panel1;
    }
}