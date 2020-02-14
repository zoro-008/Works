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
            this.btPlay = new System.Windows.Forms.Button();
            this.btRec = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.rbSave = new System.Windows.Forms.Button();
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnTop = new System.Windows.Forms.Panel();
            this.pnCam = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.pnTop.SuspendLayout();
            this.pnCam.SuspendLayout();
            this.SuspendLayout();
            // 
            // btPlay
            // 
            this.btPlay.BackColor = System.Drawing.Color.Transparent;
            this.btPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btPlay.BackgroundImage")));
            this.btPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btPlay.Dock = System.Windows.Forms.DockStyle.Right;
            this.btPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPlay.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btPlay.Location = new System.Drawing.Point(1015, 0);
            this.btPlay.Margin = new System.Windows.Forms.Padding(1);
            this.btPlay.Name = "btPlay";
            this.btPlay.Size = new System.Drawing.Size(30, 30);
            this.btPlay.TabIndex = 19;
            this.btPlay.UseVisualStyleBackColor = false;
            this.btPlay.Click += new System.EventHandler(this.btPlay_Click);
            // 
            // btRec
            // 
            this.btRec.BackColor = System.Drawing.Color.Transparent;
            this.btRec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btRec.BackgroundImage")));
            this.btRec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btRec.Dock = System.Windows.Forms.DockStyle.Right;
            this.btRec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btRec.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btRec.Location = new System.Drawing.Point(1045, 0);
            this.btRec.Margin = new System.Windows.Forms.Padding(1);
            this.btRec.Name = "btRec";
            this.btRec.Size = new System.Drawing.Size(30, 30);
            this.btRec.TabIndex = 21;
            this.btRec.UseVisualStyleBackColor = false;
            this.btRec.Click += new System.EventHandler(this.btRec_Click);
            // 
            // btStop
            // 
            this.btStop.BackColor = System.Drawing.Color.Transparent;
            this.btStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btStop.BackgroundImage")));
            this.btStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btStop.Dock = System.Windows.Forms.DockStyle.Right;
            this.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btStop.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btStop.Location = new System.Drawing.Point(1075, 0);
            this.btStop.Margin = new System.Windows.Forms.Padding(1);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(30, 30);
            this.btStop.TabIndex = 22;
            this.btStop.UseVisualStyleBackColor = false;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // rbSave
            // 
            this.rbSave.BackColor = System.Drawing.Color.Transparent;
            this.rbSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbSave.BackgroundImage")));
            this.rbSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.rbSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbSave.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.rbSave.Location = new System.Drawing.Point(1105, 0);
            this.rbSave.Margin = new System.Windows.Forms.Padding(1);
            this.rbSave.Name = "rbSave";
            this.rbSave.Size = new System.Drawing.Size(30, 30);
            this.rbSave.TabIndex = 23;
            this.rbSave.UseVisualStyleBackColor = false;
            this.rbSave.Click += new System.EventHandler(this.rbSave_Click);
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.Black;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibCam.Location = new System.Drawing.Point(0, 30);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(1135, 542);
            this.ibCam.TabIndex = 22;
            this.ibCam.TabStop = false;
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibCam_Paint);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnTop
            // 
            this.pnTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnTop.BackgroundImage")));
            this.pnTop.Controls.Add(this.label13);
            this.pnTop.Controls.Add(this.btPlay);
            this.pnTop.Controls.Add(this.btRec);
            this.pnTop.Controls.Add(this.btStop);
            this.pnTop.Controls.Add(this.rbSave);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Margin = new System.Windows.Forms.Padding(0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1135, 30);
            this.pnTop.TabIndex = 137;
            // 
            // pnCam
            // 
            this.pnCam.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnCam.Controls.Add(this.ibCam);
            this.pnCam.Controls.Add(this.pnTop);
            this.pnCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCam.Location = new System.Drawing.Point(0, 0);
            this.pnCam.Name = "pnCam";
            this.pnCam.Size = new System.Drawing.Size(1135, 572);
            this.pnCam.TabIndex = 138;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(0, 0);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(1015, 30);
            this.label13.TabIndex = 136;
            this.label13.Text = "CAMERA";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormCam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 572);
            this.Controls.Add(this.pnCam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCam";
            this.Text = "FormCam";
            this.VisibleChanged += new System.EventHandler(this.FormCam_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.pnTop.ResumeLayout(false);
            this.pnCam.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btPlay;
        private System.Windows.Forms.Button btRec;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button rbSave;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnCam;
        private System.Windows.Forms.Label label13;
    }
}