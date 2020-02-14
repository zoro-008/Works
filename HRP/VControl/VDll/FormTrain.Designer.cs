namespace VDll
{
    partial class FormTrain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrain));
            this.pnControlBase = new System.Windows.Forms.Panel();
            this.pgPara = new System.Windows.Forms.PropertyGrid();
            this.pnffgff = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.rbMasterPara = new System.Windows.Forms.RadioButton();
            this.btSave = new System.Windows.Forms.Button();
            this.rbUserPara = new System.Windows.Forms.RadioButton();
            this.btGlobalValue = new System.Windows.Forms.Button();
            this.btLocalValue = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnCamera = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btCapture = new System.Windows.Forms.Button();
            this.cbLiveCam = new System.Windows.Forms.CheckBox();
            this.btLoadImg = new System.Windows.Forms.Button();
            this.btSaveImg = new System.Windows.Forms.Button();
            this.btSetting = new System.Windows.Forms.Button();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lvPkg = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btPkgEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btNext = new System.Windows.Forms.Button();
            this.btPre = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ibTrainCam = new Emgu.CV.UI.ImageBox();
            this.paCamTop = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbStratch = new System.Windows.Forms.RadioButton();
            this.rbRealRatio = new System.Windows.Forms.RadioButton();
            this.btZoomIn = new System.Windows.Forms.Button();
            this.btZoomOut = new System.Windows.Forms.Button();
            this.btProfile = new System.Windows.Forms.Button();
            this.btInsp = new System.Windows.Forms.Button();
            this.btMap3D = new System.Windows.Forms.Button();
            this.btGrabCam = new System.Windows.Forms.Button();
            this.pnColor = new System.Windows.Forms.Panel();
            this.lbPixel = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmLive = new System.Windows.Forms.Timer(this.components);
            this.rbFit = new System.Windows.Forms.RadioButton();
            this.pnControlBase.SuspendLayout();
            this.pnffgff.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pnCamera.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibTrainCam)).BeginInit();
            this.paCamTop.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnControlBase
            // 
            this.pnControlBase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnControlBase.Controls.Add(this.pgPara);
            this.pnControlBase.Controls.Add(this.pnffgff);
            this.pnControlBase.Controls.Add(this.pnCamera);
            this.pnControlBase.Controls.Add(this.panel4);
            this.pnControlBase.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnControlBase.Location = new System.Drawing.Point(935, 0);
            this.pnControlBase.Name = "pnControlBase";
            this.pnControlBase.Size = new System.Drawing.Size(329, 985);
            this.pnControlBase.TabIndex = 5;
            // 
            // pgPara
            // 
            this.pgPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPara.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pgPara.HelpVisible = false;
            this.pgPara.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgPara.Location = new System.Drawing.Point(0, 341);
            this.pgPara.Name = "pgPara";
            this.pgPara.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgPara.Size = new System.Drawing.Size(325, 640);
            this.pgPara.TabIndex = 7;
            this.pgPara.ToolbarVisible = false;
            this.pgPara.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgPara_PropertyValueChanged);
            // 
            // pnffgff
            // 
            this.pnffgff.Controls.Add(this.panel10);
            this.pnffgff.Controls.Add(this.panel6);
            this.pnffgff.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnffgff.Location = new System.Drawing.Point(0, 252);
            this.pnffgff.Name = "pnffgff";
            this.pnffgff.Size = new System.Drawing.Size(325, 89);
            this.pnffgff.TabIndex = 6;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.SystemColors.Control;
            this.panel10.Controls.Add(this.rbMasterPara);
            this.panel10.Controls.Add(this.btSave);
            this.panel10.Controls.Add(this.rbUserPara);
            this.panel10.Controls.Add(this.btGlobalValue);
            this.panel10.Controls.Add(this.btLocalValue);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 30);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(325, 59);
            this.panel10.TabIndex = 7;
            // 
            // rbMasterPara
            // 
            this.rbMasterPara.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterPara.Location = new System.Drawing.Point(59, 3);
            this.rbMasterPara.Name = "rbMasterPara";
            this.rbMasterPara.Size = new System.Drawing.Size(50, 50);
            this.rbMasterPara.TabIndex = 1;
            this.rbMasterPara.Text = "MASTER";
            this.rbMasterPara.UseVisualStyleBackColor = true;
            this.rbMasterPara.CheckedChanged += new System.EventHandler(this.rbMasterPara_CheckedChanged);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(112, 4);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(50, 50);
            this.btSave.TabIndex = 13;
            this.btSave.Text = "SAVE";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // rbUserPara
            // 
            this.rbUserPara.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbUserPara.Checked = true;
            this.rbUserPara.Location = new System.Drawing.Point(4, 4);
            this.rbUserPara.Name = "rbUserPara";
            this.rbUserPara.Size = new System.Drawing.Size(50, 50);
            this.rbUserPara.TabIndex = 0;
            this.rbUserPara.TabStop = true;
            this.rbUserPara.Text = "USER";
            this.rbUserPara.UseVisualStyleBackColor = true;
            this.rbUserPara.CheckedChanged += new System.EventHandler(this.rbUserPara_CheckedChanged);
            // 
            // btGlobalValue
            // 
            this.btGlobalValue.Location = new System.Drawing.Point(270, 4);
            this.btGlobalValue.Name = "btGlobalValue";
            this.btGlobalValue.Size = new System.Drawing.Size(50, 50);
            this.btGlobalValue.TabIndex = 15;
            this.btGlobalValue.Text = "GLOBAL VALUE TABLE";
            this.btGlobalValue.UseVisualStyleBackColor = true;
            this.btGlobalValue.Click += new System.EventHandler(this.btGlobalValue_Click);
            // 
            // btLocalValue
            // 
            this.btLocalValue.Location = new System.Drawing.Point(216, 4);
            this.btLocalValue.Name = "btLocalValue";
            this.btLocalValue.Size = new System.Drawing.Size(50, 50);
            this.btLocalValue.TabIndex = 14;
            this.btLocalValue.Text = "VALUE TABLE";
            this.btLocalValue.UseVisualStyleBackColor = true;
            this.btLocalValue.Click += new System.EventHandler(this.btLocalValue_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel6.Controls.Add(this.label3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(325, 30);
            this.panel6.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(325, 30);
            this.label3.TabIndex = 1;
            this.label3.Text = "PARAMETER";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnCamera
            // 
            this.pnCamera.Controls.Add(this.flowLayoutPanel2);
            this.pnCamera.Controls.Add(this.panel9);
            this.pnCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCamera.Location = new System.Drawing.Point(0, 180);
            this.pnCamera.Name = "pnCamera";
            this.pnCamera.Size = new System.Drawing.Size(325, 72);
            this.pnCamera.TabIndex = 7;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btCapture);
            this.flowLayoutPanel2.Controls.Add(this.cbLiveCam);
            this.flowLayoutPanel2.Controls.Add(this.btLoadImg);
            this.flowLayoutPanel2.Controls.Add(this.btSaveImg);
            this.flowLayoutPanel2.Controls.Add(this.btSetting);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 30);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(325, 42);
            this.flowLayoutPanel2.TabIndex = 12;
            // 
            // btCapture
            // 
            this.btCapture.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btCapture.BackgroundImage")));
            this.btCapture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btCapture.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btCapture.Location = new System.Drawing.Point(1, 1);
            this.btCapture.Margin = new System.Windows.Forms.Padding(1);
            this.btCapture.Name = "btCapture";
            this.btCapture.Size = new System.Drawing.Size(40, 40);
            this.btCapture.TabIndex = 24;
            this.btCapture.UseVisualStyleBackColor = true;
            this.btCapture.Click += new System.EventHandler(this.btCapture_Click);
            // 
            // cbLiveCam
            // 
            this.cbLiveCam.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLiveCam.BackgroundImage = global::VDll.Properties.Resources.Live;
            this.cbLiveCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbLiveCam.Location = new System.Drawing.Point(43, 1);
            this.cbLiveCam.Margin = new System.Windows.Forms.Padding(1);
            this.cbLiveCam.Name = "cbLiveCam";
            this.cbLiveCam.Size = new System.Drawing.Size(40, 40);
            this.cbLiveCam.TabIndex = 23;
            this.cbLiveCam.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbLiveCam.UseVisualStyleBackColor = true;
            this.cbLiveCam.CheckedChanged += new System.EventHandler(this.cbLiveCam_CheckedChanged);
            // 
            // btLoadImg
            // 
            this.btLoadImg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btLoadImg.BackgroundImage")));
            this.btLoadImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btLoadImg.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btLoadImg.Location = new System.Drawing.Point(85, 1);
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
            this.btSaveImg.Location = new System.Drawing.Point(127, 1);
            this.btSaveImg.Margin = new System.Windows.Forms.Padding(1);
            this.btSaveImg.Name = "btSaveImg";
            this.btSaveImg.Size = new System.Drawing.Size(40, 40);
            this.btSaveImg.TabIndex = 10;
            this.btSaveImg.UseVisualStyleBackColor = true;
            this.btSaveImg.Click += new System.EventHandler(this.btSaveImg_Click);
            // 
            // btSetting
            // 
            this.btSetting.Location = new System.Drawing.Point(171, 3);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(77, 38);
            this.btSetting.TabIndex = 11;
            this.btSetting.Text = "SETTING";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel9.Controls.Add(this.label2);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(325, 30);
            this.panel9.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(325, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "CONTROL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lvPkg);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(325, 180);
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
            this.lvPkg.Size = new System.Drawing.Size(325, 116);
            this.lvPkg.TabIndex = 5;
            this.lvPkg.UseCompatibleStateImageBehavior = false;
            this.lvPkg.View = System.Windows.Forms.View.Details;
            this.lvPkg.SelectedIndexChanged += new System.EventHandler(this.lvPkg_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.Controls.Add(this.btPkgEdit);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(325, 30);
            this.panel3.TabIndex = 7;
            // 
            // btPkgEdit
            // 
            this.btPkgEdit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btPkgEdit.BackgroundImage")));
            this.btPkgEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btPkgEdit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btPkgEdit.Location = new System.Drawing.Point(292, 0);
            this.btPkgEdit.Name = "btPkgEdit";
            this.btPkgEdit.Size = new System.Drawing.Size(33, 30);
            this.btPkgEdit.TabIndex = 8;
            this.btPkgEdit.UseVisualStyleBackColor = true;
            this.btPkgEdit.Click += new System.EventHandler(this.btPkgEdit_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(325, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "PACKAGE LIST";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel7.Controls.Add(this.btNext);
            this.panel7.Controls.Add(this.btPre);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 146);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(325, 34);
            this.panel7.TabIndex = 6;
            // 
            // btNext
            // 
            this.btNext.BackgroundImage = global::VDll.Properties.Resources.Right;
            this.btNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btNext.Location = new System.Drawing.Point(162, 0);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(163, 34);
            this.btNext.TabIndex = 1;
            this.btNext.UseVisualStyleBackColor = true;
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // btPre
            // 
            this.btPre.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btPre.BackgroundImage")));
            this.btPre.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btPre.Dock = System.Windows.Forms.DockStyle.Left;
            this.btPre.Location = new System.Drawing.Point(0, 0);
            this.btPre.Name = "btPre";
            this.btPre.Size = new System.Drawing.Size(162, 34);
            this.btPre.TabIndex = 0;
            this.btPre.UseVisualStyleBackColor = true;
            this.btPre.Click += new System.EventHandler(this.btPre_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ibTrainCam);
            this.panel2.Controls.Add(this.paCamTop);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(935, 985);
            this.panel2.TabIndex = 6;
            // 
            // ibTrainCam
            // 
            this.ibTrainCam.BackColor = System.Drawing.Color.Black;
            this.ibTrainCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibTrainCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibTrainCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibTrainCam.Location = new System.Drawing.Point(0, 42);
            this.ibTrainCam.Name = "ibTrainCam";
            this.ibTrainCam.Size = new System.Drawing.Size(935, 943);
            this.ibTrainCam.TabIndex = 4;
            this.ibTrainCam.TabStop = false;
            this.ibTrainCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibTrainCam_Paint);
            this.ibTrainCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseDown);
            this.ibTrainCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseMove);
            this.ibTrainCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseUp);
            // 
            // paCamTop
            // 
            this.paCamTop.Controls.Add(this.flowLayoutPanel1);
            this.paCamTop.Controls.Add(this.pnColor);
            this.paCamTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.paCamTop.Location = new System.Drawing.Point(0, 0);
            this.paCamTop.Name = "paCamTop";
            this.paCamTop.Size = new System.Drawing.Size(935, 42);
            this.paCamTop.TabIndex = 22;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.rbStratch);
            this.flowLayoutPanel1.Controls.Add(this.rbRealRatio);
            this.flowLayoutPanel1.Controls.Add(this.rbFit);
            this.flowLayoutPanel1.Controls.Add(this.btZoomIn);
            this.flowLayoutPanel1.Controls.Add(this.btZoomOut);
            this.flowLayoutPanel1.Controls.Add(this.btProfile);
            this.flowLayoutPanel1.Controls.Add(this.btInsp);
            this.flowLayoutPanel1.Controls.Add(this.btMap3D);
            this.flowLayoutPanel1.Controls.Add(this.btGrabCam);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(685, 42);
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
            this.rbStratch.TabStop = true;
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
            this.rbRealRatio.TabStop = true;
            this.rbRealRatio.UseVisualStyleBackColor = true;
            this.rbRealRatio.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
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
            // btProfile
            // 
            this.btProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btProfile.BackgroundImage")));
            this.btProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btProfile.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btProfile.Location = new System.Drawing.Point(211, 1);
            this.btProfile.Margin = new System.Windows.Forms.Padding(1);
            this.btProfile.Name = "btProfile";
            this.btProfile.Size = new System.Drawing.Size(40, 40);
            this.btProfile.TabIndex = 21;
            this.btProfile.UseVisualStyleBackColor = true;
            this.btProfile.Click += new System.EventHandler(this.btProfile_Click);
            // 
            // btInsp
            // 
            this.btInsp.BackgroundImage = global::VDll.Properties.Resources.Play;
            this.btInsp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btInsp.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btInsp.Location = new System.Drawing.Point(253, 1);
            this.btInsp.Margin = new System.Windows.Forms.Padding(1);
            this.btInsp.Name = "btInsp";
            this.btInsp.Size = new System.Drawing.Size(40, 40);
            this.btInsp.TabIndex = 23;
            this.btInsp.UseVisualStyleBackColor = true;
            this.btInsp.Visible = false;
            // 
            // btMap3D
            // 
            this.btMap3D.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btMap3D.BackgroundImage")));
            this.btMap3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btMap3D.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btMap3D.Location = new System.Drawing.Point(295, 1);
            this.btMap3D.Margin = new System.Windows.Forms.Padding(1);
            this.btMap3D.Name = "btMap3D";
            this.btMap3D.Size = new System.Drawing.Size(40, 40);
            this.btMap3D.TabIndex = 20;
            this.btMap3D.UseVisualStyleBackColor = true;
            this.btMap3D.Visible = false;
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
            this.btGrabCam.UseVisualStyleBackColor = true;
            this.btGrabCam.Visible = false;
            // 
            // pnColor
            // 
            this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnColor.Controls.Add(this.lbPixel);
            this.pnColor.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnColor.Location = new System.Drawing.Point(685, 0);
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
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // tmLive
            // 
            this.tmLive.Interval = 1;
            this.tmLive.Tick += new System.EventHandler(this.tmLive_Tick);
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
            this.rbFit.TabIndex = 27;
            this.rbFit.TabStop = true;
            this.rbFit.UseVisualStyleBackColor = true;
            this.rbFit.CheckedChanged += new System.EventHandler(this.rbStratch_CheckedChanged);
            // 
            // FormTrain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 985);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnControlBase);
            this.Name = "FormTrain";
            this.Text = "FormTrain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTrain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTrain_FormClosed);
            this.Load += new System.EventHandler(this.FormTrain_Load);
            this.Shown += new System.EventHandler(this.FormTrain_Shown);
            this.Resize += new System.EventHandler(this.FormTrain_Resize);
            this.pnControlBase.ResumeLayout(false);
            this.pnffgff.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.pnCamera.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ibTrainCam)).EndInit();
            this.paCamTop.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnColor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ibTrainCam;
        private System.Windows.Forms.Panel pnControlBase;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView lvPkg;
        private System.Windows.Forms.Panel pnffgff;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button btGlobalValue;
        private System.Windows.Forms.Button btLocalValue;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel pnCamera;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.Button btPre;
        private System.Windows.Forms.PropertyGrid pgPara;
        private System.Windows.Forms.RadioButton rbMasterPara;
        private System.Windows.Forms.RadioButton rbUserPara;
        private System.Windows.Forms.Button btPkgEdit;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Timer tmLive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel paCamTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rbStratch;
        private System.Windows.Forms.RadioButton rbRealRatio;
        private System.Windows.Forms.Button btZoomIn;
        private System.Windows.Forms.Button btZoomOut;
        private System.Windows.Forms.Button btLoadImg;
        private System.Windows.Forms.Button btSaveImg;
        private System.Windows.Forms.Button btGrabCam;
        private System.Windows.Forms.Button btMap3D;
        private System.Windows.Forms.Button btProfile;
        private System.Windows.Forms.Button btInsp;
        private System.Windows.Forms.Panel pnColor;
        private System.Windows.Forms.Label lbPixel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox cbLiveCam;
        private System.Windows.Forms.Button btCapture;
        private System.Windows.Forms.RadioButton rbFit;
    }
}