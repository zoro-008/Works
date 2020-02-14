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
            this.lbName = new System.Windows.Forms.Label();
            this.lbStat = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbDevice = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbLotNo = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.pnBase = new System.Windows.Forms.Panel();
            this.pnMenu = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btExit = new System.Windows.Forms.Button();
            this.btUtil = new System.Windows.Forms.Button();
            this.btOption = new System.Windows.Forms.Button();
            this.btSpc = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.pnMainTitle.SuspendLayout();
            this.pnMenu.SuspendLayout();
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
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("Segoe UI Symbol", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(106, 9);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(201, 54);
            this.lbName.TabIndex = 6;
            this.lbName.Text = "HRT_460t";
            this.lbName.Click += new System.EventHandler(this.lbName_Click);
            // 
            // lbStat
            // 
            this.lbStat.AutoSize = true;
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            this.lbStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat.Location = new System.Drawing.Point(561, 31);
            this.lbStat.Name = "lbStat";
            this.lbStat.Size = new System.Drawing.Size(158, 55);
            this.lbStat.TabIndex = 7;
            this.lbStat.Text = "STOP";
            this.lbStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(110, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 28);
            this.label1.TabIndex = 8;
            this.label1.Text = "Device :";
            // 
            // lbDevice
            // 
            this.lbDevice.AutoSize = true;
            this.lbDevice.BackColor = System.Drawing.Color.Transparent;
            this.lbDevice.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDevice.Location = new System.Drawing.Point(203, 74);
            this.lbDevice.Name = "lbDevice";
            this.lbDevice.Size = new System.Drawing.Size(60, 28);
            this.lbDevice.TabIndex = 9;
            this.lbDevice.Text = "5630";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(850, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 28);
            this.label3.TabIndex = 10;
            this.label3.Text = "LotNo:";
            // 
            // lbLotNo
            // 
            this.lbLotNo.AutoSize = true;
            this.lbLotNo.BackColor = System.Drawing.Color.Transparent;
            this.lbLotNo.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLotNo.Location = new System.Drawing.Point(937, 74);
            this.lbLotNo.Name = "lbLotNo";
            this.lbLotNo.Size = new System.Drawing.Size(107, 28);
            this.lbLotNo.TabIndex = 11;
            this.lbLotNo.Text = "1123-sssk";
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            this.lbMainThreadTime.Location = new System.Drawing.Point(3, 3);
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            this.lbMainThreadTime.Size = new System.Drawing.Size(66, 17);
            this.lbMainThreadTime.TabIndex = 12;
            this.lbMainThreadTime.Text = "label2";
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnMainTitle.BackgroundImage")));
            this.pnMainTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMainTitle.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle.Controls.Add(this.lbLotNo);
            this.pnMainTitle.Controls.Add(this.label3);
            this.pnMainTitle.Controls.Add(this.lbDevice);
            this.pnMainTitle.Controls.Add(this.label1);
            this.pnMainTitle.Controls.Add(this.lbStat);
            this.pnMainTitle.Controls.Add(this.lbName);
            this.pnMainTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMainTitle.Location = new System.Drawing.Point(0, 0);
            this.pnMainTitle.Name = "pnMainTitle";
            this.pnMainTitle.Size = new System.Drawing.Size(1280, 105);
            this.pnMainTitle.TabIndex = 0;
            // 
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnBase.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnBase.Location = new System.Drawing.Point(0, 105);
            this.pnBase.Name = "pnBase";
            this.pnBase.Size = new System.Drawing.Size(1280, 863);
            this.pnBase.TabIndex = 3;
            // 
            // pnMenu
            // 
            this.pnMenu.BackColor = System.Drawing.SystemColors.InactiveCaption;
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
            this.pnMenu.TabIndex = 4;
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
            this.btExit.Location = new System.Drawing.Point(1068, 3);
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
            this.btUtil.Location = new System.Drawing.Point(855, 3);
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
            this.btSpc.Location = new System.Drawing.Point(642, 3);
            this.btSpc.Name = "btSpc";
            this.btSpc.Size = new System.Drawing.Size(210, 50);
            this.btSpc.TabIndex = 3;
            this.btSpc.Tag = "4";
            this.btSpc.Text = "SPC";
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
            this.btDevice.Location = new System.Drawing.Point(216, 3);
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
            this.btOperation.Location = new System.Drawing.Point(3, 3);
            this.btOperation.Name = "btOperation";
            this.btOperation.Size = new System.Drawing.Size(210, 50);
            this.btOperation.TabIndex = 0;
            this.btOperation.Tag = "0";
            this.btOperation.Text = "OPERATION";
            this.btOperation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOperation.UseVisualStyleBackColor = false;
            this.btOperation.Click += new System.EventHandler(this.btOperation_Click);
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
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.pnMainTitle.ResumeLayout(false);
            this.pnMainTitle.PerformLayout();
            this.pnMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem Close;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbLotNo;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Panel pnBase;
        private System.Windows.Forms.Panel pnMenu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btUtil;
        private System.Windows.Forms.Button btOption;
        private System.Windows.Forms.Button btSpc;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.Button btOperation;


    }
}

