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
            this.lbAtDegree = new System.Windows.Forms.Label();
            this.lbBfDegree = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbRTorque = new System.Windows.Forms.Label();
            this.lbLTorque = new System.Windows.Forms.Label();
            this.lbVisnCnt = new System.Windows.Forms.Label();
            this.pnBase = new System.Windows.Forms.Panel();
            this.pnMenu = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btExit = new System.Windows.Forms.Button();
            this.btUtil = new System.Windows.Forms.Button();
            this.btOption = new System.Windows.Forms.Button();
            this.btSpc = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.lbMovePos = new System.Windows.Forms.Label();
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
            this.lbMainThreadTime.Location = new System.Drawing.Point(2, 85);
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            this.lbMainThreadTime.Size = new System.Drawing.Size(66, 17);
            this.lbMainThreadTime.TabIndex = 12;
            this.lbMainThreadTime.Text = "label2";
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnMainTitle.BackgroundImage")));
            this.pnMainTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMainTitle.Controls.Add(this.lbMovePos);
            this.pnMainTitle.Controls.Add(this.lbAtDegree);
            this.pnMainTitle.Controls.Add(this.lbBfDegree);
            this.pnMainTitle.Controls.Add(this.label5);
            this.pnMainTitle.Controls.Add(this.button2);
            this.pnMainTitle.Controls.Add(this.label4);
            this.pnMainTitle.Controls.Add(this.label2);
            this.pnMainTitle.Controls.Add(this.lbRTorque);
            this.pnMainTitle.Controls.Add(this.lbLTorque);
            this.pnMainTitle.Controls.Add(this.lbVisnCnt);
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
            // lbAtDegree
            // 
            this.lbAtDegree.BackColor = System.Drawing.Color.Transparent;
            this.lbAtDegree.Location = new System.Drawing.Point(853, 39);
            this.lbAtDegree.Name = "lbAtDegree";
            this.lbAtDegree.Size = new System.Drawing.Size(102, 18);
            this.lbAtDegree.TabIndex = 21;
            this.lbAtDegree.Text = "label6";
            // 
            // lbBfDegree
            // 
            this.lbBfDegree.BackColor = System.Drawing.Color.Transparent;
            this.lbBfDegree.Location = new System.Drawing.Point(853, 21);
            this.lbBfDegree.Name = "lbBfDegree";
            this.lbBfDegree.Size = new System.Drawing.Size(102, 18);
            this.lbBfDegree.TabIndex = 20;
            this.lbBfDegree.Text = "label7";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(335, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 18);
            this.label5.TabIndex = 19;
            this.label5.Text = "label5";
            this.label5.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(338, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(335, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "label4";
            this.label4.Visible = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(335, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 18);
            this.label2.TabIndex = 16;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // lbRTorque
            // 
            this.lbRTorque.BackColor = System.Drawing.Color.Transparent;
            this.lbRTorque.Location = new System.Drawing.Point(2, 66);
            this.lbRTorque.Name = "lbRTorque";
            this.lbRTorque.Size = new System.Drawing.Size(102, 18);
            this.lbRTorque.TabIndex = 15;
            this.lbRTorque.Text = "lbRTorque";
            // 
            // lbLTorque
            // 
            this.lbLTorque.BackColor = System.Drawing.Color.Transparent;
            this.lbLTorque.Location = new System.Drawing.Point(2, 48);
            this.lbLTorque.Name = "lbLTorque";
            this.lbLTorque.Size = new System.Drawing.Size(102, 18);
            this.lbLTorque.TabIndex = 14;
            this.lbLTorque.Text = "lbLTorque";
            // 
            // lbVisnCnt
            // 
            this.lbVisnCnt.BackColor = System.Drawing.Color.Transparent;
            this.lbVisnCnt.Location = new System.Drawing.Point(3, 3);
            this.lbVisnCnt.Name = "lbVisnCnt";
            this.lbVisnCnt.Size = new System.Drawing.Size(234, 18);
            this.lbVisnCnt.TabIndex = 13;
            this.lbVisnCnt.Text = "label2";
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
            this.pnMenu.Controls.Add(this.button1);
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
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.Location = new System.Drawing.Point(187, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 50);
            this.button1.TabIndex = 7;
            this.button1.Tag = "1";
            this.button1.Text = "VISION";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btOperation_Click);
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
            this.btExit.Location = new System.Drawing.Point(1097, 3);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(180, 50);
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
            this.btUtil.Location = new System.Drawing.Point(915, 3);
            this.btUtil.Name = "btUtil";
            this.btUtil.Size = new System.Drawing.Size(180, 50);
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
            this.btOption.Location = new System.Drawing.Point(551, 3);
            this.btOption.Name = "btOption";
            this.btOption.Size = new System.Drawing.Size(180, 50);
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
            this.btSpc.Location = new System.Drawing.Point(733, 3);
            this.btSpc.Name = "btSpc";
            this.btSpc.Size = new System.Drawing.Size(180, 50);
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
            this.btDevice.Location = new System.Drawing.Point(369, 3);
            this.btDevice.Name = "btDevice";
            this.btDevice.Size = new System.Drawing.Size(180, 50);
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
            this.btOperation.Size = new System.Drawing.Size(180, 50);
            this.btOperation.TabIndex = 0;
            this.btOperation.Tag = "0";
            this.btOperation.Text = "OPERATION";
            this.btOperation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOperation.UseVisualStyleBackColor = false;
            this.btOperation.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // lbMovePos
            // 
            this.lbMovePos.BackColor = System.Drawing.Color.Transparent;
            this.lbMovePos.Location = new System.Drawing.Point(853, 56);
            this.lbMovePos.Name = "lbMovePos";
            this.lbMovePos.Size = new System.Drawing.Size(102, 18);
            this.lbMovePos.TabIndex = 22;
            this.lbMovePos.Text = "label6";
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Machine";
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbVisnCnt;
        private System.Windows.Forms.Label lbLTorque;
        private System.Windows.Forms.Label lbRTorque;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbAtDegree;
        private System.Windows.Forms.Label lbBfDegree;
        private System.Windows.Forms.Label lbMovePos;
    }
}

