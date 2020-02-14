namespace Machine
{
    partial class FormSPC
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
            this.tcData = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btLotSave = new System.Windows.Forms.Button();
            this.lvLot = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.btLotDelete = new System.Windows.Forms.Button();
            this.tbLotFind = new System.Windows.Forms.TextBox();
            this.btLotFind = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btErrSave = new System.Windows.Forms.Button();
            this.lvError = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.tbErrFind = new System.Windows.Forms.TextBox();
            this.btErrDelete = new System.Windows.Forms.Button();
            this.btErrFind = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btSetRepair = new System.Windows.Forms.Button();
            this.psss = new System.Windows.Forms.Panel();
            this.btFailSave = new System.Windows.Forms.Button();
            this.lvFailure = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFailFind = new System.Windows.Forms.TextBox();
            this.btFailDelete = new System.Windows.Forms.Button();
            this.btFailFind = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbChart = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbUptime = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.sdCSV = new System.Windows.Forms.SaveFileDialog();
            this.sdXLS = new System.Windows.Forms.SaveFileDialog();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.dpSttTime = new System.Windows.Forms.DateTimePicker();
            this.dpEndTime = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btDataView = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.lbMTBA = new System.Windows.Forms.Label();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbMTTA = new System.Windows.Forms.Label();
            this.lbMTTR = new System.Windows.Forms.Label();
            this.lbMTBF = new System.Windows.Forms.Label();
            this.tcData.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.psss.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcData
            // 
            this.tcData.Controls.Add(this.tabPage1);
            this.tcData.Controls.Add(this.tabPage2);
            this.tcData.Controls.Add(this.tabPage3);
            this.tcData.Controls.Add(this.tabPage4);
            this.tcData.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tcData.Location = new System.Drawing.Point(1, 71);
            this.tcData.Name = "tcData";
            this.tcData.SelectedIndex = 0;
            this.tcData.Size = new System.Drawing.Size(1283, 780);
            this.tcData.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.btLotDelete);
            this.tabPage1.Controls.Add(this.tbLotFind);
            this.tabPage1.Controls.Add(this.btLotFind);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1275, 746);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "LOT";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btLotSave);
            this.panel2.Controls.Add(this.lvLot);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(6, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1261, 780);
            this.panel2.TabIndex = 2;
            // 
            // btLotSave
            // 
            this.btLotSave.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotSave.Location = new System.Drawing.Point(1106, -2);
            this.btLotSave.Name = "btLotSave";
            this.btLotSave.Size = new System.Drawing.Size(153, 45);
            this.btLotSave.TabIndex = 16;
            this.btLotSave.Text = "Lot Data Save";
            this.btLotSave.UseVisualStyleBackColor = true;
            this.btLotSave.Click += new System.EventHandler(this.btLotSave_Click);
            // 
            // lvLot
            // 
            this.lvLot.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvLot.Location = new System.Drawing.Point(-1, 41);
            this.lvLot.Name = "lvLot";
            this.lvLot.Size = new System.Drawing.Size(1259, 661);
            this.lvLot.TabIndex = 1;
            this.lvLot.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.MistyRose;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1257, 41);
            this.label2.TabIndex = 0;
            this.label2.Text = "LOT DATA";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btLotDelete
            // 
            this.btLotDelete.Location = new System.Drawing.Point(331, 6);
            this.btLotDelete.Name = "btLotDelete";
            this.btLotDelete.Size = new System.Drawing.Size(266, 28);
            this.btLotDelete.TabIndex = 7;
            this.btLotDelete.Tag = "1";
            this.btLotDelete.Text = "Delete Sellected Row";
            this.btLotDelete.UseVisualStyleBackColor = true;
            this.btLotDelete.Click += new System.EventHandler(this.btLotDelete_Click);
            // 
            // tbLotFind
            // 
            this.tbLotFind.Location = new System.Drawing.Point(6, 6);
            this.tbLotFind.Name = "tbLotFind";
            this.tbLotFind.Size = new System.Drawing.Size(202, 30);
            this.tbLotFind.TabIndex = 5;
            // 
            // btLotFind
            // 
            this.btLotFind.Location = new System.Drawing.Point(217, 6);
            this.btLotFind.Name = "btLotFind";
            this.btLotFind.Size = new System.Drawing.Size(94, 28);
            this.btLotFind.TabIndex = 6;
            this.btLotFind.Tag = "1";
            this.btLotFind.Text = "FIND";
            this.btLotFind.UseVisualStyleBackColor = true;
            this.btLotFind.Click += new System.EventHandler(this.btLotFind_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Controls.Add(this.tbErrFind);
            this.tabPage2.Controls.Add(this.btErrDelete);
            this.tabPage2.Controls.Add(this.btErrFind);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1275, 746);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ERROR";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btErrSave);
            this.panel3.Controls.Add(this.lvError);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(6, 40);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1261, 780);
            this.panel3.TabIndex = 1;
            // 
            // btErrSave
            // 
            this.btErrSave.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btErrSave.Location = new System.Drawing.Point(1106, -2);
            this.btErrSave.Name = "btErrSave";
            this.btErrSave.Size = new System.Drawing.Size(153, 45);
            this.btErrSave.TabIndex = 16;
            this.btErrSave.Text = "Error Data Save";
            this.btErrSave.UseVisualStyleBackColor = true;
            this.btErrSave.Click += new System.EventHandler(this.btErrSave_Click);
            // 
            // lvError
            // 
            this.lvError.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvError.Location = new System.Drawing.Point(-1, 41);
            this.lvError.Name = "lvError";
            this.lvError.Size = new System.Drawing.Size(1259, 661);
            this.lvError.TabIndex = 1;
            this.lvError.UseCompatibleStateImageBehavior = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.MistyRose;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1257, 41);
            this.label3.TabIndex = 0;
            this.label3.Text = "ERROR DATA";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbErrFind
            // 
            this.tbErrFind.Location = new System.Drawing.Point(6, 6);
            this.tbErrFind.Name = "tbErrFind";
            this.tbErrFind.Size = new System.Drawing.Size(202, 30);
            this.tbErrFind.TabIndex = 9;
            // 
            // btErrDelete
            // 
            this.btErrDelete.Location = new System.Drawing.Point(331, 6);
            this.btErrDelete.Name = "btErrDelete";
            this.btErrDelete.Size = new System.Drawing.Size(266, 28);
            this.btErrDelete.TabIndex = 11;
            this.btErrDelete.Tag = "2";
            this.btErrDelete.Text = "Delete Sellected Row";
            this.btErrDelete.UseVisualStyleBackColor = true;
            this.btErrDelete.Click += new System.EventHandler(this.btLotDelete_Click);
            // 
            // btErrFind
            // 
            this.btErrFind.Location = new System.Drawing.Point(217, 6);
            this.btErrFind.Name = "btErrFind";
            this.btErrFind.Size = new System.Drawing.Size(94, 28);
            this.btErrFind.TabIndex = 10;
            this.btErrFind.Tag = "2";
            this.btErrFind.Text = "FIND";
            this.btErrFind.UseVisualStyleBackColor = true;
            this.btErrFind.Click += new System.EventHandler(this.btLotFind_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btSetRepair);
            this.tabPage3.Controls.Add(this.psss);
            this.tabPage3.Controls.Add(this.tbFailFind);
            this.tabPage3.Controls.Add(this.btFailDelete);
            this.tabPage3.Controls.Add(this.btFailFind);
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1275, 746);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "FAILURE";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btSetRepair
            // 
            this.btSetRepair.Location = new System.Drawing.Point(1129, 3);
            this.btSetRepair.Name = "btSetRepair";
            this.btSetRepair.Size = new System.Drawing.Size(135, 28);
            this.btSetRepair.TabIndex = 18;
            this.btSetRepair.Text = "SET REPAIR";
            this.btSetRepair.UseVisualStyleBackColor = true;
            this.btSetRepair.Click += new System.EventHandler(this.btSetRepair_Click);
            // 
            // psss
            // 
            this.psss.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.psss.Controls.Add(this.btFailSave);
            this.psss.Controls.Add(this.lvFailure);
            this.psss.Controls.Add(this.label1);
            this.psss.Location = new System.Drawing.Point(6, 40);
            this.psss.Name = "psss";
            this.psss.Size = new System.Drawing.Size(1261, 780);
            this.psss.TabIndex = 2;
            // 
            // btFailSave
            // 
            this.btFailSave.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btFailSave.Location = new System.Drawing.Point(1106, -2);
            this.btFailSave.Name = "btFailSave";
            this.btFailSave.Size = new System.Drawing.Size(153, 45);
            this.btFailSave.TabIndex = 15;
            this.btFailSave.Text = "Failure Data Save";
            this.btFailSave.UseVisualStyleBackColor = true;
            this.btFailSave.Click += new System.EventHandler(this.btFailSave_Click);
            // 
            // lvFailure
            // 
            this.lvFailure.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvFailure.Location = new System.Drawing.Point(-1, 41);
            this.lvFailure.Name = "lvFailure";
            this.lvFailure.Size = new System.Drawing.Size(1259, 661);
            this.lvFailure.TabIndex = 1;
            this.lvFailure.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.MistyRose;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1257, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "FAILURE DATA";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbFailFind
            // 
            this.tbFailFind.Location = new System.Drawing.Point(6, 5);
            this.tbFailFind.Name = "tbFailFind";
            this.tbFailFind.Size = new System.Drawing.Size(202, 30);
            this.tbFailFind.TabIndex = 13;
            // 
            // btFailDelete
            // 
            this.btFailDelete.Location = new System.Drawing.Point(331, 6);
            this.btFailDelete.Name = "btFailDelete";
            this.btFailDelete.Size = new System.Drawing.Size(266, 28);
            this.btFailDelete.TabIndex = 15;
            this.btFailDelete.Tag = "3";
            this.btFailDelete.Text = "Delete Sellected Row";
            this.btFailDelete.UseVisualStyleBackColor = true;
            this.btFailDelete.Click += new System.EventHandler(this.btLotDelete_Click);
            // 
            // btFailFind
            // 
            this.btFailFind.Location = new System.Drawing.Point(217, 6);
            this.btFailFind.Name = "btFailFind";
            this.btFailFind.Size = new System.Drawing.Size(94, 28);
            this.btFailFind.TabIndex = 14;
            this.btFailFind.Tag = "3";
            this.btFailFind.Text = "FIND";
            this.btFailFind.UseVisualStyleBackColor = true;
            this.btFailFind.Click += new System.EventHandler(this.btLotFind_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 30);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1275, 746);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "CHART";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pbChart);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(6, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1261, 700);
            this.panel1.TabIndex = 3;
            // 
            // pbChart
            // 
            this.pbChart.BackColor = System.Drawing.Color.White;
            this.pbChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbChart.Location = new System.Drawing.Point(0, 41);
            this.pbChart.Name = "pbChart";
            this.pbChart.Size = new System.Drawing.Size(1259, 657);
            this.pbChart.TabIndex = 1;
            this.pbChart.TabStop = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.MistyRose;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(1259, 41);
            this.label6.TabIndex = 0;
            this.label6.Text = "REJECT TREND";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbUptime
            // 
            this.lbUptime.AutoSize = true;
            this.lbUptime.Location = new System.Drawing.Point(6, 8);
            this.lbUptime.Name = "lbUptime";
            this.lbUptime.Size = new System.Drawing.Size(82, 15);
            this.lbUptime.TabIndex = 8;
            this.lbUptime.Text = "Uptime(%)=";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1102, 794);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 50);
            this.button3.TabIndex = 4;
            this.button3.Text = "Lot";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1153, 793);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 50);
            this.button2.TabIndex = 4;
            this.button2.Text = "Err";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // sdXLS
            // 
            this.sdXLS.FileOk += new System.ComponentModel.CancelEventHandler(this.sdXLS_FileOk);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1209, 820);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 13;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dpSttTime
            // 
            this.dpSttTime.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dpSttTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpSttTime.Location = new System.Drawing.Point(346, 8);
            this.dpSttTime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dpSttTime.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dpSttTime.Name = "dpSttTime";
            this.dpSttTime.Size = new System.Drawing.Size(197, 24);
            this.dpSttTime.TabIndex = 8;
            this.dpSttTime.Value = new System.DateTime(2017, 8, 23, 0, 0, 0, 0);
            // 
            // dpEndTime
            // 
            this.dpEndTime.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpEndTime.Location = new System.Drawing.Point(346, 35);
            this.dpEndTime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dpEndTime.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dpEndTime.Name = "dpEndTime";
            this.dpEndTime.Size = new System.Drawing.Size(197, 24);
            this.dpEndTime.TabIndex = 9;
            this.dpEndTime.Value = new System.DateTime(2017, 8, 23, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.DarkGray;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(231, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 22);
            this.label4.TabIndex = 10;
            this.label4.Text = "Start Date";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.DarkGray;
            this.label5.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(231, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 22);
            this.label5.TabIndex = 11;
            this.label5.Text = "End Date";
            // 
            // btDataView
            // 
            this.btDataView.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btDataView.Location = new System.Drawing.Point(5, 8);
            this.btDataView.Name = "btDataView";
            this.btDataView.Size = new System.Drawing.Size(216, 49);
            this.btDataView.TabIndex = 12;
            this.btDataView.Text = "Data View";
            this.btDataView.UseVisualStyleBackColor = true;
            this.btDataView.Click += new System.EventHandler(this.btDataView_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControl1.Location = new System.Drawing.Point(568, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(434, 88);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label7);
            this.tabPage6.Controls.Add(this.lbMTBA);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(426, 59);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "MTBA";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.DarkGray;
            this.label7.Font = new System.Drawing.Font("굴림", 22F);
            this.label7.Location = new System.Drawing.Point(18, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 30);
            this.label7.TabIndex = 14;
            this.label7.Text = "  MTBA   ";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // lbMTBA
            // 
            this.lbMTBA.AutoSize = true;
            this.lbMTBA.BackColor = System.Drawing.Color.Transparent;
            this.lbMTBA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMTBA.Font = new System.Drawing.Font("굴림", 22F);
            this.lbMTBA.ForeColor = System.Drawing.Color.Red;
            this.lbMTBA.Location = new System.Drawing.Point(168, 12);
            this.lbMTBA.Name = "lbMTBA";
            this.lbMTBA.Size = new System.Drawing.Size(89, 32);
            this.lbMTBA.TabIndex = 13;
            this.lbMTBA.Text = " 2.2  ";
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.lbUptime);
            this.tabPage8.Controls.Add(this.label12);
            this.tabPage8.Controls.Add(this.label13);
            this.tabPage8.Controls.Add(this.label14);
            this.tabPage8.Controls.Add(this.label9);
            this.tabPage8.Controls.Add(this.label10);
            this.tabPage8.Controls.Add(this.label11);
            this.tabPage8.Controls.Add(this.lbMTTA);
            this.tabPage8.Controls.Add(this.lbMTTR);
            this.tabPage8.Controls.Add(this.lbMTBF);
            this.tabPage8.Location = new System.Drawing.Point(4, 25);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(426, 59);
            this.tabPage8.TabIndex = 3;
            this.tabPage8.Text = "Others";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.DarkGray;
            this.label12.Font = new System.Drawing.Font("굴림", 11F);
            this.label12.Location = new System.Drawing.Point(334, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 15);
            this.label12.TabIndex = 24;
            this.label12.Text = "sec";
            this.label12.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.DarkGray;
            this.label13.Font = new System.Drawing.Font("굴림", 11F);
            this.label13.Location = new System.Drawing.Point(320, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 15);
            this.label13.TabIndex = 26;
            this.label13.Text = "min";
            this.label13.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.DarkGray;
            this.label14.Font = new System.Drawing.Font("굴림", 11F);
            this.label14.Location = new System.Drawing.Point(122, 34);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 15);
            this.label14.TabIndex = 25;
            this.label14.Text = "H";
            this.label14.Visible = false;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.DarkGray;
            this.label9.Font = new System.Drawing.Font("굴림", 11F);
            this.label9.Location = new System.Drawing.Point(218, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 17);
            this.label9.TabIndex = 21;
            this.label9.Text = "MTTA";
            this.label9.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.DarkGray;
            this.label10.Font = new System.Drawing.Font("굴림", 11F);
            this.label10.Location = new System.Drawing.Point(204, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 15);
            this.label10.TabIndex = 23;
            this.label10.Text = "MTTR";
            this.label10.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.DarkGray;
            this.label11.Font = new System.Drawing.Font("굴림", 11F);
            this.label11.Location = new System.Drawing.Point(6, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "MTBF";
            this.label11.Visible = false;
            // 
            // lbMTTA
            // 
            this.lbMTTA.AutoSize = true;
            this.lbMTTA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMTTA.Font = new System.Drawing.Font("굴림", 11F);
            this.lbMTTA.Location = new System.Drawing.Point(272, 32);
            this.lbMTTA.Name = "lbMTTA";
            this.lbMTTA.Size = new System.Drawing.Size(54, 17);
            this.lbMTTA.TabIndex = 18;
            this.lbMTTA.Text = "MTTA=";
            this.lbMTTA.Visible = false;
            // 
            // lbMTTR
            // 
            this.lbMTTR.AutoSize = true;
            this.lbMTTR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMTTR.Font = new System.Drawing.Font("굴림", 11F);
            this.lbMTTR.Location = new System.Drawing.Point(259, 10);
            this.lbMTTR.Name = "lbMTTR";
            this.lbMTTR.Size = new System.Drawing.Size(55, 17);
            this.lbMTTR.TabIndex = 20;
            this.lbMTTR.Text = "MTTR=";
            this.lbMTTR.Visible = false;
            // 
            // lbMTBF
            // 
            this.lbMTBF.AutoSize = true;
            this.lbMTBF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMTBF.Font = new System.Drawing.Font("굴림", 11F);
            this.lbMTBF.Location = new System.Drawing.Point(60, 34);
            this.lbMTBF.Name = "lbMTBF";
            this.lbMTBF.Size = new System.Drawing.Size(55, 17);
            this.lbMTBF.TabIndex = 19;
            this.lbMTBF.Text = "MTBF=";
            this.lbMTBF.Visible = false;
            // 
            // FormSPC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1300, 863);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btDataView);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dpEndTime);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.dpSttTime);
            this.Controls.Add(this.tcData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSPC";
            this.Text = "FormSPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSPC_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FormSPC_VisibleChanged);
            this.tcData.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.psss.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcData;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SaveFileDialog sdCSV;
        private System.Windows.Forms.SaveFileDialog sdXLS;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView lvLot;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel psss;
        private System.Windows.Forms.ListView lvFailure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btFailDelete;
        private System.Windows.Forms.Button btFailFind;
        private System.Windows.Forms.TextBox tbFailFind;
        private System.Windows.Forms.Button btSetRepair;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pbChart;
        private System.Windows.Forms.Button btLotSave;
        private System.Windows.Forms.Button btFailSave;
        private System.Windows.Forms.Label lbUptime;
        private System.Windows.Forms.Button btLotDelete;
        private System.Windows.Forms.TextBox tbLotFind;
        private System.Windows.Forms.Button btLotFind;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btErrSave;
        private System.Windows.Forms.ListView lvError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbErrFind;
        private System.Windows.Forms.Button btErrDelete;
        private System.Windows.Forms.Button btErrFind;
        private System.Windows.Forms.DateTimePicker dpSttTime;
        private System.Windows.Forms.DateTimePicker dpEndTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btDataView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label lbMTBA;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Label lbMTTA;
        private System.Windows.Forms.Label lbMTTR;
        private System.Windows.Forms.Label lbMTBF;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}