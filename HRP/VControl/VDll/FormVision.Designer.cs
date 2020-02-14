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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVision));
            this.lbScaleOffset = new System.Windows.Forms.Label();
            this.lbZoomScale = new System.Windows.Forms.Label();
            this.lbCnt = new System.Windows.Forms.Label();
            this.lbGrabTime = new System.Windows.Forms.Label();
            this.pnSub = new System.Windows.Forms.Panel();
            this.pnTest = new System.Windows.Forms.FlowLayoutPanel();
            this.tbMsg = new System.Windows.Forms.TextBox();
            this.btMsg = new System.Windows.Forms.Button();
            this.CAL = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmLive = new System.Windows.Forms.Timer(this.components);
            this.tmGrabInsp = new System.Windows.Forms.Timer(this.components);
            this.paCamTop = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbStratch = new System.Windows.Forms.RadioButton();
            this.rbRealRatio = new System.Windows.Forms.RadioButton();
            this.btZoomIn = new System.Windows.Forms.Button();
            this.btZoomOut = new System.Windows.Forms.Button();
            this.btLoadImg = new System.Windows.Forms.Button();
            this.btSaveImg = new System.Windows.Forms.Button();
            this.btSetting = new System.Windows.Forms.Button();
            this.cbLiveCam = new System.Windows.Forms.CheckBox();
            this.btGrabCam = new System.Windows.Forms.Button();
            this.btMap3D = new System.Windows.Forms.Button();
            this.btProfile = new System.Windows.Forms.Button();
            this.btGrabInspCam = new System.Windows.Forms.Button();
            this.cbGrabInspCam = new System.Windows.Forms.CheckBox();
            this.btInsp = new System.Windows.Forms.Button();
            this.btRecipe = new System.Windows.Forms.Button();
            this.pnColor = new System.Windows.Forms.Panel();
            this.lbPixel = new System.Windows.Forms.Label();
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.rbFit = new System.Windows.Forms.RadioButton();
            this.pnSub.SuspendLayout();
            this.pnTest.SuspendLayout();
            this.paCamTop.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.SuspendLayout();
            // 
            // lbScaleOffset
            // 
            this.lbScaleOffset.AutoSize = true;
            this.lbScaleOffset.Location = new System.Drawing.Point(259, 8);
            this.lbScaleOffset.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lbScaleOffset.Name = "lbScaleOffset";
            this.lbScaleOffset.Size = new System.Drawing.Size(38, 12);
            this.lbScaleOffset.TabIndex = 9;
            this.lbScaleOffset.Text = "label1";
            // 
            // lbZoomScale
            // 
            this.lbZoomScale.AutoSize = true;
            this.lbZoomScale.Location = new System.Drawing.Point(303, 8);
            this.lbZoomScale.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lbZoomScale.Name = "lbZoomScale";
            this.lbZoomScale.Size = new System.Drawing.Size(38, 12);
            this.lbZoomScale.TabIndex = 7;
            this.lbZoomScale.Text = "label1";
            // 
            // lbCnt
            // 
            this.lbCnt.AutoSize = true;
            this.lbCnt.Location = new System.Drawing.Point(347, 8);
            this.lbCnt.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lbCnt.Name = "lbCnt";
            this.lbCnt.Size = new System.Drawing.Size(34, 12);
            this.lbCnt.TabIndex = 6;
            this.lbCnt.Text = "lbCnt";
            // 
            // lbGrabTime
            // 
            this.lbGrabTime.AutoSize = true;
            this.lbGrabTime.Location = new System.Drawing.Point(215, 8);
            this.lbGrabTime.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lbGrabTime.Name = "lbGrabTime";
            this.lbGrabTime.Size = new System.Drawing.Size(38, 12);
            this.lbGrabTime.TabIndex = 4;
            this.lbGrabTime.Text = "label1";
            // 
            // pnSub
            // 
            this.pnSub.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnSub.Controls.Add(this.pnTest);
            this.pnSub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnSub.Location = new System.Drawing.Point(0, 597);
            this.pnSub.Name = "pnSub";
            this.pnSub.Size = new System.Drawing.Size(951, 28);
            this.pnSub.TabIndex = 2;
            // 
            // pnTest
            // 
            this.pnTest.Controls.Add(this.tbMsg);
            this.pnTest.Controls.Add(this.btMsg);
            this.pnTest.Controls.Add(this.lbGrabTime);
            this.pnTest.Controls.Add(this.lbScaleOffset);
            this.pnTest.Controls.Add(this.lbZoomScale);
            this.pnTest.Controls.Add(this.lbCnt);
            this.pnTest.Controls.Add(this.CAL);
            this.pnTest.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnTest.Location = new System.Drawing.Point(0, 0);
            this.pnTest.Name = "pnTest";
            this.pnTest.Size = new System.Drawing.Size(650, 28);
            this.pnTest.TabIndex = 14;
            // 
            // tbMsg
            // 
            this.tbMsg.Location = new System.Drawing.Point(3, 3);
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.Size = new System.Drawing.Size(125, 21);
            this.tbMsg.TabIndex = 12;
            // 
            // btMsg
            // 
            this.btMsg.Location = new System.Drawing.Point(134, 3);
            this.btMsg.Name = "btMsg";
            this.btMsg.Size = new System.Drawing.Size(75, 23);
            this.btMsg.TabIndex = 13;
            this.btMsg.Text = "TMSG";
            this.btMsg.UseVisualStyleBackColor = true;
            this.btMsg.Click += new System.EventHandler(this.btMsg_Click);
            // 
            // CAL
            // 
            this.CAL.Location = new System.Drawing.Point(387, 3);
            this.CAL.Name = "CAL";
            this.CAL.Size = new System.Drawing.Size(75, 23);
            this.CAL.TabIndex = 14;
            this.CAL.Text = "CAL";
            this.CAL.UseVisualStyleBackColor = true;
            this.CAL.Click += new System.EventHandler(this.CAL_Click);
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
            // tmGrabInsp
            // 
            this.tmGrabInsp.Interval = 1;
            this.tmGrabInsp.Tick += new System.EventHandler(this.tmGrabInsp_Tick);
            // 
            // paCamTop
            // 
            this.paCamTop.Controls.Add(this.flowLayoutPanel1);
            this.paCamTop.Controls.Add(this.pnColor);
            this.paCamTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.paCamTop.Location = new System.Drawing.Point(0, 0);
            this.paCamTop.Name = "paCamTop";
            this.paCamTop.Size = new System.Drawing.Size(951, 42);
            this.paCamTop.TabIndex = 21;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.rbStratch);
            this.flowLayoutPanel1.Controls.Add(this.rbRealRatio);
            this.flowLayoutPanel1.Controls.Add(this.rbFit);
            this.flowLayoutPanel1.Controls.Add(this.btZoomIn);
            this.flowLayoutPanel1.Controls.Add(this.btZoomOut);
            this.flowLayoutPanel1.Controls.Add(this.btLoadImg);
            this.flowLayoutPanel1.Controls.Add(this.btSaveImg);
            this.flowLayoutPanel1.Controls.Add(this.btSetting);
            this.flowLayoutPanel1.Controls.Add(this.cbLiveCam);
            this.flowLayoutPanel1.Controls.Add(this.btGrabCam);
            this.flowLayoutPanel1.Controls.Add(this.btMap3D);
            this.flowLayoutPanel1.Controls.Add(this.btProfile);
            this.flowLayoutPanel1.Controls.Add(this.btGrabInspCam);
            this.flowLayoutPanel1.Controls.Add(this.cbGrabInspCam);
            this.flowLayoutPanel1.Controls.Add(this.btInsp);
            this.flowLayoutPanel1.Controls.Add(this.btRecipe);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(701, 42);
            this.flowLayoutPanel1.TabIndex = 19;
            // 
            // rbStratch
            // 
            this.rbStratch.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbStratch.BackColor = System.Drawing.SystemColors.Control;
            this.rbStratch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbStratch.BackgroundImage")));
            this.rbStratch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbStratch.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.rbStratch.Location = new System.Drawing.Point(1, 1);
            this.rbStratch.Margin = new System.Windows.Forms.Padding(1);
            this.rbStratch.Name = "rbStratch";
            this.rbStratch.Size = new System.Drawing.Size(40, 40);
            this.rbStratch.TabIndex = 15;
            this.rbStratch.UseCompatibleTextRendering = true;
            this.rbStratch.UseVisualStyleBackColor = true;
            this.rbStratch.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
            this.rbStratch.Click += new System.EventHandler(this.rbStratch_Click);
            // 
            // rbRealRatio
            // 
            this.rbRealRatio.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRealRatio.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbRealRatio.BackgroundImage")));
            this.rbRealRatio.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbRealRatio.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.rbRealRatio.Location = new System.Drawing.Point(43, 1);
            this.rbRealRatio.Margin = new System.Windows.Forms.Padding(1);
            this.rbRealRatio.Name = "rbRealRatio";
            this.rbRealRatio.Size = new System.Drawing.Size(40, 40);
            this.rbRealRatio.TabIndex = 16;
            this.rbRealRatio.UseVisualStyleBackColor = true;
            this.rbRealRatio.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
            this.rbRealRatio.Click += new System.EventHandler(this.rbStratch_Click);
            // 
            // btZoomIn
            // 
            this.btZoomIn.BackgroundImage = global::VDll.Properties.Resources.Zoom_In;
            this.btZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btZoomIn.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btZoomIn.Location = new System.Drawing.Point(127, 1);
            this.btZoomIn.Margin = new System.Windows.Forms.Padding(1);
            this.btZoomIn.Name = "btZoomIn";
            this.btZoomIn.Size = new System.Drawing.Size(40, 40);
            this.btZoomIn.TabIndex = 13;
            this.btZoomIn.UseVisualStyleBackColor = true;
            this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
            // 
            // btZoomOut
            // 
            this.btZoomOut.BackgroundImage = global::VDll.Properties.Resources.Zoom_Out;
            this.btZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btZoomOut.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btZoomOut.Location = new System.Drawing.Point(169, 1);
            this.btZoomOut.Margin = new System.Windows.Forms.Padding(1);
            this.btZoomOut.Name = "btZoomOut";
            this.btZoomOut.Size = new System.Drawing.Size(40, 40);
            this.btZoomOut.TabIndex = 12;
            this.btZoomOut.UseVisualStyleBackColor = true;
            this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
            // 
            // btLoadImg
            // 
            this.btLoadImg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btLoadImg.BackgroundImage")));
            this.btLoadImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btLoadImg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btLoadImg.Location = new System.Drawing.Point(211, 1);
            this.btLoadImg.Margin = new System.Windows.Forms.Padding(1);
            this.btLoadImg.Name = "btLoadImg";
            this.btLoadImg.Size = new System.Drawing.Size(40, 40);
            this.btLoadImg.TabIndex = 9;
            this.btLoadImg.UseVisualStyleBackColor = true;
            this.btLoadImg.Click += new System.EventHandler(this.btLoadImg_Click);
            // 
            // btSaveImg
            // 
            this.btSaveImg.BackgroundImage = global::VDll.Properties.Resources.Save_Picture;
            this.btSaveImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSaveImg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btSaveImg.Location = new System.Drawing.Point(253, 1);
            this.btSaveImg.Margin = new System.Windows.Forms.Padding(1);
            this.btSaveImg.Name = "btSaveImg";
            this.btSaveImg.Size = new System.Drawing.Size(40, 40);
            this.btSaveImg.TabIndex = 10;
            this.btSaveImg.UseVisualStyleBackColor = true;
            this.btSaveImg.Click += new System.EventHandler(this.btSaveImg_Click);
            // 
            // btSetting
            // 
            this.btSetting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btSetting.BackgroundImage")));
            this.btSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSetting.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btSetting.Location = new System.Drawing.Point(295, 1);
            this.btSetting.Margin = new System.Windows.Forms.Padding(1);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(40, 40);
            this.btSetting.TabIndex = 19;
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // cbLiveCam
            // 
            this.cbLiveCam.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLiveCam.BackgroundImage = global::VDll.Properties.Resources.Live;
            this.cbLiveCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbLiveCam.Location = new System.Drawing.Point(337, 1);
            this.cbLiveCam.Margin = new System.Windows.Forms.Padding(1);
            this.cbLiveCam.Name = "cbLiveCam";
            this.cbLiveCam.Size = new System.Drawing.Size(40, 40);
            this.cbLiveCam.TabIndex = 22;
            this.cbLiveCam.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbLiveCam.UseVisualStyleBackColor = true;
            this.cbLiveCam.CheckedChanged += new System.EventHandler(this.cbLiveCam_CheckedChanged);
            // 
            // btGrabCam
            // 
            this.btGrabCam.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btGrabCam.BackgroundImage")));
            this.btGrabCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btGrabCam.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btGrabCam.Location = new System.Drawing.Point(379, 1);
            this.btGrabCam.Margin = new System.Windows.Forms.Padding(1);
            this.btGrabCam.Name = "btGrabCam";
            this.btGrabCam.Size = new System.Drawing.Size(40, 40);
            this.btGrabCam.TabIndex = 18;
            this.btGrabCam.UseVisualStyleBackColor = true;
            this.btGrabCam.Click += new System.EventHandler(this.btGrabCam_Click);
            // 
            // btMap3D
            // 
            this.btMap3D.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btMap3D.BackgroundImage")));
            this.btMap3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btMap3D.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btMap3D.Location = new System.Drawing.Point(421, 1);
            this.btMap3D.Margin = new System.Windows.Forms.Padding(1);
            this.btMap3D.Name = "btMap3D";
            this.btMap3D.Size = new System.Drawing.Size(40, 40);
            this.btMap3D.TabIndex = 20;
            this.btMap3D.UseVisualStyleBackColor = true;
            this.btMap3D.Click += new System.EventHandler(this.btMap3D_Click);
            // 
            // btProfile
            // 
            this.btProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btProfile.BackgroundImage")));
            this.btProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btProfile.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btProfile.Location = new System.Drawing.Point(463, 1);
            this.btProfile.Margin = new System.Windows.Forms.Padding(1);
            this.btProfile.Name = "btProfile";
            this.btProfile.Size = new System.Drawing.Size(40, 40);
            this.btProfile.TabIndex = 21;
            this.btProfile.UseVisualStyleBackColor = true;
            this.btProfile.Click += new System.EventHandler(this.btProfile_Click);
            // 
            // btGrabInspCam
            // 
            this.btGrabInspCam.BackgroundImage = global::VDll.Properties.Resources.Grab;
            this.btGrabInspCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btGrabInspCam.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btGrabInspCam.Location = new System.Drawing.Point(505, 1);
            this.btGrabInspCam.Margin = new System.Windows.Forms.Padding(1);
            this.btGrabInspCam.Name = "btGrabInspCam";
            this.btGrabInspCam.Size = new System.Drawing.Size(40, 40);
            this.btGrabInspCam.TabIndex = 17;
            this.btGrabInspCam.UseVisualStyleBackColor = true;
            this.btGrabInspCam.Click += new System.EventHandler(this.btGrabInspCam_Click);
            // 
            // cbGrabInspCam
            // 
            this.cbGrabInspCam.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbGrabInspCam.BackgroundImage = global::VDll.Properties.Resources.Grab;
            this.cbGrabInspCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbGrabInspCam.Location = new System.Drawing.Point(547, 1);
            this.cbGrabInspCam.Margin = new System.Windows.Forms.Padding(1);
            this.cbGrabInspCam.Name = "cbGrabInspCam";
            this.cbGrabInspCam.Size = new System.Drawing.Size(40, 40);
            this.cbGrabInspCam.TabIndex = 24;
            this.cbGrabInspCam.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbGrabInspCam.UseVisualStyleBackColor = true;
            this.cbGrabInspCam.CheckedChanged += new System.EventHandler(this.cbGrabInspCam_CheckedChanged);
            // 
            // btInsp
            // 
            this.btInsp.BackgroundImage = global::VDll.Properties.Resources.Play;
            this.btInsp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btInsp.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btInsp.Location = new System.Drawing.Point(589, 1);
            this.btInsp.Margin = new System.Windows.Forms.Padding(1);
            this.btInsp.Name = "btInsp";
            this.btInsp.Size = new System.Drawing.Size(40, 40);
            this.btInsp.TabIndex = 23;
            this.btInsp.UseVisualStyleBackColor = true;
            this.btInsp.Click += new System.EventHandler(this.btInsp_Click);
            // 
            // btRecipe
            // 
            this.btRecipe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btRecipe.BackgroundImage")));
            this.btRecipe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btRecipe.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btRecipe.Location = new System.Drawing.Point(631, 1);
            this.btRecipe.Margin = new System.Windows.Forms.Padding(1);
            this.btRecipe.Name = "btRecipe";
            this.btRecipe.Size = new System.Drawing.Size(40, 40);
            this.btRecipe.TabIndex = 25;
            this.btRecipe.UseVisualStyleBackColor = true;
            this.btRecipe.Click += new System.EventHandler(this.btRecipe_Click);
            // 
            // pnColor
            // 
            this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnColor.Controls.Add(this.lbPixel);
            this.pnColor.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnColor.Location = new System.Drawing.Point(701, 0);
            this.pnColor.Name = "pnColor";
            this.pnColor.Size = new System.Drawing.Size(250, 42);
            this.pnColor.TabIndex = 19;
            // 
            // lbPixel
            // 
            this.lbPixel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPixel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbPixel.Location = new System.Drawing.Point(0, 0);
            this.lbPixel.Name = "lbPixel";
            this.lbPixel.Size = new System.Drawing.Size(248, 40);
            this.lbPixel.TabIndex = 0;
            this.lbPixel.Text = "Pixel";
            this.lbPixel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.Black;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibCam.Location = new System.Drawing.Point(0, 42);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(951, 555);
            this.ibCam.TabIndex = 3;
            this.ibCam.TabStop = false;
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibCam_Paint);
            this.ibCam.DoubleClick += new System.EventHandler(this.ibCam_DoubleClick);
            this.ibCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseDown);
            this.ibCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseMove);
            this.ibCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseUp);
            this.ibCam.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ibCam_PreviewKeyDown);
            // 
            // rbFit
            // 
            this.rbFit.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbFit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbFit.BackgroundImage")));
            this.rbFit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbFit.Checked = true;
            this.rbFit.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.rbFit.Location = new System.Drawing.Point(85, 1);
            this.rbFit.Margin = new System.Windows.Forms.Padding(1);
            this.rbFit.Name = "rbFit";
            this.rbFit.Size = new System.Drawing.Size(40, 40);
            this.rbFit.TabIndex = 26;
            this.rbFit.TabStop = true;
            this.rbFit.UseVisualStyleBackColor = true;
            this.rbFit.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
            this.rbFit.Click += new System.EventHandler(this.rbStratch_Click);
            // 
            // FormVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 625);
            this.Controls.Add(this.ibCam);
            this.Controls.Add(this.paCamTop);
            this.Controls.Add(this.pnSub);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormVision";
            this.Text = "FormVision";
            this.Load += new System.EventHandler(this.FormVision_Load);
            this.Resize += new System.EventHandler(this.FormVision_Resize);
            this.pnSub.ResumeLayout(false);
            this.pnTest.ResumeLayout(false);
            this.pnTest.PerformLayout();
            this.paCamTop.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnSub;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Label lbGrabTime;
        private System.Windows.Forms.Label lbCnt;
        private System.Windows.Forms.Label lbZoomScale;
        private System.Windows.Forms.Timer tmLive;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.Label lbScaleOffset;
        private System.Windows.Forms.Button btMsg;
        private System.Windows.Forms.TextBox tbMsg;
        private System.Windows.Forms.Timer tmGrabInsp;
        private System.Windows.Forms.Panel paCamTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rbStratch;
        private System.Windows.Forms.RadioButton rbRealRatio;
        private System.Windows.Forms.Button btZoomIn;
        private System.Windows.Forms.Button btZoomOut;
        private System.Windows.Forms.Button btLoadImg;
        private System.Windows.Forms.Button btSaveImg;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.CheckBox cbLiveCam;
        private System.Windows.Forms.Button btGrabCam;
        private System.Windows.Forms.Button btMap3D;
        private System.Windows.Forms.Button btProfile;
        private System.Windows.Forms.Button btGrabInspCam;
        private System.Windows.Forms.Panel pnColor;
        private System.Windows.Forms.Label lbPixel;
        private System.Windows.Forms.Button btInsp;
        private System.Windows.Forms.CheckBox cbGrabInspCam;
        private System.Windows.Forms.FlowLayoutPanel pnTest;
        private System.Windows.Forms.Button CAL;
        private System.Windows.Forms.Button btRecipe;
        private System.Windows.Forms.RadioButton rbFit;
    }
}