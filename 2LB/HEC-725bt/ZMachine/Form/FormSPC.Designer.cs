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
            this.sdCSV = new System.Windows.Forms.SaveFileDialog();
            this.sdXLS = new System.Windows.Forms.SaveFileDialog();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btDataView = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dpEndTime = new System.Windows.Forms.DateTimePicker();
            this.button3 = new System.Windows.Forms.Button();
            this.dpSttTime = new System.Windows.Forms.DateTimePicker();
            this.tcData = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lbUptime = new System.Windows.Forms.Label();
            this.btLotDelete = new System.Windows.Forms.Button();
            this.btLotFind = new System.Windows.Forms.Button();
            this.tbLotFind = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btLotSave = new System.Windows.Forms.Button();
            this.lvLot = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbMTTA = new System.Windows.Forms.Label();
            this.lbMTBA = new System.Windows.Forms.Label();
            this.btErrDelete = new System.Windows.Forms.Button();
            this.btErrFind = new System.Windows.Forms.Button();
            this.tbErrFind = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btErrSave = new System.Windows.Forms.Button();
            this.lvError = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btSetRepair = new System.Windows.Forms.Button();
            this.lbMTTR = new System.Windows.Forms.Label();
            this.lbMTBF = new System.Windows.Forms.Label();
            this.btFailDelete = new System.Windows.Forms.Button();
            this.btFailFind = new System.Windows.Forms.Button();
            this.tbFailFind = new System.Windows.Forms.TextBox();
            this.psss = new System.Windows.Forms.Panel();
            this.btFailSave = new System.Windows.Forms.Button();
            this.lvFailure = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pbChart = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tcData.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.psss.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).BeginInit();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.btDataView);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.dpEndTime);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.dpSttTime);
            this.panel1.Controls.Add(this.tcData);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1280, 863);
            this.panel1.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1207, 824);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 22;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1151, 797);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 50);
            this.button2.TabIndex = 15;
            this.button2.Text = "Err";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // btDataView
            // 
            this.btDataView.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btDataView.Location = new System.Drawing.Point(270, 789);
            this.btDataView.Name = "btDataView";
            this.btDataView.Size = new System.Drawing.Size(249, 69);
            this.btDataView.TabIndex = 21;
            this.btDataView.Text = "DATA VIEW";
            this.btDataView.UseVisualStyleBackColor = true;
            this.btDataView.Click += new System.EventHandler(this.btDataView_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(6, 827);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 25);
            this.label5.TabIndex = 20;
            this.label5.Text = "Until";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(6, 792);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 25);
            this.label4.TabIndex = 19;
            this.label4.Text = "From";
            // 
            // dpEndTime
            // 
            this.dpEndTime.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpEndTime.Location = new System.Drawing.Point(66, 827);
            this.dpEndTime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dpEndTime.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dpEndTime.Name = "dpEndTime";
            this.dpEndTime.Size = new System.Drawing.Size(197, 31);
            this.dpEndTime.TabIndex = 18;
            this.dpEndTime.Value = new System.DateTime(2017, 8, 23, 0, 0, 0, 0);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1100, 798);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 50);
            this.button3.TabIndex = 16;
            this.button3.Text = "Lot";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // dpSttTime
            // 
            this.dpSttTime.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dpSttTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpSttTime.Location = new System.Drawing.Point(66, 790);
            this.dpSttTime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dpSttTime.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dpSttTime.Name = "dpSttTime";
            this.dpSttTime.Size = new System.Drawing.Size(197, 31);
            this.dpSttTime.TabIndex = 17;
            this.dpSttTime.Value = new System.DateTime(2017, 8, 23, 0, 0, 0, 0);
            // 
            // tcData
            // 
            this.tcData.Controls.Add(this.tabPage1);
            this.tcData.Controls.Add(this.tabPage2);
            this.tcData.Controls.Add(this.tabPage3);
            this.tcData.Controls.Add(this.tabPage4);
            this.tcData.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tcData.Location = new System.Drawing.Point(-2, 4);
            this.tcData.Name = "tcData";
            this.tcData.SelectedIndex = 0;
            this.tcData.Size = new System.Drawing.Size(1279, 780);
            this.tcData.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.lbUptime);
            this.tabPage1.Controls.Add(this.btLotDelete);
            this.tabPage1.Controls.Add(this.btLotFind);
            this.tabPage1.Controls.Add(this.tbLotFind);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1271, 746);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "LOT";
            // 
            // lbUptime
            // 
            this.lbUptime.AutoSize = true;
            this.lbUptime.Location = new System.Drawing.Point(535, 713);
            this.lbUptime.Name = "lbUptime";
            this.lbUptime.Size = new System.Drawing.Size(98, 21);
            this.lbUptime.TabIndex = 8;
            this.lbUptime.Text = "Uptime(%)=";
            // 
            // btLotDelete
            // 
            this.btLotDelete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotDelete.Location = new System.Drawing.Point(315, 709);
            this.btLotDelete.Name = "btLotDelete";
            this.btLotDelete.Size = new System.Drawing.Size(202, 29);
            this.btLotDelete.TabIndex = 7;
            this.btLotDelete.Tag = "1";
            this.btLotDelete.Text = "Delete Sellected Row";
            this.btLotDelete.UseVisualStyleBackColor = true;
            this.btLotDelete.Click += new System.EventHandler(this.btLotDelete_Click_1);
            // 
            // btLotFind
            // 
            this.btLotFind.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotFind.Location = new System.Drawing.Point(214, 709);
            this.btLotFind.Name = "btLotFind";
            this.btLotFind.Size = new System.Drawing.Size(95, 29);
            this.btLotFind.TabIndex = 6;
            this.btLotFind.Tag = "1";
            this.btLotFind.Text = "FIND";
            this.btLotFind.UseVisualStyleBackColor = true;
            this.btLotFind.Click += new System.EventHandler(this.btLotFind_Click_1);
            // 
            // tbLotFind
            // 
            this.tbLotFind.Location = new System.Drawing.Point(6, 709);
            this.tbLotFind.Name = "tbLotFind";
            this.tbLotFind.Size = new System.Drawing.Size(202, 29);
            this.tbLotFind.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btLotSave);
            this.panel2.Controls.Add(this.lvLot);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(6, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1259, 695);
            this.panel2.TabIndex = 2;
            // 
            // btLotSave
            // 
            this.btLotSave.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLotSave.Location = new System.Drawing.Point(1104, 0);
            this.btLotSave.Name = "btLotSave";
            this.btLotSave.Size = new System.Drawing.Size(153, 41);
            this.btLotSave.TabIndex = 16;
            this.btLotSave.Text = "Lot Data Save";
            this.btLotSave.UseVisualStyleBackColor = true;
            this.btLotSave.Click += new System.EventHandler(this.btLotSave_Click_1);
            // 
            // lvLot
            // 
            this.lvLot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLot.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvLot.Location = new System.Drawing.Point(0, 41);
            this.lvLot.Name = "lvLot";
            this.lvLot.Size = new System.Drawing.Size(1257, 652);
            this.lvLot.TabIndex = 1;
            this.lvLot.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1257, 41);
            this.label2.TabIndex = 0;
            this.label2.Text = "LOT DATA";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.lbMTTA);
            this.tabPage2.Controls.Add(this.lbMTBA);
            this.tabPage2.Controls.Add(this.btErrDelete);
            this.tabPage2.Controls.Add(this.btErrFind);
            this.tabPage2.Controls.Add(this.tbErrFind);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1271, 746);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ERROR";
            // 
            // lbMTTA
            // 
            this.lbMTTA.AutoSize = true;
            this.lbMTTA.Location = new System.Drawing.Point(755, 713);
            this.lbMTTA.Name = "lbMTTA";
            this.lbMTTA.Size = new System.Drawing.Size(65, 21);
            this.lbMTTA.TabIndex = 13;
            this.lbMTTA.Text = "MTTA=";
            // 
            // lbMTBA
            // 
            this.lbMTBA.AutoSize = true;
            this.lbMTBA.Location = new System.Drawing.Point(535, 713);
            this.lbMTBA.Name = "lbMTBA";
            this.lbMTBA.Size = new System.Drawing.Size(65, 21);
            this.lbMTBA.TabIndex = 12;
            this.lbMTBA.Text = "MTBA=";
            // 
            // btErrDelete
            // 
            this.btErrDelete.Location = new System.Drawing.Point(315, 709);
            this.btErrDelete.Name = "btErrDelete";
            this.btErrDelete.Size = new System.Drawing.Size(202, 29);
            this.btErrDelete.TabIndex = 11;
            this.btErrDelete.Tag = "2";
            this.btErrDelete.Text = "Delete Sellected Row";
            this.btErrDelete.UseVisualStyleBackColor = true;
            this.btErrDelete.Click += new System.EventHandler(this.btLotDelete_Click_1);
            // 
            // btErrFind
            // 
            this.btErrFind.Location = new System.Drawing.Point(214, 709);
            this.btErrFind.Name = "btErrFind";
            this.btErrFind.Size = new System.Drawing.Size(95, 29);
            this.btErrFind.TabIndex = 10;
            this.btErrFind.Tag = "2";
            this.btErrFind.Text = "FIND";
            this.btErrFind.UseVisualStyleBackColor = true;
            this.btErrFind.Click += new System.EventHandler(this.btLotFind_Click_1);
            // 
            // tbErrFind
            // 
            this.tbErrFind.Location = new System.Drawing.Point(6, 709);
            this.tbErrFind.Name = "tbErrFind";
            this.tbErrFind.Size = new System.Drawing.Size(202, 29);
            this.tbErrFind.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btErrSave);
            this.panel3.Controls.Add(this.lvError);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(6, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1259, 695);
            this.panel3.TabIndex = 1;
            // 
            // btErrSave
            // 
            this.btErrSave.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btErrSave.Location = new System.Drawing.Point(1104, 0);
            this.btErrSave.Name = "btErrSave";
            this.btErrSave.Size = new System.Drawing.Size(153, 41);
            this.btErrSave.TabIndex = 16;
            this.btErrSave.Text = "Error Data Save";
            this.btErrSave.UseVisualStyleBackColor = true;
            this.btErrSave.Click += new System.EventHandler(this.btErrSave_Click_1);
            // 
            // lvError
            // 
            this.lvError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvError.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvError.Location = new System.Drawing.Point(0, 41);
            this.lvError.Name = "lvError";
            this.lvError.Size = new System.Drawing.Size(1257, 652);
            this.lvError.TabIndex = 1;
            this.lvError.UseCompatibleStateImageBehavior = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1257, 41);
            this.label3.TabIndex = 0;
            this.label3.Text = "ERROR DATA";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.btSetRepair);
            this.tabPage3.Controls.Add(this.lbMTTR);
            this.tabPage3.Controls.Add(this.lbMTBF);
            this.tabPage3.Controls.Add(this.btFailDelete);
            this.tabPage3.Controls.Add(this.btFailFind);
            this.tabPage3.Controls.Add(this.tbFailFind);
            this.tabPage3.Controls.Add(this.psss);
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1271, 746);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "FAILURE";
            // 
            // btSetRepair
            // 
            this.btSetRepair.Location = new System.Drawing.Point(1129, 709);
            this.btSetRepair.Name = "btSetRepair";
            this.btSetRepair.Size = new System.Drawing.Size(135, 29);
            this.btSetRepair.TabIndex = 18;
            this.btSetRepair.Text = "SET REPAIR";
            this.btSetRepair.UseVisualStyleBackColor = true;
            this.btSetRepair.Click += new System.EventHandler(this.btSetRepair_Click_1);
            // 
            // lbMTTR
            // 
            this.lbMTTR.AutoSize = true;
            this.lbMTTR.Location = new System.Drawing.Point(755, 713);
            this.lbMTTR.Name = "lbMTTR";
            this.lbMTTR.Size = new System.Drawing.Size(64, 21);
            this.lbMTTR.TabIndex = 17;
            this.lbMTTR.Text = "MTTR=";
            // 
            // lbMTBF
            // 
            this.lbMTBF.AutoSize = true;
            this.lbMTBF.Location = new System.Drawing.Point(535, 713);
            this.lbMTBF.Name = "lbMTBF";
            this.lbMTBF.Size = new System.Drawing.Size(62, 21);
            this.lbMTBF.TabIndex = 16;
            this.lbMTBF.Text = "MTBF=";
            // 
            // btFailDelete
            // 
            this.btFailDelete.Location = new System.Drawing.Point(315, 709);
            this.btFailDelete.Name = "btFailDelete";
            this.btFailDelete.Size = new System.Drawing.Size(202, 29);
            this.btFailDelete.TabIndex = 15;
            this.btFailDelete.Tag = "3";
            this.btFailDelete.Text = "Delete Sellected Row";
            this.btFailDelete.UseVisualStyleBackColor = true;
            this.btFailDelete.Click += new System.EventHandler(this.btLotDelete_Click_1);
            // 
            // btFailFind
            // 
            this.btFailFind.Location = new System.Drawing.Point(214, 709);
            this.btFailFind.Name = "btFailFind";
            this.btFailFind.Size = new System.Drawing.Size(95, 29);
            this.btFailFind.TabIndex = 14;
            this.btFailFind.Tag = "3";
            this.btFailFind.Text = "FIND";
            this.btFailFind.UseVisualStyleBackColor = true;
            this.btFailFind.Click += new System.EventHandler(this.btLotFind_Click_1);
            // 
            // tbFailFind
            // 
            this.tbFailFind.Location = new System.Drawing.Point(6, 709);
            this.tbFailFind.Name = "tbFailFind";
            this.tbFailFind.Size = new System.Drawing.Size(202, 29);
            this.tbFailFind.TabIndex = 13;
            // 
            // psss
            // 
            this.psss.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.psss.Controls.Add(this.btFailSave);
            this.psss.Controls.Add(this.lvFailure);
            this.psss.Controls.Add(this.label1);
            this.psss.Location = new System.Drawing.Point(6, 5);
            this.psss.Name = "psss";
            this.psss.Size = new System.Drawing.Size(1259, 695);
            this.psss.TabIndex = 2;
            // 
            // btFailSave
            // 
            this.btFailSave.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btFailSave.Location = new System.Drawing.Point(1104, 0);
            this.btFailSave.Name = "btFailSave";
            this.btFailSave.Size = new System.Drawing.Size(153, 41);
            this.btFailSave.TabIndex = 15;
            this.btFailSave.Text = "Failure Data Save";
            this.btFailSave.UseVisualStyleBackColor = true;
            this.btFailSave.Click += new System.EventHandler(this.btFailSave_Click_1);
            // 
            // lvFailure
            // 
            this.lvFailure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFailure.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvFailure.Location = new System.Drawing.Point(0, 41);
            this.lvFailure.Name = "lvFailure";
            this.lvFailure.Size = new System.Drawing.Size(1257, 652);
            this.lvFailure.TabIndex = 1;
            this.lvFailure.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1257, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "FAILURE DATA";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.panel4);
            this.tabPage4.Location = new System.Drawing.Point(4, 30);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1271, 746);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "CHART";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.pbChart);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(6, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1259, 695);
            this.panel4.TabIndex = 3;
            // 
            // pbChart
            // 
            this.pbChart.BackColor = System.Drawing.Color.White;
            this.pbChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbChart.Location = new System.Drawing.Point(0, 41);
            this.pbChart.Name = "pbChart";
            this.pbChart.Size = new System.Drawing.Size(1257, 652);
            this.pbChart.TabIndex = 1;
            this.pbChart.TabStop = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(1257, 41);
            this.label6.TabIndex = 0;
            this.label6.Text = "REJECT TREND";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormSPC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSPC";
            this.Text = "FormSPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSPC_FormClosing);
            this.Shown += new System.EventHandler(this.FormSPC_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormSPC_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog sdCSV;
        private System.Windows.Forms.SaveFileDialog sdXLS;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btDataView;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dpEndTime;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DateTimePicker dpSttTime;
        private System.Windows.Forms.TabControl tcData;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lbUptime;
        private System.Windows.Forms.Button btLotDelete;
        private System.Windows.Forms.Button btLotFind;
        private System.Windows.Forms.TextBox tbLotFind;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btLotSave;
        private System.Windows.Forms.ListView lvLot;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lbMTTA;
        private System.Windows.Forms.Label lbMTBA;
        private System.Windows.Forms.Button btErrDelete;
        private System.Windows.Forms.Button btErrFind;
        private System.Windows.Forms.TextBox tbErrFind;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btErrSave;
        private System.Windows.Forms.ListView lvError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btSetRepair;
        private System.Windows.Forms.Label lbMTTR;
        private System.Windows.Forms.Label lbMTBF;
        private System.Windows.Forms.Button btFailDelete;
        private System.Windows.Forms.Button btFailFind;
        private System.Windows.Forms.TextBox tbFailFind;
        private System.Windows.Forms.Panel psss;
        private System.Windows.Forms.Button btFailSave;
        private System.Windows.Forms.ListView lvFailure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox pbChart;
        private System.Windows.Forms.Label label6;
    }
}