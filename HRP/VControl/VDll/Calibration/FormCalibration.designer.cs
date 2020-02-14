namespace VDll
{
    partial class FormCalibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCalibration));
            this.panel2 = new System.Windows.Forms.Panel();
            this.paCamTop = new System.Windows.Forms.Panel();
            this.pnTitle = new System.Windows.Forms.FlowLayoutPanel();
            this.pnColor = new System.Windows.Forms.Panel();
            this.lbPixel = new System.Windows.Forms.Label();
            this.pnControlBase = new System.Windows.Forms.Panel();
            this.pnffgff = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.pgPara = new System.Windows.Forms.PropertyGrid();
            this.pgParaCommon = new System.Windows.Forms.PropertyGrid();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnCamera = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lvPkg = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
            this.btLiveCam = new System.Windows.Forms.Button();
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.btTrain = new System.Windows.Forms.Button();
            this.btSavePara = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.paCamTop.SuspendLayout();
            this.pnTitle.SuspendLayout();
            this.pnColor.SuspendLayout();
            this.pnControlBase.SuspendLayout();
            this.pnffgff.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pnCamera.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.paCamTop);
            this.panel2.Controls.Add(this.ibCam);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(935, 801);
            this.panel2.TabIndex = 7;
            // 
            // paCamTop
            // 
            this.paCamTop.Controls.Add(this.pnTitle);
            this.paCamTop.Controls.Add(this.pnColor);
            this.paCamTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.paCamTop.Location = new System.Drawing.Point(0, 0);
            this.paCamTop.Name = "paCamTop";
            this.paCamTop.Size = new System.Drawing.Size(933, 42);
            this.paCamTop.TabIndex = 20;
            // 
            // pnTitle
            // 
            this.pnTitle.BackColor = System.Drawing.SystemColors.Control;
            this.pnTitle.Controls.Add(this.rbStratch);
            this.pnTitle.Controls.Add(this.rbRealRatio);
            this.pnTitle.Controls.Add(this.btZoomIn);
            this.pnTitle.Controls.Add(this.btZoomOut);
            this.pnTitle.Controls.Add(this.btLoadImg);
            this.pnTitle.Controls.Add(this.btSaveImg);
            this.pnTitle.Controls.Add(this.btSetting);
            this.pnTitle.Controls.Add(this.cbLiveCam);
            this.pnTitle.Controls.Add(this.btGrabCam);
            this.pnTitle.Controls.Add(this.btMap3D);
            this.pnTitle.Controls.Add(this.btProfile);
            this.pnTitle.Controls.Add(this.btLiveCam);
            this.pnTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnTitle.Location = new System.Drawing.Point(0, 0);
            this.pnTitle.Name = "pnTitle";
            this.pnTitle.Size = new System.Drawing.Size(683, 42);
            this.pnTitle.TabIndex = 19;
            // 
            // pnColor
            // 
            this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnColor.Controls.Add(this.lbPixel);
            this.pnColor.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnColor.Location = new System.Drawing.Point(683, 0);
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
            // pnControlBase
            // 
            this.pnControlBase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnControlBase.Controls.Add(this.pnffgff);
            this.pnControlBase.Controls.Add(this.pnCamera);
            this.pnControlBase.Controls.Add(this.panel4);
            this.pnControlBase.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnControlBase.Location = new System.Drawing.Point(935, 0);
            this.pnControlBase.Name = "pnControlBase";
            this.pnControlBase.Size = new System.Drawing.Size(329, 801);
            this.pnControlBase.TabIndex = 8;
            // 
            // pnffgff
            // 
            this.pnffgff.Controls.Add(this.panel10);
            this.pnffgff.Controls.Add(this.panel6);
            this.pnffgff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnffgff.Location = new System.Drawing.Point(0, 241);
            this.pnffgff.Name = "pnffgff";
            this.pnffgff.Size = new System.Drawing.Size(327, 558);
            this.pnffgff.TabIndex = 6;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.SystemColors.Control;
            this.panel10.Controls.Add(this.pgPara);
            this.panel10.Controls.Add(this.pgParaCommon);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 30);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(327, 528);
            this.panel10.TabIndex = 7;
            // 
            // pgPara
            // 
            this.pgPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPara.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.pgPara.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgPara.Location = new System.Drawing.Point(0, 153);
            this.pgPara.Name = "pgPara";
            this.pgPara.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgPara.Size = new System.Drawing.Size(327, 375);
            this.pgPara.TabIndex = 7;
            this.pgPara.ToolbarVisible = false;
            // 
            // pgParaCommon
            // 
            this.pgParaCommon.Dock = System.Windows.Forms.DockStyle.Top;
            this.pgParaCommon.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.pgParaCommon.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgParaCommon.Location = new System.Drawing.Point(0, 0);
            this.pgParaCommon.Name = "pgParaCommon";
            this.pgParaCommon.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgParaCommon.Size = new System.Drawing.Size(327, 153);
            this.pgParaCommon.TabIndex = 8;
            this.pgParaCommon.ToolbarVisible = false;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel6.Controls.Add(this.label3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(327, 30);
            this.panel6.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(1);
            this.label3.Size = new System.Drawing.Size(327, 30);
            this.label3.TabIndex = 1;
            this.label3.Text = "PARAMETER";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnCamera
            // 
            this.pnCamera.Controls.Add(this.flowLayoutPanel1);
            this.pnCamera.Controls.Add(this.panel9);
            this.pnCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCamera.Location = new System.Drawing.Point(0, 155);
            this.pnCamera.Name = "pnCamera";
            this.pnCamera.Size = new System.Drawing.Size(327, 86);
            this.pnCamera.TabIndex = 7;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btTrain);
            this.flowLayoutPanel1.Controls.Add(this.btSavePara);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(327, 56);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel9.Controls.Add(this.label2);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(327, 30);
            this.panel9.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(327, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "CONTROL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lvPkg);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(327, 155);
            this.panel4.TabIndex = 5;
            // 
            // lvPkg
            // 
            this.lvPkg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lvPkg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvPkg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPkg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lvPkg.FullRowSelect = true;
            this.lvPkg.GridLines = true;
            this.lvPkg.HideSelection = false;
            this.lvPkg.Location = new System.Drawing.Point(0, 30);
            this.lvPkg.MultiSelect = false;
            this.lvPkg.Name = "lvPkg";
            this.lvPkg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lvPkg.Size = new System.Drawing.Size(327, 125);
            this.lvPkg.TabIndex = 5;
            this.lvPkg.UseCompatibleStateImageBehavior = false;
            this.lvPkg.View = System.Windows.Forms.View.Details;
            this.lvPkg.SelectedIndexChanged += new System.EventHandler(this.lvPkg_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(327, 30);
            this.panel3.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "PACKAGE LIST";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
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
            this.rbStratch.TabStop = true;
            this.toolTip1.SetToolTip(this.rbStratch, "Stratch");
            this.rbStratch.UseCompatibleTextRendering = true;
            this.rbStratch.UseVisualStyleBackColor = true;
            this.rbStratch.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
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
            this.rbRealRatio.TabStop = true;
            this.toolTip1.SetToolTip(this.rbRealRatio, "Real Size");
            this.rbRealRatio.UseVisualStyleBackColor = true;
            this.rbRealRatio.CheckedChanged += new System.EventHandler(this.rbRealRatio_CheckedChanged);
            // 
            // btZoomIn
            // 
            this.btZoomIn.BackgroundImage = global::VDll.Properties.Resources.Zoom_In;
            this.btZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btZoomIn.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btZoomIn.Location = new System.Drawing.Point(85, 1);
            this.btZoomIn.Margin = new System.Windows.Forms.Padding(1);
            this.btZoomIn.Name = "btZoomIn";
            this.btZoomIn.Size = new System.Drawing.Size(40, 40);
            this.btZoomIn.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btZoomIn, "Zoom In");
            this.btZoomIn.UseVisualStyleBackColor = true;
            this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
            // 
            // btZoomOut
            // 
            this.btZoomOut.BackgroundImage = global::VDll.Properties.Resources.Zoom_Out;
            this.btZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btZoomOut.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btZoomOut.Location = new System.Drawing.Point(127, 1);
            this.btZoomOut.Margin = new System.Windows.Forms.Padding(1);
            this.btZoomOut.Name = "btZoomOut";
            this.btZoomOut.Size = new System.Drawing.Size(40, 40);
            this.btZoomOut.TabIndex = 12;
            this.toolTip1.SetToolTip(this.btZoomOut, "Zoom Out");
            this.btZoomOut.UseVisualStyleBackColor = true;
            this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
            // 
            // btLoadImg
            // 
            this.btLoadImg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btLoadImg.BackgroundImage")));
            this.btLoadImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btLoadImg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btLoadImg.Location = new System.Drawing.Point(169, 1);
            this.btLoadImg.Margin = new System.Windows.Forms.Padding(1);
            this.btLoadImg.Name = "btLoadImg";
            this.btLoadImg.Size = new System.Drawing.Size(40, 40);
            this.btLoadImg.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btLoadImg, "Open Picture");
            this.btLoadImg.UseVisualStyleBackColor = true;
            this.btLoadImg.Click += new System.EventHandler(this.btLoadImg_Click);
            // 
            // btSaveImg
            // 
            this.btSaveImg.BackgroundImage = global::VDll.Properties.Resources.Save_Picture;
            this.btSaveImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSaveImg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btSaveImg.Location = new System.Drawing.Point(211, 1);
            this.btSaveImg.Margin = new System.Windows.Forms.Padding(1);
            this.btSaveImg.Name = "btSaveImg";
            this.btSaveImg.Size = new System.Drawing.Size(40, 40);
            this.btSaveImg.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btSaveImg, "Save Picture");
            this.btSaveImg.UseVisualStyleBackColor = true;
            this.btSaveImg.Click += new System.EventHandler(this.btSaveImg_Click);
            // 
            // btSetting
            // 
            this.btSetting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btSetting.BackgroundImage")));
            this.btSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSetting.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btSetting.Location = new System.Drawing.Point(253, 1);
            this.btSetting.Margin = new System.Windows.Forms.Padding(1);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(40, 40);
            this.btSetting.TabIndex = 19;
            this.toolTip1.SetToolTip(this.btSetting, "Camera Grab");
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Visible = false;
            // 
            // cbLiveCam
            // 
            this.cbLiveCam.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLiveCam.BackgroundImage = global::VDll.Properties.Resources.Live;
            this.cbLiveCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbLiveCam.Location = new System.Drawing.Point(295, 1);
            this.cbLiveCam.Margin = new System.Windows.Forms.Padding(1);
            this.cbLiveCam.Name = "cbLiveCam";
            this.cbLiveCam.Size = new System.Drawing.Size(40, 40);
            this.cbLiveCam.TabIndex = 22;
            this.cbLiveCam.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.cbLiveCam, "Live");
            this.cbLiveCam.UseVisualStyleBackColor = true;
            this.cbLiveCam.Visible = false;
            // 
            // btGrabCam
            // 
            this.btGrabCam.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btGrabCam.BackgroundImage")));
            this.btGrabCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btGrabCam.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btGrabCam.Location = new System.Drawing.Point(337, 1);
            this.btGrabCam.Margin = new System.Windows.Forms.Padding(1);
            this.btGrabCam.Name = "btGrabCam";
            this.btGrabCam.Size = new System.Drawing.Size(40, 40);
            this.btGrabCam.TabIndex = 18;
            this.toolTip1.SetToolTip(this.btGrabCam, "Camera Live");
            this.btGrabCam.UseVisualStyleBackColor = true;
            this.btGrabCam.Visible = false;
            this.btGrabCam.Click += new System.EventHandler(this.btGrabCam_Click);
            // 
            // btMap3D
            // 
            this.btMap3D.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btMap3D.BackgroundImage")));
            this.btMap3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btMap3D.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btMap3D.Location = new System.Drawing.Point(379, 1);
            this.btMap3D.Margin = new System.Windows.Forms.Padding(1);
            this.btMap3D.Name = "btMap3D";
            this.btMap3D.Size = new System.Drawing.Size(40, 40);
            this.btMap3D.TabIndex = 20;
            this.toolTip1.SetToolTip(this.btMap3D, "3D Map");
            this.btMap3D.UseVisualStyleBackColor = true;
            this.btMap3D.Visible = false;
            // 
            // btProfile
            // 
            this.btProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btProfile.BackgroundImage")));
            this.btProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btProfile.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btProfile.Location = new System.Drawing.Point(421, 1);
            this.btProfile.Margin = new System.Windows.Forms.Padding(1);
            this.btProfile.Name = "btProfile";
            this.btProfile.Size = new System.Drawing.Size(40, 40);
            this.btProfile.TabIndex = 21;
            this.toolTip1.SetToolTip(this.btProfile, "Profile");
            this.btProfile.UseVisualStyleBackColor = true;
            this.btProfile.Visible = false;
            this.btProfile.Click += new System.EventHandler(this.btProfile_Click);
            // 
            // btLiveCam
            // 
            this.btLiveCam.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btLiveCam.BackgroundImage")));
            this.btLiveCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btLiveCam.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btLiveCam.Location = new System.Drawing.Point(463, 1);
            this.btLiveCam.Margin = new System.Windows.Forms.Padding(1);
            this.btLiveCam.Name = "btLiveCam";
            this.btLiveCam.Size = new System.Drawing.Size(40, 40);
            this.btLiveCam.TabIndex = 17;
            this.toolTip1.SetToolTip(this.btLiveCam, "Train");
            this.btLiveCam.UseVisualStyleBackColor = true;
            this.btLiveCam.Visible = false;
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.Black;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibCam.Location = new System.Drawing.Point(0, 0);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(933, 799);
            this.ibCam.TabIndex = 4;
            this.ibCam.TabStop = false;
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibTrainCam_Paint);
            this.ibCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseDown);
            this.ibCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseMove);
            this.ibCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseUp);
            this.ibCam.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ibCam_PreviewKeyDown);
            // 
            // btTrain
            // 
            this.btTrain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btTrain.BackgroundImage")));
            this.btTrain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btTrain.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTrain.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btTrain.Location = new System.Drawing.Point(3, 3);
            this.btTrain.Name = "btTrain";
            this.btTrain.Size = new System.Drawing.Size(50, 50);
            this.btTrain.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btTrain, "Train");
            this.btTrain.UseVisualStyleBackColor = true;
            this.btTrain.Click += new System.EventHandler(this.btTrain_Click);
            // 
            // btSavePara
            // 
            this.btSavePara.BackgroundImage = global::VDll.Properties.Resources.Save_Parameter;
            this.btSavePara.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btSavePara.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSavePara.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btSavePara.Location = new System.Drawing.Point(59, 3);
            this.btSavePara.Name = "btSavePara";
            this.btSavePara.Size = new System.Drawing.Size(50, 50);
            this.btSavePara.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btSavePara, "Save Parameter");
            this.btSavePara.UseVisualStyleBackColor = true;
            this.btSavePara.Click += new System.EventHandler(this.btSavePara_Click);
            // 
            // FormCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 801);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnControlBase);
            this.Name = "FormCalibration";
            this.Text = "FormCalibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCalibration_FormClosing);
            this.Shown += new System.EventHandler(this.FormCalibration_Shown);
            this.Move += new System.EventHandler(this.FormCalibration_Move);
            this.panel2.ResumeLayout(false);
            this.paCamTop.ResumeLayout(false);
            this.pnTitle.ResumeLayout(false);
            this.pnColor.ResumeLayout(false);
            this.pnControlBase.ResumeLayout(false);
            this.pnffgff.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.pnCamera.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.RadioButton rbRealRatio;
        private System.Windows.Forms.RadioButton rbStratch;
        private System.Windows.Forms.Button btZoomIn;
        private System.Windows.Forms.Button btZoomOut;
        private System.Windows.Forms.Panel pnControlBase;
        private System.Windows.Forms.PropertyGrid pgPara;
        private System.Windows.Forms.Panel pnffgff;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSavePara;
        private System.Windows.Forms.Panel pnCamera;
        private System.Windows.Forms.Button btTrain;
        private System.Windows.Forms.Button btSaveImg;
        private System.Windows.Forms.Button btLoadImg;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListView lvPkg;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel pnTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PropertyGrid pgParaCommon;
        private System.Windows.Forms.Panel pnColor;
        private System.Windows.Forms.Label lbPixel;
        private System.Windows.Forms.Panel paCamTop;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Button btLiveCam;
        private System.Windows.Forms.Button btGrabCam;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Button btMap3D;
        private System.Windows.Forms.Button btProfile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbLiveCam;
    }
}