namespace Machine
{
    partial class FormCam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCam));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bt1 = new System.Windows.Forms.ToolStripButton();
            this.bt2 = new System.Windows.Forms.ToolStripButton();
            this.bt3 = new System.Windows.Forms.ToolStripButton();
            this.bt4 = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.pnCam = new System.Windows.Forms.Panel();
            this.pictureBoxIpl1 = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.bt5 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.pnCam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIpl1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bt1,
            this.bt2,
            this.bt3,
            this.bt4,
            this.bt5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(564, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bt1
            // 
            this.bt1.AutoSize = false;
            this.bt1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bt1.Image = ((System.Drawing.Image)(resources.GetObject("bt1.Image")));
            this.bt1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt1.Name = "bt1";
            this.bt1.Size = new System.Drawing.Size(23, 25);
            this.bt1.Text = "START";
            this.bt1.Click += new System.EventHandler(this.bt1_Click);
            this.bt1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt1_MouseDown);
            // 
            // bt2
            // 
            this.bt2.AutoSize = false;
            this.bt2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bt2.Image = ((System.Drawing.Image)(resources.GetObject("bt2.Image")));
            this.bt2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt2.Name = "bt2";
            this.bt2.Size = new System.Drawing.Size(23, 25);
            this.bt2.Text = "STOP";
            this.bt2.Click += new System.EventHandler(this.bt2_Click);
            this.bt2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt2_MouseDown);
            // 
            // bt3
            // 
            this.bt3.AutoSize = false;
            this.bt3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bt3.Image = ((System.Drawing.Image)(resources.GetObject("bt3.Image")));
            this.bt3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt3.Name = "bt3";
            this.bt3.Size = new System.Drawing.Size(23, 25);
            this.bt3.Text = "REC";
            this.bt3.Click += new System.EventHandler(this.bt3_Click);
            this.bt3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt3_MouseDown);
            // 
            // bt4
            // 
            this.bt4.AutoSize = false;
            this.bt4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bt4.Image = ((System.Drawing.Image)(resources.GetObject("bt4.Image")));
            this.bt4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt4.Name = "bt4";
            this.bt4.Size = new System.Drawing.Size(23, 25);
            this.bt4.Text = "CAPTURE";
            this.bt4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt4_MouseDown);
            // 
            // timer1
            // 
            this.timer1.Interval = 33;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 45;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // pnCam
            // 
            this.pnCam.Controls.Add(this.pictureBoxIpl1);
            this.pnCam.Controls.Add(this.toolStrip1);
            this.pnCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCam.Location = new System.Drawing.Point(0, 0);
            this.pnCam.Name = "pnCam";
            this.pnCam.Size = new System.Drawing.Size(564, 474);
            this.pnCam.TabIndex = 2;
            // 
            // pictureBoxIpl1
            // 
            this.pictureBoxIpl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxIpl1.Location = new System.Drawing.Point(0, 28);
            this.pictureBoxIpl1.Name = "pictureBoxIpl1";
            this.pictureBoxIpl1.Size = new System.Drawing.Size(564, 446);
            this.pictureBoxIpl1.TabIndex = 0;
            this.pictureBoxIpl1.TabStop = false;
            this.pictureBoxIpl1.Click += new System.EventHandler(this.pictureBoxIpl1_Click);
            // 
            // bt5
            // 
            this.bt5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bt5.Image = ((System.Drawing.Image)(resources.GetObject("bt5.Image")));
            this.bt5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt5.Name = "bt5";
            this.bt5.Size = new System.Drawing.Size(23, 25);
            this.bt5.Text = "RESET";
            this.bt5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton1_MouseDown);
            // 
            // FormCam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(564, 474);
            this.Controls.Add(this.pnCam);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCam";
            this.Text = "FormCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCam_FormClosing);
            this.Load += new System.EventHandler(this.FormCam_Load);
            this.VisibleChanged += new System.EventHandler(this.FormCam_VisibleChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnCam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIpl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bt1;
        private System.Windows.Forms.ToolStripButton bt2;
        private System.Windows.Forms.ToolStripButton bt3;
        private System.Windows.Forms.ToolStripButton bt4;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Panel pnCam;
        private OpenCvSharp.UserInterface.PictureBoxIpl pictureBoxIpl1;
        private System.Windows.Forms.ToolStripButton bt5;
    }
}