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
            this.btOption = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            this.btUtil = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.lbRecThreadTime = new System.Windows.Forms.Label();
            this.lbDevice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnOptn3 = new System.Windows.Forms.Panel();
            this.lbOptn3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbStat = new System.Windows.Forms.Label();
            this.lbText2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbText1 = new System.Windows.Forms.Label();
            this.btMinimization = new System.Windows.Forms.Button();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.pnBase = new System.Windows.Forms.Panel();
            this.tmTemp = new System.Windows.Forms.Timer(this.components);
            this.pnMenu.SuspendLayout();
            this.pnMainTitle.SuspendLayout();
            this.pnOptn3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Close.Name = "Close";
            resources.ApplyResources(this.Close, "Close");
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // pnMenu
            // 
            this.pnMenu.BackColor = System.Drawing.Color.MistyRose;
            this.pnMenu.Controls.Add(this.btOption);
            this.pnMenu.Controls.Add(this.btExit);
            this.pnMenu.Controls.Add(this.btUtil);
            this.pnMenu.Controls.Add(this.btDevice);
            this.pnMenu.Controls.Add(this.btOperation);
            this.pnMenu.Controls.Add(this.splitter1);
            resources.ApplyResources(this.pnMenu, "pnMenu");
            this.pnMenu.Name = "pnMenu";
            // 
            // btOption
            // 
            this.btOption.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btOption, "btOption");
            this.btOption.Name = "btOption";
            this.btOption.Tag = "3";
            this.btOption.UseVisualStyleBackColor = false;
            this.btOption.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btExit
            // 
            this.btExit.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btExit, "btExit");
            this.btExit.Name = "btExit";
            this.btExit.Tag = "6";
            this.btExit.UseVisualStyleBackColor = false;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btUtil
            // 
            this.btUtil.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btUtil, "btUtil");
            this.btUtil.Name = "btUtil";
            this.btUtil.Tag = "5";
            this.btUtil.UseVisualStyleBackColor = false;
            this.btUtil.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btDevice
            // 
            this.btDevice.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btDevice, "btDevice");
            this.btDevice.Name = "btDevice";
            this.btDevice.Tag = "1";
            this.btDevice.UseVisualStyleBackColor = false;
            this.btDevice.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btOperation
            // 
            this.btOperation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btOperation.FlatAppearance.BorderColor = System.Drawing.Color.White;
            resources.ApplyResources(this.btOperation, "btOperation");
            this.btOperation.Name = "btOperation";
            this.btOperation.Tag = "0";
            this.btOperation.UseVisualStyleBackColor = false;
            this.btOperation.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            resources.ApplyResources(this.pnMainTitle, "pnMainTitle");
            this.pnMainTitle.Controls.Add(this.lbRecThreadTime);
            this.pnMainTitle.Controls.Add(this.lbDevice);
            this.pnMainTitle.Controls.Add(this.label1);
            this.pnMainTitle.Controls.Add(this.pnOptn3);
            this.pnMainTitle.Controls.Add(this.panel2);
            this.pnMainTitle.Controls.Add(this.lbText2);
            this.pnMainTitle.Controls.Add(this.panel1);
            this.pnMainTitle.Controls.Add(this.lbText1);
            this.pnMainTitle.Controls.Add(this.btMinimization);
            this.pnMainTitle.Controls.Add(this.lbDate);
            this.pnMainTitle.Controls.Add(this.lbVer);
            this.pnMainTitle.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle.Controls.Add(this.lbName);
            this.pnMainTitle.Name = "pnMainTitle";
            // 
            // lbRecThreadTime
            // 
            this.lbRecThreadTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbRecThreadTime, "lbRecThreadTime");
            this.lbRecThreadTime.Name = "lbRecThreadTime";
            // 
            // lbDevice
            // 
            resources.ApplyResources(this.lbDevice, "lbDevice");
            this.lbDevice.BackColor = System.Drawing.Color.Transparent;
            this.lbDevice.Name = "lbDevice";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Name = "label1";
            // 
            // pnOptn3
            // 
            this.pnOptn3.BackColor = System.Drawing.Color.Red;
            this.pnOptn3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOptn3.Controls.Add(this.lbOptn3);
            resources.ApplyResources(this.pnOptn3, "pnOptn3");
            this.pnOptn3.Name = "pnOptn3";
            // 
            // lbOptn3
            // 
            this.lbOptn3.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbOptn3, "lbOptn3");
            this.lbOptn3.ForeColor = System.Drawing.Color.White;
            this.lbOptn3.Name = "lbOptn3";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.lbStat);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // lbStat
            // 
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbStat, "lbStat");
            this.lbStat.Name = "lbStat";
            // 
            // lbText2
            // 
            resources.ApplyResources(this.lbText2, "lbText2");
            this.lbText2.BackColor = System.Drawing.Color.Transparent;
            this.lbText2.Name = "lbText2";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            this.panel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDoubleClick);
            // 
            // lbText1
            // 
            resources.ApplyResources(this.lbText1, "lbText1");
            this.lbText1.BackColor = System.Drawing.Color.Transparent;
            this.lbText1.Name = "lbText1";
            // 
            // btMinimization
            // 
            resources.ApplyResources(this.btMinimization, "btMinimization");
            this.btMinimization.Name = "btMinimization";
            this.btMinimization.UseVisualStyleBackColor = true;
            this.btMinimization.Click += new System.EventHandler(this.btMinimization_Click_1);
            // 
            // lbDate
            // 
            this.lbDate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbDate, "lbDate");
            this.lbDate.Name = "lbDate";
            this.lbDate.DoubleClick += new System.EventHandler(this.lbDate_DoubleClick);
            // 
            // lbVer
            // 
            this.lbVer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbVer, "lbVer");
            this.lbVer.Name = "lbVer";
            this.lbVer.DoubleClick += new System.EventHandler(this.lbVer_DoubleClick);
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbMainThreadTime, "lbMainThreadTime");
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Name = "lbName";
            this.lbName.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbName_MouseDoubleClick);
            // 
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.Color.MistyRose;
            resources.ApplyResources(this.pnBase, "pnBase");
            this.pnBase.Name = "pnBase";
            // 
            // tmTemp
            // 
            this.tmTemp.Interval = 1000;
            this.tmTemp.Tick += new System.EventHandler(this.tmTemp_Tick);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnBase);
            this.Controls.Add(this.pnMenu);
            this.Controls.Add(this.pnMainTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FormMain_VisibleChanged);
            this.pnMenu.ResumeLayout(false);
            this.pnMainTitle.ResumeLayout(false);
            this.pnMainTitle.PerformLayout();
            this.pnOptn3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem Close;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel pnMenu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Button btMinimization;
        private System.Windows.Forms.Label lbText2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbText1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnOptn3;
        private System.Windows.Forms.Label lbOptn3;
        private System.Windows.Forms.Panel pnBase;
        private System.Windows.Forms.Label lbDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btUtil;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.Button btOperation;
        private System.Windows.Forms.Label lbRecThreadTime;
        private System.Windows.Forms.Button btOption;
        private System.Windows.Forms.Timer tmTemp;
    }
}

