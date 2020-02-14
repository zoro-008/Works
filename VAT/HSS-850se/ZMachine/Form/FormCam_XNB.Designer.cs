namespace Machine
{
    partial class FormCam_XNB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCam_XNB));
            this.panel1 = new System.Windows.Forms.Panel();
            this.sdkCamera = new AxXNSSDKDEVICELib.AxXnsSdkDevice();
            this.sdkWindow = new AxXNSSDKWINDOWLib.AxXnsSdkWindow();
            this.pnTitle = new System.Windows.Forms.FlowLayoutPanel();
            this.btPlay = new System.Windows.Forms.Button();
            this.btRec = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.rbSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.btOri = new System.Windows.Forms.Button();
            this.btThr = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.btSetting = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnTitle1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbArcCnt = new System.Windows.Forms.Label();
            this.btInsp = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sdkCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdkWindow)).BeginInit();
            this.pnTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.pnTitle1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.sdkCamera);
            this.panel1.Controls.Add(this.sdkWindow);
            this.panel1.Location = new System.Drawing.Point(3, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(29, 28);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // sdkCamera
            // 
            this.sdkCamera.Enabled = true;
            this.sdkCamera.Location = new System.Drawing.Point(3, 3);
            this.sdkCamera.Name = "sdkCamera";
            this.sdkCamera.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("sdkCamera.OcxState")));
            this.sdkCamera.Size = new System.Drawing.Size(19, 20);
            this.sdkCamera.TabIndex = 15;
            this.sdkCamera.OnDeviceStatusChanged += new AxXNSSDKDEVICELib._DXnsSdkDeviceEvents_OnDeviceStatusChangedEventHandler(this.sdkCamera_OnDeviceStatusChanged);
            // 
            // sdkWindow
            // 
            this.sdkWindow.Enabled = true;
            this.sdkWindow.Location = new System.Drawing.Point(0, 0);
            this.sdkWindow.Name = "sdkWindow";
            this.sdkWindow.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("sdkWindow.OcxState")));
            this.sdkWindow.Size = new System.Drawing.Size(22, 22);
            this.sdkWindow.TabIndex = 14;
            this.sdkWindow.OnVideoRawDataReceived += new AxXNSSDKWINDOWLib._DXnsSdkWindowEvents_OnVideoRawDataReceivedEventHandler(this.sdkWindow_OnVideoRawDataReceived);
            // 
            // pnTitle
            // 
            this.pnTitle.BackColor = System.Drawing.SystemColors.Control;
            this.pnTitle.Controls.Add(this.btPlay);
            this.pnTitle.Controls.Add(this.btRec);
            this.pnTitle.Controls.Add(this.btStop);
            this.pnTitle.Controls.Add(this.rbSave);
            this.pnTitle.Controls.Add(this.btLoad);
            this.pnTitle.Controls.Add(this.btOri);
            this.pnTitle.Controls.Add(this.btThr);
            this.pnTitle.Controls.Add(this.numericUpDown1);
            this.pnTitle.Controls.Add(this.btSetting);
            this.pnTitle.Controls.Add(this.lbCount);
            this.pnTitle.Controls.Add(this.panel1);
            this.pnTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTitle.Location = new System.Drawing.Point(0, 0);
            this.pnTitle.Name = "pnTitle";
            this.pnTitle.Size = new System.Drawing.Size(371, 31);
            this.pnTitle.TabIndex = 20;
            // 
            // btPlay
            // 
            this.btPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btPlay.BackgroundImage")));
            this.btPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPlay.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btPlay.Location = new System.Drawing.Point(1, 1);
            this.btPlay.Margin = new System.Windows.Forms.Padding(1);
            this.btPlay.Name = "btPlay";
            this.btPlay.Size = new System.Drawing.Size(27, 27);
            this.btPlay.TabIndex = 18;
            this.btPlay.UseVisualStyleBackColor = true;
            this.btPlay.Click += new System.EventHandler(this.btPlay_Click);
            // 
            // btRec
            // 
            this.btRec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btRec.BackgroundImage")));
            this.btRec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btRec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btRec.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btRec.Location = new System.Drawing.Point(30, 1);
            this.btRec.Margin = new System.Windows.Forms.Padding(1);
            this.btRec.Name = "btRec";
            this.btRec.Size = new System.Drawing.Size(27, 27);
            this.btRec.TabIndex = 19;
            this.btRec.UseVisualStyleBackColor = true;
            this.btRec.Click += new System.EventHandler(this.btRec_Click);
            // 
            // btStop
            // 
            this.btStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btStop.BackgroundImage")));
            this.btStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btStop.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btStop.Location = new System.Drawing.Point(59, 1);
            this.btStop.Margin = new System.Windows.Forms.Padding(1);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(27, 27);
            this.btStop.TabIndex = 20;
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // rbSave
            // 
            this.rbSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbSave.BackgroundImage")));
            this.rbSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbSave.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.rbSave.Location = new System.Drawing.Point(88, 1);
            this.rbSave.Margin = new System.Windows.Forms.Padding(1);
            this.rbSave.Name = "rbSave";
            this.rbSave.Size = new System.Drawing.Size(27, 27);
            this.rbSave.TabIndex = 12;
            this.rbSave.UseVisualStyleBackColor = true;
            this.rbSave.Click += new System.EventHandler(this.rbSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btLoad.BackgroundImage")));
            this.btLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btLoad.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btLoad.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btLoad.Location = new System.Drawing.Point(117, 1);
            this.btLoad.Margin = new System.Windows.Forms.Padding(1);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(27, 27);
            this.btLoad.TabIndex = 22;
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // btOri
            // 
            this.btOri.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btOri.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btOri.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btOri.Location = new System.Drawing.Point(146, 1);
            this.btOri.Margin = new System.Windows.Forms.Padding(1);
            this.btOri.Name = "btOri";
            this.btOri.Size = new System.Drawing.Size(27, 27);
            this.btOri.TabIndex = 22;
            this.btOri.Text = "O";
            this.btOri.UseVisualStyleBackColor = true;
            this.btOri.Click += new System.EventHandler(this.btOri_Click);
            // 
            // btThr
            // 
            this.btThr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btThr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btThr.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btThr.Location = new System.Drawing.Point(175, 1);
            this.btThr.Margin = new System.Windows.Forms.Padding(1);
            this.btThr.Name = "btThr";
            this.btThr.Size = new System.Drawing.Size(27, 27);
            this.btThr.TabIndex = 23;
            this.btThr.Text = "T";
            this.btThr.UseVisualStyleBackColor = true;
            this.btThr.Click += new System.EventHandler(this.btThr_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.AutoSize = true;
            this.numericUpDown1.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.numericUpDown1.Location = new System.Drawing.Point(206, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(53, 29);
            this.numericUpDown1.TabIndex = 25;
            this.numericUpDown1.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numericUpDown1.Visible = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // btSetting
            // 
            this.btSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSetting.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btSetting.Location = new System.Drawing.Point(263, 1);
            this.btSetting.Margin = new System.Windows.Forms.Padding(1);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(27, 27);
            this.btSetting.TabIndex = 30;
            this.btSetting.Text = "S";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // lbCount
            // 
            this.lbCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("Arial", 12F);
            this.lbCount.Location = new System.Drawing.Point(294, 8);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(44, 18);
            this.lbCount.TabIndex = 29;
            this.lbCount.Text = "1234";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown2.AutoSize = true;
            this.numericUpDown2.Font = new System.Drawing.Font("Arial", 12F);
            this.numericUpDown2.Location = new System.Drawing.Point(40, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(51, 26);
            this.numericUpDown2.TabIndex = 27;
            this.numericUpDown2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown3.AutoSize = true;
            this.numericUpDown3.Font = new System.Drawing.Font("Arial", 12F);
            this.numericUpDown3.Location = new System.Drawing.Point(128, 3);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(51, 26);
            this.numericUpDown3.TabIndex = 28;
            this.numericUpDown3.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.Black;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibCam.Location = new System.Drawing.Point(0, 59);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(371, 261);
            this.ibCam.TabIndex = 21;
            this.ibCam.TabStop = false;
            this.ibCam.SizeChanged += new System.EventHandler(this.ibCam_SizeChanged);
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibCam_Paint);
            this.ibCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseDown);
            this.ibCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseMove);
            this.ibCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnTitle1
            // 
            this.pnTitle1.BackColor = System.Drawing.SystemColors.Control;
            this.pnTitle1.Controls.Add(this.label2);
            this.pnTitle1.Controls.Add(this.numericUpDown2);
            this.pnTitle1.Controls.Add(this.label3);
            this.pnTitle1.Controls.Add(this.numericUpDown3);
            this.pnTitle1.Controls.Add(this.lbArcCnt);
            this.pnTitle1.Controls.Add(this.btInsp);
            this.pnTitle1.Controls.Add(this.btSave);
            this.pnTitle1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTitle1.Location = new System.Drawing.Point(0, 31);
            this.pnTitle1.Name = "pnTitle1";
            this.pnTitle1.Size = new System.Drawing.Size(371, 28);
            this.pnTitle1.TabIndex = 22;
            this.pnTitle1.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F);
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 16);
            this.label2.TabIndex = 31;
            this.label2.Text = "MIN";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F);
            this.label3.Location = new System.Drawing.Point(97, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 16);
            this.label3.TabIndex = 32;
            this.label3.Text = "Th";
            // 
            // lbArcCnt
            // 
            this.lbArcCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArcCnt.AutoSize = true;
            this.lbArcCnt.Font = new System.Drawing.Font("Arial", 12F);
            this.lbArcCnt.Location = new System.Drawing.Point(185, 7);
            this.lbArcCnt.Name = "lbArcCnt";
            this.lbArcCnt.Size = new System.Drawing.Size(35, 18);
            this.lbArcCnt.TabIndex = 33;
            this.lbArcCnt.Text = "123";
            // 
            // btInsp
            // 
            this.btInsp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btInsp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btInsp.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btInsp.Location = new System.Drawing.Point(224, 1);
            this.btInsp.Margin = new System.Windows.Forms.Padding(1);
            this.btInsp.Name = "btInsp";
            this.btInsp.Size = new System.Drawing.Size(50, 27);
            this.btInsp.TabIndex = 34;
            this.btInsp.Text = "INSP";
            this.btInsp.UseVisualStyleBackColor = true;
            this.btInsp.Click += new System.EventHandler(this.btInsp_Click);
            // 
            // btSave
            // 
            this.btSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSave.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btSave.Location = new System.Drawing.Point(276, 1);
            this.btSave.Margin = new System.Windows.Forms.Padding(1);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(50, 27);
            this.btSave.TabIndex = 36;
            this.btSave.Text = "SAVE";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // FormCam_XNB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 320);
            this.Controls.Add(this.ibCam);
            this.Controls.Add(this.pnTitle1);
            this.Controls.Add(this.pnTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCam_XNB";
            this.Text = "FormCam_XNB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCam_XNB_FormClosing);
            this.Shown += new System.EventHandler(this.FormCam_XNB_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sdkCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdkWindow)).EndInit();
            this.pnTitle.ResumeLayout(false);
            this.pnTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.pnTitle1.ResumeLayout(false);
            this.pnTitle1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private AxXNSSDKDEVICELib.AxXnsSdkDevice sdkCamera;
        private AxXNSSDKWINDOWLib.AxXnsSdkWindow sdkWindow;
        private System.Windows.Forms.FlowLayoutPanel pnTitle;
        private System.Windows.Forms.Button rbSave;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.Button btPlay;
        private System.Windows.Forms.Button btRec;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btOri;
        private System.Windows.Forms.Button btThr;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.FlowLayoutPanel pnTitle1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Label lbArcCnt;
        private System.Windows.Forms.Button btInsp;
        private System.Windows.Forms.Button btSave;
    }
}