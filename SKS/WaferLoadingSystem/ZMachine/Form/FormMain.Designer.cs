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
            this.pnMain = new MetroFramework.Controls.MetroPanel();
            this.pnBase = new System.Windows.Forms.Panel();
            this.btReport = new MetroFramework.Controls.MetroTile();
            this.btUtil = new MetroFramework.Controls.MetroTile();
            this.btManual = new MetroFramework.Controls.MetroTile();
            this.btOption = new MetroFramework.Controls.MetroTile();
            this.pnMainTitle2 = new System.Windows.Forms.Panel();
            this.lbStat = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbName = new System.Windows.Forms.Label();
            this.btMinimize = new MetroFramework.Controls.MetroTile();
            this.btDevice = new MetroFramework.Controls.MetroTile();
            this.btExit = new MetroFramework.Controls.MetroTile();
            this.btOperation = new MetroFramework.Controls.MetroTile();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbMainThreadTime = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.pnMain.SuspendLayout();
            this.pnBase.SuspendLayout();
            this.pnMainTitle2.SuspendLayout();
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
            // pnMain
            // 
            this.pnMain.BackColor = System.Drawing.Color.DimGray;
            this.pnMain.Controls.Add(this.pnBase);
            this.pnMain.Controls.Add(this.pnMainTitle2);
            resources.ApplyResources(this.pnMain, "pnMain");
            this.pnMain.HorizontalScrollbarBarColor = true;
            this.pnMain.HorizontalScrollbarHighlightOnWheel = false;
            this.pnMain.HorizontalScrollbarSize = 10;
            this.pnMain.Name = "pnMain";
            this.pnMain.VerticalScrollbarBarColor = true;
            this.pnMain.VerticalScrollbarHighlightOnWheel = false;
            this.pnMain.VerticalScrollbarSize = 10;
            // 
            // pnBase
            // 
            this.pnBase.BackColor = System.Drawing.Color.DarkGray;
            this.pnBase.Controls.Add(this.btReport);
            this.pnBase.Controls.Add(this.btUtil);
            this.pnBase.Controls.Add(this.btManual);
            this.pnBase.Controls.Add(this.btOption);
            resources.ApplyResources(this.pnBase, "pnBase");
            this.pnBase.Name = "pnBase";
            // 
            // btReport
            // 
            this.btReport.ActiveControl = null;
            this.btReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btReport, "btReport");
            this.btReport.Name = "btReport";
            this.btReport.Tag = "4";
            this.btReport.TileImage = ((System.Drawing.Image)(resources.GetObject("btReport.TileImage")));
            this.btReport.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btReport.UseSelectable = true;
            this.btReport.UseTileImage = true;
            this.btReport.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btUtil
            // 
            this.btUtil.ActiveControl = null;
            this.btUtil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btUtil, "btUtil");
            this.btUtil.Name = "btUtil";
            this.btUtil.Tag = "5";
            this.btUtil.TileImage = ((System.Drawing.Image)(resources.GetObject("btUtil.TileImage")));
            this.btUtil.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btUtil.UseSelectable = true;
            this.btUtil.UseTileImage = true;
            this.btUtil.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btManual
            // 
            this.btManual.ActiveControl = null;
            this.btManual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btManual, "btManual");
            this.btManual.Name = "btManual";
            this.btManual.Tag = "1";
            this.btManual.TileImage = ((System.Drawing.Image)(resources.GetObject("btManual.TileImage")));
            this.btManual.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btManual.UseSelectable = true;
            this.btManual.UseTileImage = true;
            this.btManual.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btOption
            // 
            this.btOption.ActiveControl = null;
            this.btOption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btOption, "btOption");
            this.btOption.Name = "btOption";
            this.btOption.Tag = "3";
            this.btOption.TileImage = ((System.Drawing.Image)(resources.GetObject("btOption.TileImage")));
            this.btOption.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btOption.UseSelectable = true;
            this.btOption.UseTileImage = true;
            this.btOption.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // pnMainTitle2
            // 
            this.pnMainTitle2.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pnMainTitle2, "pnMainTitle2");
            this.pnMainTitle2.Controls.Add(this.lbStat);
            this.pnMainTitle2.Controls.Add(this.panel1);
            this.pnMainTitle2.Controls.Add(this.lbName);
            this.pnMainTitle2.Controls.Add(this.btMinimize);
            this.pnMainTitle2.Controls.Add(this.btDevice);
            this.pnMainTitle2.Controls.Add(this.btExit);
            this.pnMainTitle2.Controls.Add(this.btOperation);
            this.pnMainTitle2.Controls.Add(this.lbDate);
            this.pnMainTitle2.Controls.Add(this.lbMainThreadTime);
            this.pnMainTitle2.Controls.Add(this.lbVer);
            this.pnMainTitle2.Name = "pnMainTitle2";
            // 
            // lbStat
            // 
            this.lbStat.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbStat, "lbStat");
            this.lbStat.ForeColor = System.Drawing.Color.White;
            this.lbStat.Name = "lbStat";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.ForeColor = System.Drawing.Color.White;
            this.lbName.Name = "lbName";
            this.lbName.Click += new System.EventHandler(this.lbName_Click);
            this.lbName.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbName_MouseDoubleClick);
            // 
            // btMinimize
            // 
            this.btMinimize.ActiveControl = null;
            resources.ApplyResources(this.btMinimize, "btMinimize");
            this.btMinimize.Name = "btMinimize";
            this.btMinimize.UseSelectable = true;
            this.btMinimize.Click += new System.EventHandler(this.btMinimize_Click);
            // 
            // btDevice
            // 
            this.btDevice.ActiveControl = null;
            this.btDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btDevice, "btDevice");
            this.btDevice.Name = "btDevice";
            this.btDevice.Tag = "2";
            this.btDevice.TileImage = ((System.Drawing.Image)(resources.GetObject("btDevice.TileImage")));
            this.btDevice.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btDevice.UseSelectable = true;
            this.btDevice.UseTileImage = true;
            this.btDevice.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // btExit
            // 
            this.btExit.ActiveControl = null;
            this.btExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btExit, "btExit");
            this.btExit.Name = "btExit";
            this.btExit.Tag = "6";
            this.btExit.TileImage = ((System.Drawing.Image)(resources.GetObject("btExit.TileImage")));
            this.btExit.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btExit.UseSelectable = true;
            this.btExit.UseTileImage = true;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btOperation
            // 
            this.btOperation.ActiveControl = null;
            this.btOperation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.btOperation, "btOperation");
            this.btOperation.Name = "btOperation";
            this.btOperation.Tag = "0";
            this.btOperation.TileImage = ((System.Drawing.Image)(resources.GetObject("btOperation.TileImage")));
            this.btOperation.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btOperation.UseSelectable = true;
            this.btOperation.UseTileImage = true;
            this.btOperation.Click += new System.EventHandler(this.btOperation_Click);
            // 
            // lbDate
            // 
            this.lbDate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbDate, "lbDate");
            this.lbDate.ForeColor = System.Drawing.Color.Silver;
            this.lbDate.Name = "lbDate";
            this.lbDate.Click += new System.EventHandler(this.lbDate_Click);
            // 
            // lbMainThreadTime
            // 
            this.lbMainThreadTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbMainThreadTime, "lbMainThreadTime");
            this.lbMainThreadTime.ForeColor = System.Drawing.Color.Silver;
            this.lbMainThreadTime.Name = "lbMainThreadTime";
            this.lbMainThreadTime.Click += new System.EventHandler(this.lbMainThreadTime_Click);
            // 
            // lbVer
            // 
            this.lbVer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lbVer, "lbVer");
            this.lbVer.ForeColor = System.Drawing.Color.Silver;
            this.lbVer.Name = "lbVer";
            this.lbVer.Click += new System.EventHandler(this.lbVer_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.pnMain.ResumeLayout(false);
            this.pnBase.ResumeLayout(false);
            this.pnMainTitle2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem Close;
        private System.Windows.Forms.Timer tmUpdate;
        private MetroFramework.Controls.MetroPanel pnMain;
        private System.Windows.Forms.Panel pnMainTitle2;
        private MetroFramework.Controls.MetroTile btExit;
        private MetroFramework.Controls.MetroTile btUtil;
        private MetroFramework.Controls.MetroTile btManual;
        private MetroFramework.Controls.MetroTile btReport;
        private MetroFramework.Controls.MetroTile btOption;
        private MetroFramework.Controls.MetroTile btDevice;
        private MetroFramework.Controls.MetroTile btOperation;
        private System.Windows.Forms.Label lbStat;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbMainThreadTime;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Panel pnBase;
        private MetroFramework.Controls.MetroTile btMinimize;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Panel panel1;
    }
}

