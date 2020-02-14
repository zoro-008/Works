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
            this.ibTrainCam = new Emgu.CV.UI.ImageBox();
            this.pnTitle = new System.Windows.Forms.Panel();
            this.btFitView = new System.Windows.Forms.Button();
            this.pnColor = new System.Windows.Forms.Panel();
            this.lbPixel = new System.Windows.Forms.Label();
            this.lbMousePoint = new System.Windows.Forms.Label();
            this.btCalibration = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.btRealSize = new System.Windows.Forms.Button();
            this.btZoomIn = new System.Windows.Forms.Button();
            this.btZoomOut = new System.Windows.Forms.Button();
            this.pnControlBase = new System.Windows.Forms.Panel();
            this.pgPara = new System.Windows.Forms.PropertyGrid();
            this.pnffgff = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.pnParaSel = new System.Windows.Forms.Panel();
            this.rbMasterPara = new System.Windows.Forms.RadioButton();
            this.rbUserPara = new System.Windows.Forms.RadioButton();
            this.btGlobalValue = new System.Windows.Forms.Button();
            this.btLocalValue = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.btSave = new System.Windows.Forms.Button();
            this.pnCamera = new System.Windows.Forms.Panel();
            this.btSetting = new System.Windows.Forms.Button();
            this.btTrain = new System.Windows.Forms.Button();
            this.btSaveImg = new System.Windows.Forms.Button();
            this.btLoadImg = new System.Windows.Forms.Button();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btPkgEdit = new System.Windows.Forms.Button();
            this.btNext = new System.Windows.Forms.Button();
            this.btPre = new System.Windows.Forms.Button();
            this.lvPkg = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmLive = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ibTrainCam)).BeginInit();
            this.pnTitle.SuspendLayout();
            this.pnColor.SuspendLayout();
            this.pnControlBase.SuspendLayout();
            this.pnffgff.SuspendLayout();
            this.panel10.SuspendLayout();
            this.pnParaSel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel12.SuspendLayout();
            this.pnCamera.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ibTrainCam
            // 
            this.ibTrainCam.BackColor = System.Drawing.Color.DimGray;
            this.ibTrainCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibTrainCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibTrainCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibTrainCam.Location = new System.Drawing.Point(0, 23);
            this.ibTrainCam.Name = "ibTrainCam";
            this.ibTrainCam.Size = new System.Drawing.Size(935, 962);
            this.ibTrainCam.TabIndex = 4;
            this.ibTrainCam.TabStop = false;
            this.ibTrainCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibTrainCam_Paint);
            this.ibTrainCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseDown);
            this.ibTrainCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseMove);
            this.ibTrainCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibTrainCam_MouseUp);
            // 
            // pnTitle
            // 
            this.pnTitle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnTitle.Controls.Add(this.btFitView);
            this.pnTitle.Controls.Add(this.pnColor);
            this.pnTitle.Controls.Add(this.lbMousePoint);
            this.pnTitle.Controls.Add(this.btCalibration);
            this.pnTitle.Controls.Add(this.button1);
            this.pnTitle.Controls.Add(this.btLoad);
            this.pnTitle.Controls.Add(this.btRealSize);
            this.pnTitle.Controls.Add(this.btZoomIn);
            this.pnTitle.Controls.Add(this.btZoomOut);
            this.pnTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTitle.Location = new System.Drawing.Point(0, 0);
            this.pnTitle.Name = "pnTitle";
            this.pnTitle.Size = new System.Drawing.Size(935, 23);
            this.pnTitle.TabIndex = 3;
            // 
            // btFitView
            // 
            this.btFitView.Location = new System.Drawing.Point(221, 0);
            this.btFitView.Name = "btFitView";
            this.btFitView.Size = new System.Drawing.Size(67, 23);
            this.btFitView.TabIndex = 23;
            this.btFitView.Text = "FitView";
            this.btFitView.UseVisualStyleBackColor = true;
            this.btFitView.Click += new System.EventHandler(this.btFitView_Click);
            // 
            // pnColor
            // 
            this.pnColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnColor.Controls.Add(this.lbPixel);
            this.pnColor.Location = new System.Drawing.Point(728, 1);
            this.pnColor.Name = "pnColor";
            this.pnColor.Size = new System.Drawing.Size(207, 21);
            this.pnColor.TabIndex = 22;
            // 
            // lbPixel
            // 
            this.lbPixel.AutoSize = true;
            this.lbPixel.Location = new System.Drawing.Point(4, 4);
            this.lbPixel.Name = "lbPixel";
            this.lbPixel.Size = new System.Drawing.Size(203, 12);
            this.lbPixel.TabIndex = 0;
            this.lbPixel.Text = "<2048,2048>(2048,2048)=255,255,255";
            this.lbPixel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMousePoint
            // 
            this.lbMousePoint.AutoSize = true;
            this.lbMousePoint.Location = new System.Drawing.Point(692, 5);
            this.lbMousePoint.Name = "lbMousePoint";
            this.lbMousePoint.Size = new System.Drawing.Size(38, 12);
            this.lbMousePoint.TabIndex = 21;
            this.lbMousePoint.Text = "label1";
            // 
            // btCalibration
            // 
            this.btCalibration.Location = new System.Drawing.Point(606, 0);
            this.btCalibration.Name = "btCalibration";
            this.btCalibration.Size = new System.Drawing.Size(83, 23);
            this.btCalibration.TabIndex = 20;
            this.btCalibration.Text = "Calibration";
            this.btCalibration.UseVisualStyleBackColor = true;
            this.btCalibration.Click += new System.EventHandler(this.btCalibration_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(534, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(475, 0);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(53, 23);
            this.btLoad.TabIndex = 17;
            this.btLoad.Text = "Load";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // btRealSize
            // 
            this.btRealSize.Location = new System.Drawing.Point(148, 0);
            this.btRealSize.Name = "btRealSize";
            this.btRealSize.Size = new System.Drawing.Size(67, 23);
            this.btRealSize.TabIndex = 14;
            this.btRealSize.Text = "RealSize";
            this.btRealSize.UseVisualStyleBackColor = true;
            this.btRealSize.Click += new System.EventHandler(this.btRealSize_Click);
            // 
            // btZoomIn
            // 
            this.btZoomIn.Location = new System.Drawing.Point(79, 0);
            this.btZoomIn.Name = "btZoomIn";
            this.btZoomIn.Size = new System.Drawing.Size(63, 23);
            this.btZoomIn.TabIndex = 13;
            this.btZoomIn.Text = "Zoom In";
            this.btZoomIn.UseVisualStyleBackColor = true;
            this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
            // 
            // btZoomOut
            // 
            this.btZoomOut.Location = new System.Drawing.Point(8, 0);
            this.btZoomOut.Name = "btZoomOut";
            this.btZoomOut.Size = new System.Drawing.Size(65, 23);
            this.btZoomOut.TabIndex = 12;
            this.btZoomOut.Text = "ZoomOut";
            this.btZoomOut.UseVisualStyleBackColor = true;
            this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
            // 
            // pnControlBase
            // 
            this.pnControlBase.Controls.Add(this.pgPara);
            this.pnControlBase.Controls.Add(this.pnffgff);
            this.pnControlBase.Controls.Add(this.panel12);
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
            this.pgPara.Location = new System.Drawing.Point(0, 337);
            this.pgPara.Name = "pgPara";
            this.pgPara.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgPara.Size = new System.Drawing.Size(329, 599);
            this.pgPara.TabIndex = 7;
            this.pgPara.ToolbarVisible = false;
            this.pgPara.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgPara_PropertyValueChanged);
            // 
            // pnffgff
            // 
            this.pnffgff.Controls.Add(this.panel10);
            this.pnffgff.Controls.Add(this.panel6);
            this.pnffgff.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnffgff.Location = new System.Drawing.Point(0, 260);
            this.pnffgff.Name = "pnffgff";
            this.pnffgff.Size = new System.Drawing.Size(329, 77);
            this.pnffgff.TabIndex = 6;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.SystemColors.Control;
            this.panel10.Controls.Add(this.pnParaSel);
            this.panel10.Controls.Add(this.btGlobalValue);
            this.panel10.Controls.Add(this.btLocalValue);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 23);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(329, 54);
            this.panel10.TabIndex = 7;
            // 
            // pnParaSel
            // 
            this.pnParaSel.Controls.Add(this.rbMasterPara);
            this.pnParaSel.Controls.Add(this.rbUserPara);
            this.pnParaSel.Location = new System.Drawing.Point(9, 6);
            this.pnParaSel.Name = "pnParaSel";
            this.pnParaSel.Size = new System.Drawing.Size(199, 45);
            this.pnParaSel.TabIndex = 16;
            // 
            // rbMasterPara
            // 
            this.rbMasterPara.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterPara.AutoSize = true;
            this.rbMasterPara.Location = new System.Drawing.Point(103, 10);
            this.rbMasterPara.Name = "rbMasterPara";
            this.rbMasterPara.Size = new System.Drawing.Size(80, 22);
            this.rbMasterPara.TabIndex = 1;
            this.rbMasterPara.Text = "MasterPara";
            this.rbMasterPara.UseVisualStyleBackColor = true;
            // 
            // rbUserPara
            // 
            this.rbUserPara.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbUserPara.AutoSize = true;
            this.rbUserPara.Checked = true;
            this.rbUserPara.Location = new System.Drawing.Point(18, 6);
            this.rbUserPara.Name = "rbUserPara";
            this.rbUserPara.Size = new System.Drawing.Size(67, 22);
            this.rbUserPara.TabIndex = 0;
            this.rbUserPara.TabStop = true;
            this.rbUserPara.Text = "UserPara";
            this.rbUserPara.UseVisualStyleBackColor = true;
            this.rbUserPara.CheckedChanged += new System.EventHandler(this.rbUserPara_CheckedChanged);
            // 
            // btGlobalValue
            // 
            this.btGlobalValue.Location = new System.Drawing.Point(274, 2);
            this.btGlobalValue.Name = "btGlobalValue";
            this.btGlobalValue.Size = new System.Drawing.Size(50, 50);
            this.btGlobalValue.TabIndex = 15;
            this.btGlobalValue.Text = "Global Value Table";
            this.btGlobalValue.UseVisualStyleBackColor = true;
            this.btGlobalValue.Click += new System.EventHandler(this.btGlobalValue_Click);
            // 
            // btLocalValue
            // 
            this.btLocalValue.Location = new System.Drawing.Point(214, 2);
            this.btLocalValue.Name = "btLocalValue";
            this.btLocalValue.Size = new System.Drawing.Size(50, 50);
            this.btLocalValue.TabIndex = 14;
            this.btLocalValue.Text = "ValueTable";
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
            this.panel6.Size = new System.Drawing.Size(329, 23);
            this.panel6.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(329, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "PARAMETER";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.SystemColors.Control;
            this.panel12.Controls.Add(this.btSave);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel12.Location = new System.Drawing.Point(0, 936);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(329, 49);
            this.panel12.TabIndex = 9;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(174, 3);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(143, 43);
            this.btSave.TabIndex = 13;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // pnCamera
            // 
            this.pnCamera.Controls.Add(this.btSetting);
            this.pnCamera.Controls.Add(this.btTrain);
            this.pnCamera.Controls.Add(this.btSaveImg);
            this.pnCamera.Controls.Add(this.btLoadImg);
            this.pnCamera.Controls.Add(this.panel9);
            this.pnCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCamera.Location = new System.Drawing.Point(0, 180);
            this.pnCamera.Name = "pnCamera";
            this.pnCamera.Size = new System.Drawing.Size(329, 80);
            this.pnCamera.TabIndex = 7;
            // 
            // btSetting
            // 
            this.btSetting.Location = new System.Drawing.Point(96, 26);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(88, 50);
            this.btSetting.TabIndex = 11;
            this.btSetting.Text = "SETTING";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // btTrain
            // 
            this.btTrain.Location = new System.Drawing.Point(3, 26);
            this.btTrain.Name = "btTrain";
            this.btTrain.Size = new System.Drawing.Size(91, 50);
            this.btTrain.TabIndex = 2;
            this.btTrain.Text = "TRAIN";
            this.btTrain.UseVisualStyleBackColor = true;
            this.btTrain.Click += new System.EventHandler(this.btTrain_Click);
            // 
            // btSaveImg
            // 
            this.btSaveImg.Location = new System.Drawing.Point(258, 26);
            this.btSaveImg.Name = "btSaveImg";
            this.btSaveImg.Size = new System.Drawing.Size(66, 50);
            this.btSaveImg.TabIndex = 10;
            this.btSaveImg.Text = "Save Img";
            this.btSaveImg.UseVisualStyleBackColor = true;
            // 
            // btLoadImg
            // 
            this.btLoadImg.Location = new System.Drawing.Point(190, 26);
            this.btLoadImg.Name = "btLoadImg";
            this.btLoadImg.Size = new System.Drawing.Size(66, 50);
            this.btLoadImg.TabIndex = 9;
            this.btLoadImg.Text = "Load Img";
            this.btLoadImg.UseVisualStyleBackColor = true;
            this.btLoadImg.Click += new System.EventHandler(this.btLoadImg_Click);
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel9.Controls.Add(this.label2);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(329, 23);
            this.panel9.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(329, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "CONTROL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Controls.Add(this.lvPkg);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(329, 180);
            this.panel4.TabIndex = 5;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel7.Controls.Add(this.btPkgEdit);
            this.panel7.Controls.Add(this.btNext);
            this.panel7.Controls.Add(this.btPre);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 146);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(329, 34);
            this.panel7.TabIndex = 6;
            // 
            // btPkgEdit
            // 
            this.btPkgEdit.Location = new System.Drawing.Point(62, 3);
            this.btPkgEdit.Name = "btPkgEdit";
            this.btPkgEdit.Size = new System.Drawing.Size(74, 28);
            this.btPkgEdit.TabIndex = 8;
            this.btPkgEdit.Text = "Edit";
            this.btPkgEdit.UseVisualStyleBackColor = true;
            this.btPkgEdit.Click += new System.EventHandler(this.btPkgEdit_Click);
            // 
            // btNext
            // 
            this.btNext.Location = new System.Drawing.Point(268, 3);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(49, 28);
            this.btNext.TabIndex = 1;
            this.btNext.Text = "=>";
            this.btNext.UseVisualStyleBackColor = true;
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // btPre
            // 
            this.btPre.Location = new System.Drawing.Point(6, 3);
            this.btPre.Name = "btPre";
            this.btPre.Size = new System.Drawing.Size(49, 28);
            this.btPre.TabIndex = 0;
            this.btPre.Text = "<=";
            this.btPre.UseVisualStyleBackColor = true;
            this.btPre.Click += new System.EventHandler(this.btPre_Click);
            // 
            // lvPkg
            // 
            this.lvPkg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lvPkg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvPkg.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvPkg.FullRowSelect = true;
            this.lvPkg.Location = new System.Drawing.Point(2, 23);
            this.lvPkg.Name = "lvPkg";
            this.lvPkg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lvPkg.Size = new System.Drawing.Size(325, 120);
            this.lvPkg.TabIndex = 5;
            this.lvPkg.UseCompatibleStateImageBehavior = false;
            this.lvPkg.SelectedIndexChanged += new System.EventHandler(this.lvPkg_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(329, 23);
            this.panel3.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "PACKAGE LIST";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ibTrainCam);
            this.panel2.Controls.Add(this.pnTitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(935, 985);
            this.panel2.TabIndex = 6;
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
            this.Shown += new System.EventHandler(this.FormTrain_Shown);
            this.Resize += new System.EventHandler(this.FormTrain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ibTrainCam)).EndInit();
            this.pnTitle.ResumeLayout(false);
            this.pnTitle.PerformLayout();
            this.pnColor.ResumeLayout(false);
            this.pnColor.PerformLayout();
            this.pnControlBase.ResumeLayout(false);
            this.pnffgff.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.pnParaSel.ResumeLayout(false);
            this.pnParaSel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.pnCamera.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ibTrainCam;
        private System.Windows.Forms.Panel pnTitle;
        private System.Windows.Forms.Panel pnControlBase;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView lvPkg;
        private System.Windows.Forms.Panel pnffgff;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button btGlobalValue;
        private System.Windows.Forms.Button btLocalValue;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel pnCamera;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Button btSaveImg;
        private System.Windows.Forms.Button btLoadImg;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btTrain;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.Button btPre;
        private System.Windows.Forms.PropertyGrid pgPara;
        private System.Windows.Forms.Panel pnParaSel;
        private System.Windows.Forms.RadioButton rbMasterPara;
        private System.Windows.Forms.RadioButton rbUserPara;
        private System.Windows.Forms.Button btPkgEdit;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Timer tmLive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btRealSize;
        private System.Windows.Forms.Button btZoomIn;
        private System.Windows.Forms.Button btZoomOut;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Button btCalibration;
        private System.Windows.Forms.Panel pnColor;
        private System.Windows.Forms.Label lbPixel;
        private System.Windows.Forms.Label lbMousePoint;
        private System.Windows.Forms.Button btFitView;
    }
}