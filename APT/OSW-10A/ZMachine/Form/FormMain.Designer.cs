namespace Machine
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.Close = new System.Windows.Forms.ToolStripMenuItem();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnBase = new System.Windows.Forms.Panel();
            this.pnMenu = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btExit = new System.Windows.Forms.Button();
            this.btUtil = new System.Windows.Forms.Button();
            this.btOption = new System.Windows.Forms.Button();
            this.btSpc = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.pnDBNotConnect = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pnGoldenDevice = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnIdleRunning = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnUnderRepair = new System.Windows.Forms.Panel();
            this.lbUnderRepair = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.lbLotNo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbDevice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbStat = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.pnMenu.SuspendLayout();
            this.pnMainTitle.SuspendLayout();
            this.pnDBNotConnect.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnGoldenDevice.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnIdleRunning.SuspendLayout();
            this.pnUnderRepair.SuspendLayout();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(103, 22);
            this.Close.Text = "Close";
            // 
            // tmUpdate
            // 
            this.tmUpdate.Interval = 1;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // pnBase
            // 
            this.pnBase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnBase.BackColor = System.Drawing.Color.MistyRose;
            this.pnBase.Location = new System.Drawing.Point(0, 103);
            this.pnBase.Name = "pnBase";
            this.pnBase.Size = new System.Drawing.Size(1280, 866);
            this.pnBase.TabIndex = 27;
            // 
            // pnMenu
            // 
            this.pnMenu.BackColor = System.Drawing.Color.MistyRose;
            this.pnMenu.Controls.Add(this.splitter1);
            this.pnMenu.Controls.Add(this.btExit);
            this.pnMenu.Controls.Add(this.btUtil);
            this.pnMenu.Controls.Add(this.btOption);
            this.pnMenu.Controls.Add(this.btSpc);
            this.pnMenu.Controls.Add(this.btDevice);
            this.pnMenu.Controls.Add(this.btOperation);
            this.pnMenu.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnMenu.Location = new System.Drawing.Point(0, 968);
            this.pnMenu.Name = "pnMenu";
            this.pnMenu.Size = new System.Drawing.Size(1280, 56);
            this.pnMenu.TabIndex = 36;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 56);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // btExit
            // 
            this.btExit.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btExit.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btExit.Image = ((System.Drawing.Image)(resources.GetObject("btExit.Image")));
            this.btExit.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btExit.Location = new System.Drawing.Point(1065, 3);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(210, 50);
            this.btExit.TabIndex = 5;
            this.btExit.Tag = "6";
            this.btExit.Text = "EXIT";
            this.btExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btExit.UseVisualStyleBackColor = false;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btUtil
            // 
            this.btUtil.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btUtil.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btUtil.Image = ((System.Drawing.Image)(resources.GetObject("btUtil.Image")));
            this.btUtil.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btUtil.Location = new System.Drawing.Point(853, 3);
            this.btUtil.Name = "btUtil";
            this.btUtil.Size = new System.Drawing.Size(210, 50);
            this.btUtil.TabIndex = 4;
            this.btUtil.Tag = "5";
            this.btUtil.Text = "UTIL";
            this.btUtil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btUtil.UseVisualStyleBackColor = false;
            this.btUtil.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btOption
            // 
            this.btOption.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btOption.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btOption.Image = ((System.Drawing.Image)(resources.GetObject("btOption.Image")));
            this.btOption.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btOption.Location = new System.Drawing.Point(429, 3);
            this.btOption.Name = "btOption";
            this.btOption.Size = new System.Drawing.Size(210, 50);
            this.btOption.TabIndex = 2;
            this.btOption.Tag = "3";
            this.btOption.Text = "OPTION";
            this.btOption.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOption.UseVisualStyleBackColor = false;
            this.btOption.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btSpc
            // 
            this.btSpc.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btSpc.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSpc.Image = ((System.Drawing.Image)(resources.GetObject("btSpc.Image")));
            this.btSpc.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSpc.Location = new System.Drawing.Point(641, 3);
            this.btSpc.Name = "btSpc";
            this.btSpc.Size = new System.Drawing.Size(210, 50);
            this.btSpc.TabIndex = 3;
            this.btSpc.Tag = "4";
            this.btSpc.Text = "REPORT";
            this.btSpc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSpc.UseVisualStyleBackColor = false;
            this.btSpc.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btDevice
            // 
            this.btDevice.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btDevice.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btDevice.Image = ((System.Drawing.Image)(resources.GetObject("btDevice.Image")));
            this.btDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDevice.Location = new System.Drawing.Point(217, 3);
            this.btDevice.Name = "btDevice";
            this.btDevice.Size = new System.Drawing.Size(210, 50);
            this.btDevice.TabIndex = 1;
            this.btDevice.Tag = "2";
            this.btDevice.Text = "DEVICE";
            this.btDevice.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDevice.UseVisualStyleBackColor = false;
            this.btDevice.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btOperation
            // 
            this.btOperation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btOperation.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btOperation.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btOperation.Image = ((System.Drawing.Image)(resources.GetObject("btOperation.Image")));
            this.btOperation.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btOperation.Location = new System.Drawing.Point(5, 3);
            this.btOperation.Name = "btOperation";
            this.btOperation.Size = new System.Drawing.Size(210, 50);
            this.btOperation.TabIndex = 0;
            this.btOperation.Tag = "0";
            this.btOperation.Text = "OPERATION";
            this.btOperation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOperation.UseVisualStyleBackColor = false;
            this.btOperation.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnMainTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnMainTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMainTitle.Controls.Add(this.pnDBNotConnect);
            this.pnMainTitle.Controls.Add(this.pnGoldenDevice);
            this.pnMainTitle.Controls.Add(this.panel2);
            this.pnMainTitle.Controls.Add(this.panel1);
            this.pnMainTitle.Controls.Add(this.pnIdleRunning);
            this.pnMainTitle.Controls.Add(this.pnUnderRepair);
            this.pnMainTitle.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle.Controls.Add(this.lbLotNo);
            this.pnMainTitle.Controls.Add(this.label3);
            this.pnMainTitle.Controls.Add(this.lbDevice);
            this.pnMainTitle.Controls.Add(this.label1);
            this.pnMainTitle.Controls.Add(this.lbStat);
            this.pnMainTitle.Controls.Add(this.lbName);
            this.pnMainTitle.Location = new System.Drawing.Point(1, 1);
            this.pnMainTitle.Name = "pnMainTitle";
            this.pnMainTitle.Size = new System.Drawing.Size(1277, 105);
            this.pnMainTitle.TabIndex = 37;
            // 
            // pnDBNotConnect
            // 
            this.pnDBNotConnect.BackColor = System.Drawing.Color.Red;
            this.pnDBNotConnect.Controls.Add(this.panel5);
            this.pnDBNotConnect.Controls.Add(this.label7);
            this.pnDBNotConnect.Location = new System.Drawing.Point(770, 36);
            this.pnDBNotConnect.Name = "pnDBNotConnect";
            this.pnDBNotConnect.Size = new System.Drawing.Size(177, 33);
            this.pnDBNotConnect.TabIndex = 43;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Red;
            this.panel5.Controls.Add(this.label6);
            this.panel5.Location = new System.Drawing.Point(0, 33);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(177, 33);
            this.panel5.TabIndex = 43;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(6, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "GOLDEN DEVICE";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(6, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "DB Not Connect";
            // 
            // pnGoldenDevice
            // 
            this.pnGoldenDevice.BackColor = System.Drawing.Color.Red;
            this.pnGoldenDevice.Controls.Add(this.panel3);
            this.pnGoldenDevice.Controls.Add(this.label4);
            this.pnGoldenDevice.Location = new System.Drawing.Point(770, 3);
            this.pnGoldenDevice.Name = "pnGoldenDevice";
            this.pnGoldenDevice.Size = new System.Drawing.Size(177, 33);
            this.pnGoldenDevice.TabIndex = 42;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Red;
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(0, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(177, 33);
            this.panel3.TabIndex = 43;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(6, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 20);
            this.label5.TabIndex = 17;
            this.label5.Text = "GOLDEN DEVICE";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "GOLDEN DEVICE";
            // 
            // lbVer
            // 
            this.lbVer.AutoSize = true;
            this.lbVer.BackColor = System.Drawing.Color.Transparent;
            this.lbVer.Font = new System.Drawing.Font("Segoe UI Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVer.Location = new System.Drawing.Point(106, 6);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(85, 19);
            this.lbVer.TabIndex = 41;
            this.lbVer.Text = "VER : 1.1.1.1";
            this.lbVer.DoubleClick += new System.EventHandler(this.lbVer_DoubleClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(1111, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(165, 66);
            this.panel2.TabIndex = 40;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.BackgroundImage = global::Machine.Properties.Resources.AP_Tech_Image;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lbVer);
            this.panel1.Location = new System.Drawing.Point(0, -3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(287, 108);
            this.panel1.TabIndex = 39;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // pnIdleRunning
            // 
            this.pnIdleRunning.BackColor = System.Drawing.Color.Red;
            this.pnIdleRunning.Controls.Add(this.label2);
            this.pnIdleRunning.Location = new System.Drawing.Point(947, 3);
            this.pnIdleRunning.Name = "pnIdleRunning";
            this.pnIdleRunning.Size = new System.Drawing.Size(164, 33);
            this.pnIdleRunning.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 20);
            this.label2.TabIndex = 17;
            this.label2.Text = "IDLE RUNNING";
            // 
            // pnUnderRepair
            // 
            this.pnUnderRepair.BackColor = System.Drawing.Color.Red;
            this.pnUnderRepair.Controls.Add(this.lbUnderRepair);
            this.pnUnderRepair.Location = new System.Drawing.Point(1111, 3);
            this.pnUnderRepair.Name = "pnUnderRepair";
            this.pnUnderRepair.Size = new System.Drawing.Size(164, 33);
            this.pnUnderRepair.TabIndex = 17;
            // 
            // lbUnderRepair
            // 
            this.lbUnderRepair.AutoSize = true;
            this.lbUnderRepair.BackColor = System.Drawing.Color.Transparent;
            this.lbUnderRepair.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbUnderRepair.ForeColor = System.Drawing.Color.White;
            this.lbUnderRepair.Location = new System.Drawing.Point(6, 6);
            this.lbUnderRepair.Name = "lbUnderRepair";
            this.lbUnderRepair.Size = new System.Drawing.Size(153, 20);
            this.lbUnderRepair.TabIndex = 17;
            this.lbUnderRepair.Text = "UNDER REPAIR";
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            this.lbMainThreadTime.Location = new System.Drawing.Point(2, 85);
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            this.lbMainThreadTime.Size = new System.Drawing.Size(66, 17);
            this.lbMainThreadTime.TabIndex = 12;
            this.lbMainThreadTime.Text = "label2";
            // 
            // lbLotNo
            // 
            this.lbLotNo.AutoSize = true;
            this.lbLotNo.BackColor = System.Drawing.Color.Transparent;
            this.lbLotNo.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLotNo.Location = new System.Drawing.Point(985, 74);
            this.lbLotNo.Name = "lbLotNo";
            this.lbLotNo.Size = new System.Drawing.Size(107, 28);
            this.lbLotNo.TabIndex = 11;
            this.lbLotNo.Text = "1123-sssk";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(898, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 28);
            this.label3.TabIndex = 10;
            this.label3.Text = "LotNo:";
            // 
            // lbDevice
            // 
            this.lbDevice.AutoSize = true;
            this.lbDevice.BackColor = System.Drawing.Color.Transparent;
            this.lbDevice.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDevice.Location = new System.Drawing.Point(381, 73);
            this.lbDevice.Name = "lbDevice";
            this.lbDevice.Size = new System.Drawing.Size(60, 28);
            this.lbDevice.TabIndex = 9;
            this.lbDevice.Text = "5630";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(296, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 28);
            this.label1.TabIndex = 8;
            this.label1.Text = "Device :";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbStat
            // 
            this.lbStat.AutoSize = true;
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            this.lbStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat.Location = new System.Drawing.Point(611, 23);
            this.lbStat.Name = "lbStat";
            this.lbStat.Size = new System.Drawing.Size(158, 55);
            this.lbStat.TabIndex = 7;
            this.lbStat.Text = "STOP";
            this.lbStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("Segoe UI Symbol", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(292, 4);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(201, 54);
            this.lbName.TabIndex = 6;
            this.lbName.Text = "HRT_460t";
            this.lbName.Click += new System.EventHandler(this.lbName_Click);
            this.lbName.DoubleClick += new System.EventHandler(this.lbName_DoubleClick_1);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 1024);
            this.Controls.Add(this.pnBase);
            this.Controls.Add(this.pnMainTitle);
            this.Controls.Add(this.pnMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Machine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.pnMenu.ResumeLayout(false);
            this.pnMainTitle.ResumeLayout(false);
            this.pnMainTitle.PerformLayout();
            this.pnDBNotConnect.ResumeLayout(false);
            this.pnDBNotConnect.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnGoldenDevice.ResumeLayout(false);
            this.pnGoldenDevice.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnIdleRunning.ResumeLayout(false);
            this.pnIdleRunning.PerformLayout();
            this.pnUnderRepair.ResumeLayout(false);
            this.pnUnderRepair.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem Close;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel pnBase;
        private System.Windows.Forms.Panel pnMenu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btUtil;
        private System.Windows.Forms.Button btOption;
        private System.Windows.Forms.Button btSpc;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.Button btOperation;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnIdleRunning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnUnderRepair;
        private System.Windows.Forms.Label lbUnderRepair;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Label lbLotNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Panel pnGoldenDevice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnDBNotConnect;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;


    }
}

