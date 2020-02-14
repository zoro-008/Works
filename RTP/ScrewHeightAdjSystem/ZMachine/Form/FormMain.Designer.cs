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
            this.btReport = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.btOperation = new System.Windows.Forms.Button();
            this.pnBase = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnOptn2 = new System.Windows.Forms.Panel();
            this.lbOptn2 = new System.Windows.Forms.Label();
            this.pnOptn1 = new System.Windows.Forms.Panel();
            this.lbOptn1 = new System.Windows.Forms.Label();
            this.lbStat = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.pnMainTitle = new System.Windows.Forms.Panel();
            this.lbDate = new System.Windows.Forms.Label();
            this.pnMenu.SuspendLayout();
            this.pnOptn2.SuspendLayout();
            this.pnOptn1.SuspendLayout();
            this.pnMainTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Close.Name = "Close";
            resources.ApplyResources(this.Close, "Close");
            // 
            // tmUpdate
            // 
            this.tmUpdate.Interval = 50;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // pnMenu
            // 
            this.pnMenu.BackColor = System.Drawing.Color.MistyRose;
            this.pnMenu.Controls.Add(this.splitter1);
            this.pnMenu.Controls.Add(this.btExit);
            this.pnMenu.Controls.Add(this.btUtil);
            this.pnMenu.Controls.Add(this.btOption);
            this.pnMenu.Controls.Add(this.btReport);
            this.pnMenu.Controls.Add(this.btDevice);
            this.pnMenu.Controls.Add(this.btOperation);
            resources.ApplyResources(this.pnMenu, "pnMenu");
            this.pnMenu.Name = "pnMenu";
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
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
            // btOption
            // 
            this.btOption.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btOption, "btOption");
            this.btOption.Name = "btOption";
            this.btOption.Tag = "3";
            this.btOption.UseVisualStyleBackColor = false;
            this.btOption.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btReport
            // 
            this.btReport.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btReport, "btReport");
            this.btReport.Name = "btReport";
            this.btReport.Tag = "4";
            this.btReport.UseVisualStyleBackColor = false;
            this.btReport.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btDevice
            // 
            this.btDevice.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.btDevice, "btDevice");
            this.btDevice.Name = "btDevice";
            this.btDevice.Tag = "2";
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
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pnBase, "pnBase");
            this.pnBase.Name = "pnBase";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // pnOptn2
            // 
            this.pnOptn2.BackColor = System.Drawing.Color.Red;
            this.pnOptn2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOptn2.Controls.Add(this.lbOptn2);
            resources.ApplyResources(this.pnOptn2, "pnOptn2");
            this.pnOptn2.Name = "pnOptn2";
            // 
            // lbOptn2
            // 
            this.lbOptn2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbOptn2, "lbOptn2");
            this.lbOptn2.ForeColor = System.Drawing.Color.White;
            this.lbOptn2.Name = "lbOptn2";
            // 
            // pnOptn1
            // 
            this.pnOptn1.BackColor = System.Drawing.Color.Red;
            this.pnOptn1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOptn1.Controls.Add(this.lbOptn1);
            resources.ApplyResources(this.pnOptn1, "pnOptn1");
            this.pnOptn1.Name = "pnOptn1";
            // 
            // lbOptn1
            // 
            this.lbOptn1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbOptn1, "lbOptn1");
            this.lbOptn1.ForeColor = System.Drawing.Color.White;
            this.lbOptn1.Name = "lbOptn1";
            // 
            // lbStat
            // 
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbStat, "lbStat");
            this.lbStat.ForeColor = System.Drawing.Color.White;
            this.lbStat.Name = "lbStat";
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.ForeColor = System.Drawing.Color.White;
            this.lbName.Name = "lbName";
            this.lbName.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbName_MouseDoubleClick);
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbMainThreadTime, "lbMainThreadTime");
            this.lbMainThreadTime.ForeColor = System.Drawing.Color.Silver;
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            // 
            // lbVer
            // 
            this.lbVer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbVer, "lbVer");
            this.lbVer.ForeColor = System.Drawing.Color.Silver;
            this.lbVer.Name = "lbVer";
            // 
            // pnMainTitle
            // 
            this.pnMainTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            resources.ApplyResources(this.pnMainTitle, "pnMainTitle");
            this.pnMainTitle.Controls.Add(this.lbDate);
            this.pnMainTitle.Controls.Add(this.panel2);
            this.pnMainTitle.Controls.Add(this.lbStat);
            this.pnMainTitle.Controls.Add(this.lbVer);
            this.pnMainTitle.Controls.Add(this.pnOptn2);
            this.pnMainTitle.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle.Controls.Add(this.pnOptn1);
            this.pnMainTitle.Controls.Add(this.lbName);
            this.pnMainTitle.Name = "pnMainTitle";
            // 
            // lbDate
            // 
            this.lbDate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbDate, "lbDate");
            this.lbDate.Name = "lbDate";
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnBase);
            this.Controls.Add(this.pnMainTitle);
            this.Controls.Add(this.pnMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.pnMenu.ResumeLayout(false);
            this.pnOptn2.ResumeLayout(false);
            this.pnOptn1.ResumeLayout(false);
            this.pnMainTitle.ResumeLayout(false);
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
        private System.Windows.Forms.Button btReport;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.Button btOperation;
        private System.Windows.Forms.Panel pnBase;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Panel pnOptn2;
        private System.Windows.Forms.Label lbOptn2;
        private System.Windows.Forms.Panel pnOptn1;
        private System.Windows.Forms.Label lbOptn1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnMainTitle;
        private System.Windows.Forms.Label lbDate;
    }
}

