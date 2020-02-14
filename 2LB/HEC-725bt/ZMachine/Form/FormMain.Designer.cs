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
            this.pnMenu = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btExit = new System.Windows.Forms.Button();
            this.btUtil = new System.Windows.Forms.Button();
            this.btOption = new System.Windows.Forms.Button();
            this.btSpc = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbOptn2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lbOptn3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbStat = new System.Windows.Forms.Label();
            this.lbText2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbText1 = new System.Windows.Forms.Label();
            this.btMinimization = new System.Windows.Forms.Button();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.pnRewind = new System.Windows.Forms.Panel();
            this.lbOptn1 = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.pnBase = new System.Windows.Forms.Panel();
            this.pnMenu.SuspendLayout();
            this.pnMainTitle.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnRewind.SuspendLayout();
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
            this.pnMenu.Location = new System.Drawing.Point(0, 950);
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
            this.btExit.Font = new System.Drawing.Font("맑은 고딕", 16F);
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
            this.btUtil.Font = new System.Drawing.Font("맑은 고딕", 16F);
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
            this.btOption.Font = new System.Drawing.Font("맑은 고딕", 16F);
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
            this.btSpc.Font = new System.Drawing.Font("맑은 고딕", 16F);
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
            this.btDevice.Font = new System.Drawing.Font("맑은 고딕", 16F);
            this.btDevice.Image = ((System.Drawing.Image)(resources.GetObject("btDevice.Image")));
            this.btDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDevice.Location = new System.Drawing.Point(217, 3);
            this.btDevice.Name = "btDevice";
            this.btDevice.Size = new System.Drawing.Size(210, 50);
            this.btDevice.TabIndex = 1;
            this.btDevice.Tag = "1";
            this.btDevice.Text = "DEVICE";
            this.btDevice.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDevice.UseVisualStyleBackColor = false;
            this.btDevice.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btOperation
            // 
            this.btOperation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btOperation.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btOperation.Font = new System.Drawing.Font("맑은 고딕", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.pnMainTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnMainTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnMainTitle.BackgroundImage")));
            this.pnMainTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMainTitle.Controls.Add(this.panel3);
            this.pnMainTitle.Controls.Add(this.panel4);
            this.pnMainTitle.Controls.Add(this.panel2);
            this.pnMainTitle.Controls.Add(this.lbText2);
            this.pnMainTitle.Controls.Add(this.panel1);
            this.pnMainTitle.Controls.Add(this.lbText1);
            this.pnMainTitle.Controls.Add(this.btMinimization);
            this.pnMainTitle.Controls.Add(this.lbDate);
            this.pnMainTitle.Controls.Add(this.lbVer);
            this.pnMainTitle.Controls.Add(this.pnRewind);
            this.pnMainTitle.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle.Controls.Add(this.lbName);
            this.pnMainTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMainTitle.Location = new System.Drawing.Point(0, 0);
            this.pnMainTitle.Name = "pnMainTitle";
            this.pnMainTitle.Size = new System.Drawing.Size(1280, 105);
            this.pnMainTitle.TabIndex = 37;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Red;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lbOptn2);
            this.panel3.Location = new System.Drawing.Point(902, 37);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3);
            this.panel3.Size = new System.Drawing.Size(210, 33);
            this.panel3.TabIndex = 58;
            this.panel3.Visible = false;
            // 
            // lbOptn2
            // 
            this.lbOptn2.BackColor = System.Drawing.Color.Transparent;
            this.lbOptn2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOptn2.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lbOptn2.ForeColor = System.Drawing.Color.White;
            this.lbOptn2.Location = new System.Drawing.Point(3, 3);
            this.lbOptn2.Name = "lbOptn2";
            this.lbOptn2.Size = new System.Drawing.Size(202, 25);
            this.lbOptn2.TabIndex = 17;
            this.lbOptn2.Text = "IDLE RUNNING";
            this.lbOptn2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Red;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lbOptn3);
            this.panel4.Location = new System.Drawing.Point(902, 71);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(3);
            this.panel4.Size = new System.Drawing.Size(210, 33);
            this.panel4.TabIndex = 59;
            this.panel4.Visible = false;
            // 
            // lbOptn3
            // 
            this.lbOptn3.BackColor = System.Drawing.Color.Transparent;
            this.lbOptn3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOptn3.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lbOptn3.ForeColor = System.Drawing.Color.White;
            this.lbOptn3.Location = new System.Drawing.Point(3, 3);
            this.lbOptn3.Name = "lbOptn3";
            this.lbOptn3.Size = new System.Drawing.Size(202, 25);
            this.lbOptn3.TabIndex = 17;
            this.lbOptn3.Text = "IDLE RUNNING";
            this.lbOptn3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.lbStat);
            this.panel2.Location = new System.Drawing.Point(390, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(500, 84);
            this.panel2.TabIndex = 50;
            // 
            // lbStat
            // 
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            this.lbStat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStat.Font = new System.Drawing.Font("맑은 고딕", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat.Location = new System.Drawing.Point(0, 0);
            this.lbStat.Name = "lbStat";
            this.lbStat.Size = new System.Drawing.Size(500, 84);
            this.lbStat.TabIndex = 46;
            this.lbStat.Text = "DEBUG RUNNING";
            this.lbStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbText2
            // 
            this.lbText2.AutoSize = true;
            this.lbText2.BackColor = System.Drawing.Color.Transparent;
            this.lbText2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbText2.Location = new System.Drawing.Point(216, 80);
            this.lbText2.Name = "lbText2";
            this.lbText2.Size = new System.Drawing.Size(54, 18);
            this.lbText2.TabIndex = 57;
            this.lbText2.Text = "2L-BIO";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(102, 95);
            this.panel1.TabIndex = 56;
            this.panel1.Visible = false;
            // 
            // lbText1
            // 
            this.lbText1.AutoSize = true;
            this.lbText1.BackColor = System.Drawing.Color.Transparent;
            this.lbText1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbText1.Location = new System.Drawing.Point(116, 56);
            this.lbText1.Name = "lbText1";
            this.lbText1.Size = new System.Drawing.Size(219, 18);
            this.lbText1.TabIndex = 55;
            this.lbText1.Text = "꿈과 희망과 가치를 키우는 기업";
            this.lbText1.Visible = false;
            // 
            // btMinimization
            // 
            this.btMinimization.Location = new System.Drawing.Point(1256, 2);
            this.btMinimization.Name = "btMinimization";
            this.btMinimization.Size = new System.Drawing.Size(21, 19);
            this.btMinimization.TabIndex = 54;
            this.btMinimization.Text = "-";
            this.btMinimization.UseVisualStyleBackColor = true;
            this.btMinimization.Click += new System.EventHandler(this.btMinimization_Click_1);
            // 
            // lbDate
            // 
            this.lbDate.BackColor = System.Drawing.Color.Transparent;
            this.lbDate.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbDate.Location = new System.Drawing.Point(1128, 79);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(144, 19);
            this.lbDate.TabIndex = 52;
            this.lbDate.Text = "1983-11-08 12:35:33";
            // 
            // lbVer
            // 
            this.lbVer.BackColor = System.Drawing.Color.Transparent;
            this.lbVer.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVer.Location = new System.Drawing.Point(1128, 56);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(144, 19);
            this.lbVer.TabIndex = 51;
            this.lbVer.Text = "Version 1.0.0.2";
            this.lbVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnRewind
            // 
            this.pnRewind.BackColor = System.Drawing.Color.Red;
            this.pnRewind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRewind.Controls.Add(this.lbOptn1);
            this.pnRewind.Location = new System.Drawing.Point(902, 3);
            this.pnRewind.Name = "pnRewind";
            this.pnRewind.Padding = new System.Windows.Forms.Padding(3);
            this.pnRewind.Size = new System.Drawing.Size(210, 33);
            this.pnRewind.TabIndex = 50;
            this.pnRewind.Visible = false;
            // 
            // lbOptn1
            // 
            this.lbOptn1.BackColor = System.Drawing.Color.Transparent;
            this.lbOptn1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOptn1.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbOptn1.ForeColor = System.Drawing.Color.White;
            this.lbOptn1.Location = new System.Drawing.Point(3, 3);
            this.lbOptn1.Name = "lbOptn1";
            this.lbOptn1.Size = new System.Drawing.Size(202, 25);
            this.lbOptn1.TabIndex = 17;
            this.lbOptn1.Text = "되감기 모드";
            this.lbOptn1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            this.lbMainThreadTime.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbMainThreadTime.Location = new System.Drawing.Point(1128, 33);
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            this.lbMainThreadTime.Size = new System.Drawing.Size(144, 19);
            this.lbMainThreadTime.TabIndex = 49;
            this.lbMainThreadTime.Text = "12.123";
            this.lbMainThreadTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(111, 2);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(175, 46);
            this.lbName.TabIndex = 45;
            this.lbName.Text = "HRT_460t";
            this.lbName.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbName_MouseDoubleClick);
            // 
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.Color.MistyRose;
            this.pnBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBase.Location = new System.Drawing.Point(0, 105);
            this.pnBase.Name = "pnBase";
            this.pnBase.Size = new System.Drawing.Size(1280, 845);
            this.pnBase.TabIndex = 38;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 1006);
            this.Controls.Add(this.pnBase);
            this.Controls.Add(this.pnMainTitle);
            this.Controls.Add(this.pnMenu);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Machine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.pnMenu.ResumeLayout(false);
            this.pnMainTitle.ResumeLayout(false);
            this.pnMainTitle.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnRewind.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem Close;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel pnMenu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btUtil;
        private System.Windows.Forms.Button btOption;
        private System.Windows.Forms.Button btSpc;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.Button btOperation;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Panel pnRewind;
        private System.Windows.Forms.Label lbOptn1;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Button btMinimization;
        private System.Windows.Forms.Label lbText2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbText1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbOptn2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbOptn3;
        private System.Windows.Forms.Panel pnBase;
    }
}

