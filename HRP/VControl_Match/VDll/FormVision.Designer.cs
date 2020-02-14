namespace VDll
{
    partial class FormVision
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
            this.pnTitle = new System.Windows.Forms.Panel();
            this.btProfile = new System.Windows.Forms.Button();
            this.btInsp = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.cbGrabInsp = new System.Windows.Forms.CheckBox();
            this.lbScaleOffset = new System.Windows.Forms.Label();
            this.btGrabInsp = new System.Windows.Forms.Button();
            this.btTrain = new System.Windows.Forms.Button();
            this.lbZoomScale = new System.Windows.Forms.Label();
            this.lbCnt = new System.Windows.Forms.Label();
            this.lbGrabTime = new System.Windows.Forms.Label();
            this.cbLive = new System.Windows.Forms.CheckBox();
            this.btGrab = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.pnSub = new System.Windows.Forms.Panel();
            this.btFitSize = new System.Windows.Forms.Button();
            this.pnColor = new System.Windows.Forms.Panel();
            this.lbPixel = new System.Windows.Forms.Label();
            this.btMap = new System.Windows.Forms.Button();
            this.btMsg = new System.Windows.Forms.Button();
            this.tbMsg = new System.Windows.Forms.TextBox();
            this.btRealSize = new System.Windows.Forms.Button();
            this.btZoomIn = new System.Windows.Forms.Button();
            this.btZoomOut = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmLive = new System.Windows.Forms.Timer(this.components);
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.tmGrabInsp = new System.Windows.Forms.Timer(this.components);
            this.pnTitle.SuspendLayout();
            this.pnSub.SuspendLayout();
            this.pnColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTitle
            // 
            this.pnTitle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnTitle.Controls.Add(this.btProfile);
            this.pnTitle.Controls.Add(this.btInsp);
            this.pnTitle.Controls.Add(this.btSave);
            this.pnTitle.Controls.Add(this.cbGrabInsp);
            this.pnTitle.Controls.Add(this.lbScaleOffset);
            this.pnTitle.Controls.Add(this.btGrabInsp);
            this.pnTitle.Controls.Add(this.btTrain);
            this.pnTitle.Controls.Add(this.lbZoomScale);
            this.pnTitle.Controls.Add(this.lbCnt);
            this.pnTitle.Controls.Add(this.lbGrabTime);
            this.pnTitle.Controls.Add(this.cbLive);
            this.pnTitle.Controls.Add(this.btGrab);
            this.pnTitle.Controls.Add(this.btLoad);
            this.pnTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTitle.Location = new System.Drawing.Point(0, 0);
            this.pnTitle.Name = "pnTitle";
            this.pnTitle.Size = new System.Drawing.Size(1068, 28);
            this.pnTitle.TabIndex = 0;
            // 
            // btProfile
            // 
            this.btProfile.Location = new System.Drawing.Point(439, 3);
            this.btProfile.Name = "btProfile";
            this.btProfile.Size = new System.Drawing.Size(53, 23);
            this.btProfile.TabIndex = 13;
            this.btProfile.Text = "Profile";
            this.btProfile.UseVisualStyleBackColor = true;
            this.btProfile.Click += new System.EventHandler(this.btProfile_Click);
            // 
            // btInsp
            // 
            this.btInsp.Location = new System.Drawing.Point(380, 3);
            this.btInsp.Name = "btInsp";
            this.btInsp.Size = new System.Drawing.Size(53, 23);
            this.btInsp.TabIndex = 12;
            this.btInsp.Text = "Insp";
            this.btInsp.UseVisualStyleBackColor = true;
            this.btInsp.Click += new System.EventHandler(this.btInsp_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(62, 2);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(53, 23);
            this.btSave.TabIndex = 11;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // cbGrabInsp
            // 
            this.cbGrabInsp.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbGrabInsp.AutoSize = true;
            this.cbGrabInsp.Location = new System.Drawing.Point(165, 4);
            this.cbGrabInsp.Name = "cbGrabInsp";
            this.cbGrabInsp.Size = new System.Drawing.Size(27, 22);
            this.cbGrabInsp.TabIndex = 10;
            this.cbGrabInsp.Text = "GI";
            this.cbGrabInsp.UseVisualStyleBackColor = true;
            this.cbGrabInsp.CheckedChanged += new System.EventHandler(this.cbGrabInsp_CheckedChanged);
            // 
            // lbScaleOffset
            // 
            this.lbScaleOffset.AutoSize = true;
            this.lbScaleOffset.Location = new System.Drawing.Point(704, 8);
            this.lbScaleOffset.Name = "lbScaleOffset";
            this.lbScaleOffset.Size = new System.Drawing.Size(38, 12);
            this.lbScaleOffset.TabIndex = 9;
            this.lbScaleOffset.Text = "label1";
            // 
            // btGrabInsp
            // 
            this.btGrabInsp.Location = new System.Drawing.Point(268, 6);
            this.btGrabInsp.Name = "btGrabInsp";
            this.btGrabInsp.Size = new System.Drawing.Size(47, 19);
            this.btGrabInsp.TabIndex = 8;
            this.btGrabInsp.Text = "GI";
            this.btGrabInsp.UseVisualStyleBackColor = true;
            this.btGrabInsp.Click += new System.EventHandler(this.btGrabInsp_Click);
            // 
            // btTrain
            // 
            this.btTrain.Location = new System.Drawing.Point(576, 4);
            this.btTrain.Name = "btTrain";
            this.btTrain.Size = new System.Drawing.Size(45, 23);
            this.btTrain.TabIndex = 3;
            this.btTrain.Text = "Train";
            this.btTrain.UseVisualStyleBackColor = true;
            this.btTrain.Click += new System.EventHandler(this.btTrain_Click);
            // 
            // lbZoomScale
            // 
            this.lbZoomScale.AutoSize = true;
            this.lbZoomScale.Location = new System.Drawing.Point(820, 6);
            this.lbZoomScale.Name = "lbZoomScale";
            this.lbZoomScale.Size = new System.Drawing.Size(38, 12);
            this.lbZoomScale.TabIndex = 7;
            this.lbZoomScale.Text = "label1";
            // 
            // lbCnt
            // 
            this.lbCnt.AutoSize = true;
            this.lbCnt.Location = new System.Drawing.Point(879, 7);
            this.lbCnt.Name = "lbCnt";
            this.lbCnt.Size = new System.Drawing.Size(34, 12);
            this.lbCnt.TabIndex = 6;
            this.lbCnt.Text = "lbCnt";
            // 
            // lbGrabTime
            // 
            this.lbGrabTime.AutoSize = true;
            this.lbGrabTime.Location = new System.Drawing.Point(629, 9);
            this.lbGrabTime.Name = "lbGrabTime";
            this.lbGrabTime.Size = new System.Drawing.Size(38, 12);
            this.lbGrabTime.TabIndex = 4;
            this.lbGrabTime.Text = "label1";
            // 
            // cbLive
            // 
            this.cbLive.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLive.AutoSize = true;
            this.cbLive.Location = new System.Drawing.Point(121, 3);
            this.cbLive.Name = "cbLive";
            this.cbLive.Size = new System.Drawing.Size(38, 22);
            this.cbLive.TabIndex = 3;
            this.cbLive.Text = "Live";
            this.cbLive.UseVisualStyleBackColor = true;
            this.cbLive.CheckedChanged += new System.EventHandler(this.cbLive_CheckedChanged);
            // 
            // btGrab
            // 
            this.btGrab.Location = new System.Drawing.Point(321, 4);
            this.btGrab.Name = "btGrab";
            this.btGrab.Size = new System.Drawing.Size(53, 23);
            this.btGrab.TabIndex = 1;
            this.btGrab.Text = "Grab";
            this.btGrab.UseVisualStyleBackColor = true;
            this.btGrab.Click += new System.EventHandler(this.btGrab_Click);
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(3, 2);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(53, 23);
            this.btLoad.TabIndex = 0;
            this.btLoad.Text = "Load";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // pnSub
            // 
            this.pnSub.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnSub.Controls.Add(this.btFitSize);
            this.pnSub.Controls.Add(this.pnColor);
            this.pnSub.Controls.Add(this.btMap);
            this.pnSub.Controls.Add(this.btMsg);
            this.pnSub.Controls.Add(this.tbMsg);
            this.pnSub.Controls.Add(this.btRealSize);
            this.pnSub.Controls.Add(this.btZoomIn);
            this.pnSub.Controls.Add(this.btZoomOut);
            this.pnSub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnSub.Location = new System.Drawing.Point(0, 431);
            this.pnSub.Name = "pnSub";
            this.pnSub.Size = new System.Drawing.Size(1068, 28);
            this.pnSub.TabIndex = 2;
            // 
            // btFitSize
            // 
            this.btFitSize.Location = new System.Drawing.Point(208, 3);
            this.btFitSize.Name = "btFitSize";
            this.btFitSize.Size = new System.Drawing.Size(53, 23);
            this.btFitSize.TabIndex = 18;
            this.btFitSize.Text = "Fit";
            this.btFitSize.UseVisualStyleBackColor = true;
            this.btFitSize.Click += new System.EventHandler(this.btFitSize_Click);
            // 
            // pnColor
            // 
            this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnColor.Controls.Add(this.lbPixel);
            this.pnColor.Location = new System.Drawing.Point(717, 4);
            this.pnColor.Name = "pnColor";
            this.pnColor.Size = new System.Drawing.Size(250, 21);
            this.pnColor.TabIndex = 17;
            // 
            // lbPixel
            // 
            this.lbPixel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPixel.Location = new System.Drawing.Point(0, 0);
            this.lbPixel.Name = "lbPixel";
            this.lbPixel.Size = new System.Drawing.Size(248, 19);
            this.lbPixel.TabIndex = 0;
            this.lbPixel.Text = "label1";
            this.lbPixel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btMap
            // 
            this.btMap.Location = new System.Drawing.Point(658, 3);
            this.btMap.Name = "btMap";
            this.btMap.Size = new System.Drawing.Size(53, 22);
            this.btMap.TabIndex = 15;
            this.btMap.Text = "Map";
            this.btMap.UseVisualStyleBackColor = true;
            this.btMap.Click += new System.EventHandler(this.btMap_Click);
            // 
            // btMsg
            // 
            this.btMsg.Location = new System.Drawing.Point(572, 2);
            this.btMsg.Name = "btMsg";
            this.btMsg.Size = new System.Drawing.Size(75, 23);
            this.btMsg.TabIndex = 13;
            this.btMsg.Text = "button1";
            this.btMsg.UseVisualStyleBackColor = true;
            this.btMsg.Click += new System.EventHandler(this.btMsg_Click);
            // 
            // tbMsg
            // 
            this.tbMsg.Location = new System.Drawing.Point(445, 3);
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.Size = new System.Drawing.Size(125, 21);
            this.tbMsg.TabIndex = 12;
            // 
            // btRealSize
            // 
            this.btRealSize.Location = new System.Drawing.Point(149, 3);
            this.btRealSize.Name = "btRealSize";
            this.btRealSize.Size = new System.Drawing.Size(53, 23);
            this.btRealSize.TabIndex = 3;
            this.btRealSize.Text = "Real";
            this.btRealSize.UseVisualStyleBackColor = true;
            this.btRealSize.Click += new System.EventHandler(this.btRealSize_Click);
            // 
            // btZoomIn
            // 
            this.btZoomIn.Location = new System.Drawing.Point(90, 3);
            this.btZoomIn.Name = "btZoomIn";
            this.btZoomIn.Size = new System.Drawing.Size(53, 23);
            this.btZoomIn.TabIndex = 2;
            this.btZoomIn.Text = "+";
            this.btZoomIn.UseVisualStyleBackColor = true;
            this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
            // 
            // btZoomOut
            // 
            this.btZoomOut.Location = new System.Drawing.Point(31, 3);
            this.btZoomOut.Name = "btZoomOut";
            this.btZoomOut.Size = new System.Drawing.Size(53, 23);
            this.btZoomOut.TabIndex = 1;
            this.btZoomOut.Text = "-";
            this.btZoomOut.UseVisualStyleBackColor = true;
            this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // tmLive
            // 
            this.tmLive.Interval = 1;
            this.tmLive.Tick += new System.EventHandler(this.tmLive_Tick);
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.DimGray;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.PanAndZoom;
            this.ibCam.Location = new System.Drawing.Point(0, 28);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(1068, 403);
            this.ibCam.TabIndex = 3;
            this.ibCam.TabStop = false;
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibCam_Paint);
            this.ibCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseDown);
            this.ibCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseMove);
            this.ibCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseUp);
            this.ibCam.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ibCam_PreviewKeyDown);
            // 
            // tmGrabInsp
            // 
            this.tmGrabInsp.Interval = 1;
            this.tmGrabInsp.Tick += new System.EventHandler(this.tmGrabInsp_Tick);
            // 
            // FormVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 459);
            this.Controls.Add(this.ibCam);
            this.Controls.Add(this.pnSub);
            this.Controls.Add(this.pnTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormVision";
            this.Text = "FormVision";
            this.Shown += new System.EventHandler(this.FormVision_Shown);
            this.Resize += new System.EventHandler(this.FormVision_Resize);
            this.pnTitle.ResumeLayout(false);
            this.pnTitle.PerformLayout();
            this.pnSub.ResumeLayout(false);
            this.pnSub.PerformLayout();
            this.pnColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTitle;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Panel pnSub;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Button btGrab;
        private System.Windows.Forms.CheckBox cbLive;
        private System.Windows.Forms.Label lbGrabTime;
        private System.Windows.Forms.Label lbCnt;
        private System.Windows.Forms.Label lbZoomScale;
        private System.Windows.Forms.Button btTrain;
        private System.Windows.Forms.Button btRealSize;
        private System.Windows.Forms.Button btZoomIn;
        private System.Windows.Forms.Button btZoomOut;
        private System.Windows.Forms.Timer tmLive;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.Button btGrabInsp;
        private System.Windows.Forms.Label lbScaleOffset;
        private System.Windows.Forms.Button btMsg;
        private System.Windows.Forms.TextBox tbMsg;
        private System.Windows.Forms.CheckBox cbGrabInsp;
        private System.Windows.Forms.Timer tmGrabInsp;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btMap;
        private System.Windows.Forms.Button btInsp;
        private System.Windows.Forms.Panel pnColor;
        private System.Windows.Forms.Label lbPixel;
        private System.Windows.Forms.Button btProfile;
        private System.Windows.Forms.Button btFitSize;
    }
}