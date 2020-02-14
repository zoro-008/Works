namespace Machine
{
    partial class FormOperation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOperation));
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnCamBase = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnCam = new System.Windows.Forms.Panel();
            this.pnError = new System.Windows.Forms.Panel();
            this.lbErr = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pnOper = new System.Windows.Forms.Panel();
            this.btOperator = new System.Windows.Forms.Button();
            this.btAllHome = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pnOperation = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnDownSide = new System.Windows.Forms.Panel();
            this.pnOperMan = new System.Windows.Forms.Panel();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.button3 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btMan1 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btStgHeat = new System.Windows.Forms.Button();
            this.btBldHeat = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnLotInfo = new System.Windows.Forms.Panel();
            this.lvLog = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnLotOpen = new System.Windows.Forms.Panel();
            this.btLotEnd = new System.Windows.Forms.Button();
            this.btLotOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pnWorkInfo = new System.Windows.Forms.Panel();
            this.lvLot = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.pnCamBase.SuspendLayout();
            this.pnError.SuspendLayout();
            this.pnOper.SuspendLayout();
            this.pnOperation.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnDownSide.SuspendLayout();
            this.pnOperMan.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnLotInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnLotOpen.SuspendLayout();
            this.pnWorkInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnCamBase
            // 
            this.pnCamBase.BackColor = System.Drawing.SystemColors.Control;
            this.pnCamBase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnCamBase.Controls.Add(this.label3);
            this.pnCamBase.Controls.Add(this.pnCam);
            this.pnCamBase.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pnCamBase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnCamBase.Location = new System.Drawing.Point(638, 3);
            this.pnCamBase.Name = "pnCamBase";
            this.pnCamBase.Size = new System.Drawing.Size(647, 546);
            this.pnCamBase.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(645, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "CAMERA";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnCam
            // 
            this.pnCam.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnCam.Location = new System.Drawing.Point(0, 26);
            this.pnCam.Name = "pnCam";
            this.pnCam.Size = new System.Drawing.Size(640, 515);
            this.pnCam.TabIndex = 2;
            // 
            // pnError
            // 
            this.pnError.BackColor = System.Drawing.SystemColors.Control;
            this.pnError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnError.Controls.Add(this.lbErr);
            this.pnError.Controls.Add(this.label7);
            this.pnError.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnError.Location = new System.Drawing.Point(0, 206);
            this.pnError.Name = "pnError";
            this.pnError.Size = new System.Drawing.Size(430, 110);
            this.pnError.TabIndex = 17;
            // 
            // lbErr
            // 
            this.lbErr.BackColor = System.Drawing.SystemColors.Control;
            this.lbErr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbErr.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbErr.FormattingEnabled = true;
            this.lbErr.ItemHeight = 20;
            this.lbErr.Location = new System.Drawing.Point(0, 24);
            this.lbErr.Name = "lbErr";
            this.lbErr.Size = new System.Drawing.Size(428, 84);
            this.lbErr.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(428, 24);
            this.label7.TabIndex = 0;
            this.label7.Text = "ERROR INFO";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnOper
            // 
            this.pnOper.BackColor = System.Drawing.SystemColors.Control;
            this.pnOper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOper.Controls.Add(this.btOperator);
            this.pnOper.Controls.Add(this.btAllHome);
            this.pnOper.Controls.Add(this.btReset);
            this.pnOper.Controls.Add(this.btStop);
            this.pnOper.Controls.Add(this.btStart);
            this.pnOper.Controls.Add(this.label1);
            this.pnOper.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnOper.Location = new System.Drawing.Point(1030, 0);
            this.pnOper.Name = "pnOper";
            this.pnOper.Size = new System.Drawing.Size(250, 316);
            this.pnOper.TabIndex = 18;
            // 
            // btOperator
            // 
            this.btOperator.BackColor = System.Drawing.SystemColors.Control;
            this.btOperator.Dock = System.Windows.Forms.DockStyle.Top;
            this.btOperator.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btOperator.ForeColor = System.Drawing.Color.DarkKhaki;
            this.btOperator.Image = ((System.Drawing.Image)(resources.GetObject("btOperator.Image")));
            this.btOperator.Location = new System.Drawing.Point(0, 256);
            this.btOperator.Name = "btOperator";
            this.btOperator.Size = new System.Drawing.Size(248, 58);
            this.btOperator.TabIndex = 8;
            this.btOperator.Text = "  LOG IN";
            this.btOperator.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOperator.UseVisualStyleBackColor = false;
            this.btOperator.Click += new System.EventHandler(this.btOperator_Click_1);
            // 
            // btAllHome
            // 
            this.btAllHome.BackColor = System.Drawing.SystemColors.Control;
            this.btAllHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.btAllHome.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btAllHome.ForeColor = System.Drawing.Color.LightSeaGreen;
            this.btAllHome.Image = ((System.Drawing.Image)(resources.GetObject("btAllHome.Image")));
            this.btAllHome.Location = new System.Drawing.Point(0, 198);
            this.btAllHome.Name = "btAllHome";
            this.btAllHome.Size = new System.Drawing.Size(248, 58);
            this.btAllHome.TabIndex = 7;
            this.btAllHome.Text = "  HOME";
            this.btAllHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btAllHome.UseVisualStyleBackColor = false;
            this.btAllHome.Click += new System.EventHandler(this.btAllHome_Click);
            // 
            // btReset
            // 
            this.btReset.BackColor = System.Drawing.SystemColors.Control;
            this.btReset.Dock = System.Windows.Forms.DockStyle.Top;
            this.btReset.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btReset.ForeColor = System.Drawing.Color.Gold;
            this.btReset.Image = ((System.Drawing.Image)(resources.GetObject("btReset.Image")));
            this.btReset.Location = new System.Drawing.Point(0, 140);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(248, 58);
            this.btReset.TabIndex = 6;
            this.btReset.Text = "  RESET";
            this.btReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btReset.UseVisualStyleBackColor = false;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // btStop
            // 
            this.btStop.BackColor = System.Drawing.SystemColors.Control;
            this.btStop.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStop.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btStop.ForeColor = System.Drawing.Color.Red;
            this.btStop.Image = ((System.Drawing.Image)(resources.GetObject("btStop.Image")));
            this.btStop.Location = new System.Drawing.Point(0, 82);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(248, 58);
            this.btStop.TabIndex = 3;
            this.btStop.Text = "  STOP";
            this.btStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStop.UseVisualStyleBackColor = false;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btStart
            // 
            this.btStart.BackColor = System.Drawing.SystemColors.Control;
            this.btStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStart.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btStart.ForeColor = System.Drawing.Color.Green;
            this.btStart.Image = ((System.Drawing.Image)(resources.GetObject("btStart.Image")));
            this.btStart.Location = new System.Drawing.Point(0, 24);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(248, 58);
            this.btStart.TabIndex = 2;
            this.btStart.Text = "  START";
            this.btStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStart.UseVisualStyleBackColor = false;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "OPERATION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnOperation
            // 
            this.pnOperation.Controls.Add(this.panel3);
            this.pnOperation.Controls.Add(this.pnDownSide);
            this.pnOperation.Controls.Add(this.pnCamBase);
            this.pnOperation.Location = new System.Drawing.Point(0, 0);
            this.pnOperation.Name = "pnOperation";
            this.pnOperation.Size = new System.Drawing.Size(1280, 863);
            this.pnOperation.TabIndex = 21;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel3.Location = new System.Drawing.Point(2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(634, 541);
            this.panel3.TabIndex = 12;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(632, 515);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(632, 24);
            this.label6.TabIndex = 3;
            this.label6.Text = "DATA";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnDownSide
            // 
            this.pnDownSide.Controls.Add(this.pnOperMan);
            this.pnDownSide.Controls.Add(this.panel2);
            this.pnDownSide.Controls.Add(this.panel1);
            this.pnDownSide.Controls.Add(this.pnOper);
            this.pnDownSide.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDownSide.Location = new System.Drawing.Point(0, 547);
            this.pnDownSide.Name = "pnDownSide";
            this.pnDownSide.Size = new System.Drawing.Size(1280, 316);
            this.pnDownSide.TabIndex = 11;
            // 
            // pnOperMan
            // 
            this.pnOperMan.BackColor = System.Drawing.SystemColors.Control;
            this.pnOperMan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOperMan.Controls.Add(this.flowLayoutPanel6);
            this.pnOperMan.Controls.Add(this.flowLayoutPanel1);
            this.pnOperMan.Controls.Add(this.label10);
            this.pnOperMan.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnOperMan.Location = new System.Drawing.Point(638, 0);
            this.pnOperMan.Name = "pnOperMan";
            this.pnOperMan.Size = new System.Drawing.Size(392, 316);
            this.pnOperMan.TabIndex = 26;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel6.Controls.Add(this.button3);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(194, 24);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(194, 290);
            this.flowLayoutPanel6.TabIndex = 61;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.button3.Location = new System.Drawing.Point(1, 1);
            this.button3.Margin = new System.Windows.Forms.Padding(1);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(191, 54);
            this.button3.TabIndex = 33;
            this.button3.Tag = "12";
            this.button3.Text = "Work Step && Count Clear";
            this.button3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btMan1);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.button2);
            this.flowLayoutPanel1.Controls.Add(this.btStgHeat);
            this.flowLayoutPanel1.Controls.Add(this.btBldHeat);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(194, 290);
            this.flowLayoutPanel1.TabIndex = 59;
            // 
            // btMan1
            // 
            this.btMan1.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btMan1.Location = new System.Drawing.Point(1, 1);
            this.btMan1.Margin = new System.Windows.Forms.Padding(1);
            this.btMan1.Name = "btMan1";
            this.btMan1.Size = new System.Drawing.Size(191, 54);
            this.btMan1.TabIndex = 26;
            this.btMan1.Tag = "10";
            this.btMan1.Text = "1Step Cutting";
            this.btMan1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btMan1.UseVisualStyleBackColor = true;
            this.btMan1.Click += new System.EventHandler(this.btManClick);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.button1.Location = new System.Drawing.Point(1, 57);
            this.button1.Margin = new System.Windows.Forms.Padding(1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(191, 54);
            this.button1.TabIndex = 27;
            this.button1.Tag = "11";
            this.button1.Text = "Feed 5mm";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btManClick);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.button2.Location = new System.Drawing.Point(1, 113);
            this.button2.Margin = new System.Windows.Forms.Padding(1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(191, 54);
            this.button2.TabIndex = 28;
            this.button2.Tag = "12";
            this.button2.Text = "Feed 10mm";
            this.button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btManClick);
            // 
            // btStgHeat
            // 
            this.btStgHeat.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btStgHeat.Location = new System.Drawing.Point(1, 169);
            this.btStgHeat.Margin = new System.Windows.Forms.Padding(1);
            this.btStgHeat.Name = "btStgHeat";
            this.btStgHeat.Size = new System.Drawing.Size(191, 54);
            this.btStgHeat.TabIndex = 31;
            this.btStgHeat.Tag = "12";
            this.btStgHeat.Text = "Stage Heater On";
            this.btStgHeat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStgHeat.UseVisualStyleBackColor = true;
            this.btStgHeat.Click += new System.EventHandler(this.btStgHeat_Click);
            // 
            // btBldHeat
            // 
            this.btBldHeat.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btBldHeat.Location = new System.Drawing.Point(1, 225);
            this.btBldHeat.Margin = new System.Windows.Forms.Padding(1);
            this.btBldHeat.Name = "btBldHeat";
            this.btBldHeat.Size = new System.Drawing.Size(191, 54);
            this.btBldHeat.TabIndex = 32;
            this.btBldHeat.Tag = "12";
            this.btBldHeat.Text = "Blade Heater On ";
            this.btBldHeat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btBldHeat.UseVisualStyleBackColor = true;
            this.btBldHeat.Click += new System.EventHandler(this.btBldHeat_Click);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(390, 24);
            this.label10.TabIndex = 0;
            this.label10.Text = "OPERATION MANUAL";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnLotInfo);
            this.panel2.Controls.Add(this.pnError);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(430, 316);
            this.panel2.TabIndex = 25;
            // 
            // pnLotInfo
            // 
            this.pnLotInfo.BackColor = System.Drawing.SystemColors.Control;
            this.pnLotInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnLotInfo.Controls.Add(this.lvLog);
            this.pnLotInfo.Controls.Add(this.label4);
            this.pnLotInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnLotInfo.Location = new System.Drawing.Point(0, 0);
            this.pnLotInfo.Name = "pnLotInfo";
            this.pnLotInfo.Size = new System.Drawing.Size(430, 204);
            this.pnLotInfo.TabIndex = 23;
            // 
            // lvLog
            // 
            this.lvLog.BackColor = System.Drawing.SystemColors.Control;
            this.lvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLog.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvLog.Location = new System.Drawing.Point(0, 24);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(428, 178);
            this.lvLog.TabIndex = 1;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvLog_MouseDoubleClick);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(428, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "WORK LOG";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnLotOpen);
            this.panel1.Controls.Add(this.pnWorkInfo);
            this.panel1.Location = new System.Drawing.Point(431, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 316);
            this.panel1.TabIndex = 24;
            // 
            // pnLotOpen
            // 
            this.pnLotOpen.BackColor = System.Drawing.Color.Gainsboro;
            this.pnLotOpen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnLotOpen.Controls.Add(this.btLotEnd);
            this.pnLotOpen.Controls.Add(this.btLotOpen);
            this.pnLotOpen.Controls.Add(this.label2);
            this.pnLotOpen.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnLotOpen.Location = new System.Drawing.Point(0, 206);
            this.pnLotOpen.Name = "pnLotOpen";
            this.pnLotOpen.Size = new System.Drawing.Size(207, 110);
            this.pnLotOpen.TabIndex = 25;
            // 
            // btLotEnd
            // 
            this.btLotEnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btLotEnd.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotEnd.Image = ((System.Drawing.Image)(resources.GetObject("btLotEnd.Image")));
            this.btLotEnd.Location = new System.Drawing.Point(0, 66);
            this.btLotEnd.Name = "btLotEnd";
            this.btLotEnd.Size = new System.Drawing.Size(205, 40);
            this.btLotEnd.TabIndex = 4;
            this.btLotEnd.Text = "LOT END";
            this.btLotEnd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLotEnd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLotEnd.UseVisualStyleBackColor = true;
            this.btLotEnd.Click += new System.EventHandler(this.btLotEnd_Click_1);
            // 
            // btLotOpen
            // 
            this.btLotOpen.Dock = System.Windows.Forms.DockStyle.Top;
            this.btLotOpen.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotOpen.Image = ((System.Drawing.Image)(resources.GetObject("btLotOpen.Image")));
            this.btLotOpen.Location = new System.Drawing.Point(0, 24);
            this.btLotOpen.Name = "btLotOpen";
            this.btLotOpen.Size = new System.Drawing.Size(205, 42);
            this.btLotOpen.TabIndex = 3;
            this.btLotOpen.Tag = "7";
            this.btLotOpen.Text = "LOT OPEN";
            this.btLotOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLotOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLotOpen.UseVisualStyleBackColor = true;
            this.btLotOpen.Click += new System.EventHandler(this.btLotOpen_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 24);
            this.label2.TabIndex = 0;
            this.label2.Text = "LOT WORK";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnWorkInfo
            // 
            this.pnWorkInfo.BackColor = System.Drawing.SystemColors.Control;
            this.pnWorkInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnWorkInfo.Controls.Add(this.lvLot);
            this.pnWorkInfo.Controls.Add(this.label5);
            this.pnWorkInfo.Location = new System.Drawing.Point(0, 0);
            this.pnWorkInfo.Name = "pnWorkInfo";
            this.pnWorkInfo.Size = new System.Drawing.Size(206, 204);
            this.pnWorkInfo.TabIndex = 23;
            // 
            // lvLot
            // 
            this.lvLot.BackColor = System.Drawing.SystemColors.Control;
            this.lvLot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLot.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lvLot.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvLot.Location = new System.Drawing.Point(0, 24);
            this.lvLot.Name = "lvLot";
            this.lvLot.Size = new System.Drawing.Size(204, 178);
            this.lvLot.TabIndex = 2;
            this.lvLot.UseCompatibleStateImageBehavior = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "LOT INFORMATION";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.pnOperation);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOperation";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FormOperation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOperation_FormClosing);
            this.Shown += new System.EventHandler(this.FormOperation_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOperation_VisibleChanged);
            this.pnCamBase.ResumeLayout(false);
            this.pnError.ResumeLayout(false);
            this.pnOper.ResumeLayout(false);
            this.pnOperation.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnDownSide.ResumeLayout(false);
            this.pnOperMan.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnLotInfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pnLotOpen.ResumeLayout(false);
            this.pnWorkInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel pnCamBase;
        private System.Windows.Forms.Panel pnError;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnOper;
        private System.Windows.Forms.Button btOperator;
        private System.Windows.Forms.Button btAllHome;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnOperation;
        private System.Windows.Forms.Panel pnLotInfo;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnDownSide;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnWorkInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbErr;
        private System.Windows.Forms.Panel pnCam;
        private System.Windows.Forms.ListView lvLot;
        private System.Windows.Forms.Panel pnOperMan;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btMan1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel pnLotOpen;
        private System.Windows.Forms.Button btLotEnd;
        private System.Windows.Forms.Button btLotOpen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btStgHeat;
        private System.Windows.Forms.Button btBldHeat;
        private System.Windows.Forms.Button button3;
    }
}