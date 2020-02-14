namespace Machine
{
    partial class FormOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbNoDisp = new System.Windows.Forms.CheckBox();
            this.cbEpoxyPushTest = new System.Windows.Forms.CheckBox();
            this.cbUseGuideEjct = new System.Windows.Forms.CheckBox();
            this.cbSubBarcodeSkip = new System.Windows.Forms.CheckBox();
            this.cbWfrBarcodeSkip = new System.Windows.Forms.CheckBox();
            this.cbSSTGLoadingStop = new System.Windows.Forms.CheckBox();
            this.cbWSTGLoadingStop = new System.Windows.Forms.CheckBox();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbNotUseHeater = new System.Windows.Forms.CheckBox();
            this.lbVer = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel12 = new System.Windows.Forms.Panel();
            this.label167 = new System.Windows.Forms.Label();
            this.tbFileLotIDMask = new System.Windows.Forms.TextBox();
            this.tbMapFileFolder = new System.Windows.Forms.TextBox();
            this.label170 = new System.Windows.Forms.Label();
            this.label171 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btMapFileFolder = new System.Windows.Forms.Button();
            this.tbSampleFileName = new System.Windows.Forms.TextBox();
            this.tbSampleWaferBar = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBarLotIDMask = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbBarWfrNoMask = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel12.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbNoDisp);
            this.panel1.Controls.Add(this.cbEpoxyPushTest);
            this.panel1.Controls.Add(this.cbUseGuideEjct);
            this.panel1.Controls.Add(this.cbSubBarcodeSkip);
            this.panel1.Controls.Add(this.cbWfrBarcodeSkip);
            this.panel1.Controls.Add(this.cbSSTGLoadingStop);
            this.panel1.Controls.Add(this.cbWSTGLoadingStop);
            this.panel1.Controls.Add(this.cbIgnrDoor);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 306);
            this.panel1.TabIndex = 0;
            // 
            // cbNoDisp
            // 
            this.cbNoDisp.AutoSize = true;
            this.cbNoDisp.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbNoDisp.Location = new System.Drawing.Point(10, 230);
            this.cbNoDisp.Name = "cbNoDisp";
            this.cbNoDisp.Size = new System.Drawing.Size(209, 20);
            this.cbNoDisp.TabIndex = 8;
            this.cbNoDisp.Text = "디스펜싱 토출신호 안씀";
            this.cbNoDisp.UseVisualStyleBackColor = true;
            // 
            // cbEpoxyPushTest
            // 
            this.cbEpoxyPushTest.AutoSize = true;
            this.cbEpoxyPushTest.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbEpoxyPushTest.Location = new System.Drawing.Point(10, 204);
            this.cbEpoxyPushTest.Name = "cbEpoxyPushTest";
            this.cbEpoxyPushTest.Size = new System.Drawing.Size(238, 20);
            this.cbEpoxyPushTest.TabIndex = 7;
            this.cbEpoxyPushTest.Text = "에폭시 눌림 확인 검사 모드";
            this.cbEpoxyPushTest.UseVisualStyleBackColor = true;
            // 
            // cbUseGuideEjct
            // 
            this.cbUseGuideEjct.AutoSize = true;
            this.cbUseGuideEjct.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbUseGuideEjct.Location = new System.Drawing.Point(10, 178);
            this.cbUseGuideEjct.Name = "cbUseGuideEjct";
            this.cbUseGuideEjct.Size = new System.Drawing.Size(215, 20);
            this.cbUseGuideEjct.TabIndex = 6;
            this.cbUseGuideEjct.Text = "픽커 가이드 이젝트 사용";
            this.cbUseGuideEjct.UseVisualStyleBackColor = true;
            // 
            // cbSubBarcodeSkip
            // 
            this.cbSubBarcodeSkip.AutoSize = true;
            this.cbSubBarcodeSkip.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSubBarcodeSkip.Location = new System.Drawing.Point(10, 152);
            this.cbSubBarcodeSkip.Name = "cbSubBarcodeSkip";
            this.cbSubBarcodeSkip.Size = new System.Drawing.Size(283, 20);
            this.cbSubBarcodeSkip.TabIndex = 5;
            this.cbSubBarcodeSkip.Text = "서브스트레이트 바코드 동작 무시";
            this.cbSubBarcodeSkip.UseVisualStyleBackColor = true;
            // 
            // cbWfrBarcodeSkip
            // 
            this.cbWfrBarcodeSkip.AutoSize = true;
            this.cbWfrBarcodeSkip.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbWfrBarcodeSkip.Location = new System.Drawing.Point(10, 126);
            this.cbWfrBarcodeSkip.Name = "cbWfrBarcodeSkip";
            this.cbWfrBarcodeSkip.Size = new System.Drawing.Size(215, 20);
            this.cbWfrBarcodeSkip.TabIndex = 4;
            this.cbWfrBarcodeSkip.Text = "웨이퍼 바코드 동작 무시";
            this.cbWfrBarcodeSkip.UseVisualStyleBackColor = true;
            // 
            // cbSSTGLoadingStop
            // 
            this.cbSSTGLoadingStop.AutoSize = true;
            this.cbSSTGLoadingStop.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSSTGLoadingStop.Location = new System.Drawing.Point(10, 100);
            this.cbSSTGLoadingStop.Name = "cbSSTGLoadingStop";
            this.cbSSTGLoadingStop.Size = new System.Drawing.Size(266, 20);
            this.cbSSTGLoadingStop.TabIndex = 3;
            this.cbSSTGLoadingStop.Text = "서브스트레이트 로더 동작 정지";
            this.cbSSTGLoadingStop.UseVisualStyleBackColor = true;
            // 
            // cbWSTGLoadingStop
            // 
            this.cbWSTGLoadingStop.AutoSize = true;
            this.cbWSTGLoadingStop.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbWSTGLoadingStop.Location = new System.Drawing.Point(10, 74);
            this.cbWSTGLoadingStop.Name = "cbWSTGLoadingStop";
            this.cbWSTGLoadingStop.Size = new System.Drawing.Size(198, 20);
            this.cbWSTGLoadingStop.TabIndex = 2;
            this.cbWSTGLoadingStop.Text = "웨이퍼 로더 동작 정지";
            this.cbWSTGLoadingStop.UseVisualStyleBackColor = true;
            // 
            // cbIgnrDoor
            // 
            this.cbIgnrDoor.AutoSize = true;
            this.cbIgnrDoor.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbIgnrDoor.Location = new System.Drawing.Point(10, 48);
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.Size = new System.Drawing.Size(164, 20);
            this.cbIgnrDoor.TabIndex = 1;
            this.cbIgnrDoor.Text = "문 열림 알람 무시";
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(412, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "OPTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbNotUseHeater
            // 
            this.cbNotUseHeater.AutoSize = true;
            this.cbNotUseHeater.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbNotUseHeater.Location = new System.Drawing.Point(661, 446);
            this.cbNotUseHeater.Name = "cbNotUseHeater";
            this.cbNotUseHeater.Size = new System.Drawing.Size(209, 20);
            this.cbNotUseHeater.TabIndex = 6;
            this.cbNotUseHeater.Text = "스테이지 히터 사용안함";
            this.cbNotUseHeater.UseVisualStyleBackColor = true;
            this.cbNotUseHeater.Visible = false;
            // 
            // lbVer
            // 
            this.lbVer.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbVer.Location = new System.Drawing.Point(713, 786);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(157, 23);
            this.lbVer.TabIndex = 1;
            this.lbVer.Text = "VER : 1.1.1.1";
            // 
            // lbDate
            // 
            this.lbDate.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDate.Location = new System.Drawing.Point(714, 816);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(220, 16);
            this.lbDate.TabIndex = 2;
            this.lbDate.Text = "2016_06_22_PM 4: 57";
            // 
            // btSave
            // 
            this.btSave.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSave.Image = ((System.Drawing.Image)(resources.GetObject("btSave.Image")));
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSave.Location = new System.Drawing.Point(952, 769);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(305, 73);
            this.btSave.TabIndex = 3;
            this.btSave.Text = "SAVE";
            this.btSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.label5);
            this.panel12.Controls.Add(this.tbBarWfrNoMask);
            this.panel12.Controls.Add(this.label4);
            this.panel12.Controls.Add(this.tbBarLotIDMask);
            this.panel12.Controls.Add(this.tbSampleWaferBar);
            this.panel12.Controls.Add(this.label3);
            this.panel12.Controls.Add(this.tbSampleFileName);
            this.panel12.Controls.Add(this.btMapFileFolder);
            this.panel12.Controls.Add(this.label2);
            this.panel12.Controls.Add(this.label167);
            this.panel12.Controls.Add(this.tbFileLotIDMask);
            this.panel12.Controls.Add(this.tbMapFileFolder);
            this.panel12.Controls.Add(this.label170);
            this.panel12.Controls.Add(this.label171);
            this.panel12.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.panel12.Location = new System.Drawing.Point(414, 1);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(418, 439);
            this.panel12.TabIndex = 35;
            // 
            // label167
            // 
            this.label167.AutoSize = true;
            this.label167.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label167.Location = new System.Drawing.Point(6, 175);
            this.label167.Name = "label167";
            this.label167.Size = new System.Drawing.Size(350, 16);
            this.label167.TabIndex = 50;
            this.label167.Text = "맵파일 이름에서 LotID 마스크(-:skip *:use)";
            // 
            // tbFileLotIDMask
            // 
            this.tbFileLotIDMask.Location = new System.Drawing.Point(9, 198);
            this.tbFileLotIDMask.Name = "tbFileLotIDMask";
            this.tbFileLotIDMask.Size = new System.Drawing.Size(402, 26);
            this.tbFileLotIDMask.TabIndex = 48;
            // 
            // tbMapFileFolder
            // 
            this.tbMapFileFolder.Location = new System.Drawing.Point(9, 74);
            this.tbMapFileFolder.Name = "tbMapFileFolder";
            this.tbMapFileFolder.Size = new System.Drawing.Size(402, 26);
            this.tbMapFileFolder.TabIndex = 46;
            // 
            // label170
            // 
            this.label170.AutoSize = true;
            this.label170.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label170.Location = new System.Drawing.Point(6, 113);
            this.label170.Name = "label170";
            this.label170.Size = new System.Drawing.Size(295, 16);
            this.label170.TabIndex = 45;
            this.label170.Text = "샘플 맵파일 이름(시퀜스에 사용안함)";
            // 
            // label171
            // 
            this.label171.AutoSize = true;
            this.label171.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label171.Location = new System.Drawing.Point(5, 54);
            this.label171.Name = "label171";
            this.label171.Size = new System.Drawing.Size(139, 16);
            this.label171.TabIndex = 43;
            this.label171.Text = "맵파일 폴더 루트";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(416, 35);
            this.label2.TabIndex = 56;
            this.label2.Text = "OPTION";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btMapFileFolder
            // 
            this.btMapFileFolder.Location = new System.Drawing.Point(379, 51);
            this.btMapFileFolder.Name = "btMapFileFolder";
            this.btMapFileFolder.Size = new System.Drawing.Size(33, 22);
            this.btMapFileFolder.TabIndex = 57;
            this.btMapFileFolder.Text = "...";
            this.btMapFileFolder.UseVisualStyleBackColor = true;
            this.btMapFileFolder.Click += new System.EventHandler(this.btMapFileFolder_Click);
            // 
            // tbSampleFileName
            // 
            this.tbSampleFileName.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.tbSampleFileName.Location = new System.Drawing.Point(9, 134);
            this.tbSampleFileName.Name = "tbSampleFileName";
            this.tbSampleFileName.Size = new System.Drawing.Size(402, 26);
            this.tbSampleFileName.TabIndex = 58;
            // 
            // tbSampleWaferBar
            // 
            this.tbSampleWaferBar.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.tbSampleWaferBar.Location = new System.Drawing.Point(9, 255);
            this.tbSampleWaferBar.Name = "tbSampleWaferBar";
            this.tbSampleWaferBar.Size = new System.Drawing.Size(402, 26);
            this.tbSampleWaferBar.TabIndex = 60;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(6, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(312, 16);
            this.label3.TabIndex = 59;
            this.label3.Text = "샘플 웨이퍼 바코드(시퀜스에 사용안함)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(6, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(367, 16);
            this.label4.TabIndex = 62;
            this.label4.Text = "웨이퍼 바코드에서 LotID 마스크(-:skip *:use)";
            // 
            // tbBarLotIDMask
            // 
            this.tbBarLotIDMask.Location = new System.Drawing.Point(9, 312);
            this.tbBarLotIDMask.Name = "tbBarLotIDMask";
            this.tbBarLotIDMask.Size = new System.Drawing.Size(402, 26);
            this.tbBarLotIDMask.TabIndex = 61;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(6, 350);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(338, 16);
            this.label5.TabIndex = 64;
            this.label5.Text = "웨이퍼 바코드에서 WaferNo(-:skip *:use)";
            // 
            // tbBarWfrNoMask
            // 
            this.tbBarWfrNoMask.Location = new System.Drawing.Point(9, 373);
            this.tbBarWfrNoMask.Name = "tbBarWfrNoMask";
            this.tbBarWfrNoMask.Size = new System.Drawing.Size(402, 26);
            this.tbBarWfrNoMask.TabIndex = 63;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.cbNotUseHeater);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVer);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Text = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
        private System.Windows.Forms.CheckBox cbWSTGLoadingStop;
        private System.Windows.Forms.CheckBox cbSSTGLoadingStop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbWfrBarcodeSkip;
        private System.Windows.Forms.CheckBox cbSubBarcodeSkip;
        private System.Windows.Forms.CheckBox cbNotUseHeater;
        private System.Windows.Forms.CheckBox cbUseGuideEjct;
        private System.Windows.Forms.CheckBox cbEpoxyPushTest;
        private System.Windows.Forms.CheckBox cbNoDisp;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btMapFileFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label167;
        private System.Windows.Forms.TextBox tbFileLotIDMask;
        private System.Windows.Forms.TextBox tbMapFileFolder;
        private System.Windows.Forms.Label label170;
        private System.Windows.Forms.Label label171;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbBarWfrNoMask;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBarLotIDMask;
        private System.Windows.Forms.TextBox tbSampleWaferBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSampleFileName;
    }
}