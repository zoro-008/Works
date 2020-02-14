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
            this.lbVer = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.tbXrayRptCnt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tbCrntLvNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tbSetLvNo = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbIgnrSerialErr = new System.Windows.Forms.CheckBox();
            this.cbIgnrCnctErr = new System.Windows.Forms.CheckBox();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.cbBarcodeSkip = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.tbEzSensorPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tbDressyPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbSkipEntr = new System.Windows.Forms.CheckBox();
            this.cbSkipAging = new System.Windows.Forms.CheckBox();
            this.cbSkipMTF = new System.Windows.Forms.CheckBox();
            this.cbSkipCalib = new System.Windows.Forms.CheckBox();
            this.cbSkipSkull = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbBuzzOffTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btRecordPath = new System.Windows.Forms.Button();
            this.tbRecordPath = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbUseRecord = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbVer
            // 
            this.lbVer.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbVer.Location = new System.Drawing.Point(9, 786);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(230, 30);
            this.lbVer.TabIndex = 1;
            this.lbVer.Text = "Version : 1.1.1.1";
            // 
            // lbDate
            // 
            this.lbDate.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDate.Location = new System.Drawing.Point(9, 820);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(230, 30);
            this.lbDate.TabIndex = 2;
            this.lbDate.Text = "1983-11-08 12:35:33";
            // 
            // btSave
            // 
            this.btSave.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSave.Image = ((System.Drawing.Image)(resources.GetObject("btSave.Image")));
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSave.Location = new System.Drawing.Point(1092, 12);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(176, 52);
            this.btSave.TabIndex = 3;
            this.btSave.Text = " SAVE";
            this.btSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbXrayRptCnt);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.tbCrntLvNo);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.tbSetLvNo);
            this.panel1.Controls.Add(this.label25);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 138);
            this.panel1.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(332, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 21);
            this.label4.TabIndex = 32;
            this.label4.Text = "회";
            // 
            // tbXrayRptCnt
            // 
            this.tbXrayRptCnt.Location = new System.Drawing.Point(222, 100);
            this.tbXrayRptCnt.Name = "tbXrayRptCnt";
            this.tbXrayRptCnt.Size = new System.Drawing.Size(104, 23);
            this.tbXrayRptCnt.TabIndex = 31;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 21);
            this.label5.TabIndex = 30;
            this.label5.Text = "X-Ray 조사 반복 횟수";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(332, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "변경";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbCrntLvNo
            // 
            this.tbCrntLvNo.Enabled = false;
            this.tbCrntLvNo.Location = new System.Drawing.Point(222, 42);
            this.tbCrntLvNo.Name = "tbCrntLvNo";
            this.tbCrntLvNo.Size = new System.Drawing.Size(104, 23);
            this.tbCrntLvNo.TabIndex = 25;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(9, 42);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(172, 21);
            this.label26.TabIndex = 24;
            this.label26.Text = "현재 워크 리스트 번호";
            // 
            // tbSetLvNo
            // 
            this.tbSetLvNo.Location = new System.Drawing.Point(222, 71);
            this.tbSetLvNo.Name = "tbSetLvNo";
            this.tbSetLvNo.Size = new System.Drawing.Size(104, 23);
            this.tbSetLvNo.TabIndex = 23;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(7, 73);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(172, 21);
            this.label25.TabIndex = 22;
            this.label25.Text = "워크 리스트 번호 변경";
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label27.Location = new System.Drawing.Point(-1, -1);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(414, 35);
            this.label27.TabIndex = 0;
            this.label27.Text = "WORK OPTION";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cbIgnrSerialErr);
            this.panel2.Controls.Add(this.cbIgnrCnctErr);
            this.panel2.Controls.Add(this.cbIgnrDoor);
            this.panel2.Controls.Add(this.cbBarcodeSkip);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(0, 137);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 170);
            this.panel2.TabIndex = 55;
            // 
            // cbIgnrSerialErr
            // 
            this.cbIgnrSerialErr.AutoSize = true;
            this.cbIgnrSerialErr.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbIgnrSerialErr.Location = new System.Drawing.Point(9, 139);
            this.cbIgnrSerialErr.Name = "cbIgnrSerialErr";
            this.cbIgnrSerialErr.Size = new System.Drawing.Size(229, 25);
            this.cbIgnrSerialErr.TabIndex = 16;
            this.cbIgnrSerialErr.Text = "시리얼 넘버 매칭 에러 무시";
            this.cbIgnrSerialErr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbIgnrSerialErr.UseVisualStyleBackColor = true;
            // 
            // cbIgnrCnctErr
            // 
            this.cbIgnrCnctErr.AutoSize = true;
            this.cbIgnrCnctErr.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbIgnrCnctErr.Location = new System.Drawing.Point(9, 108);
            this.cbIgnrCnctErr.Name = "cbIgnrCnctErr";
            this.cbIgnrCnctErr.Size = new System.Drawing.Size(190, 25);
            this.cbIgnrCnctErr.TabIndex = 15;
            this.cbIgnrCnctErr.Text = "USB 커넥트 에러 무시";
            this.cbIgnrCnctErr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbIgnrCnctErr.UseVisualStyleBackColor = true;
            // 
            // cbIgnrDoor
            // 
            this.cbIgnrDoor.AutoSize = true;
            this.cbIgnrDoor.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbIgnrDoor.Location = new System.Drawing.Point(9, 77);
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.Size = new System.Drawing.Size(159, 25);
            this.cbIgnrDoor.TabIndex = 14;
            this.cbIgnrDoor.Text = "외부 문 열림 무시";
            this.cbIgnrDoor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
            // 
            // cbBarcodeSkip
            // 
            this.cbBarcodeSkip.AutoSize = true;
            this.cbBarcodeSkip.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbBarcodeSkip.Location = new System.Drawing.Point(9, 46);
            this.cbBarcodeSkip.Name = "cbBarcodeSkip";
            this.cbBarcodeSkip.Size = new System.Drawing.Size(115, 25);
            this.cbBarcodeSkip.TabIndex = 13;
            this.cbBarcodeSkip.Text = "바코드 무시";
            this.cbBarcodeSkip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbBarcodeSkip.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(412, 35);
            this.label2.TabIndex = 3;
            this.label2.Text = "IGNORE OPTION";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.tbEzSensorPath);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Controls.Add(this.tbDressyPath);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(0, 306);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(414, 156);
            this.panel3.TabIndex = 56;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(379, 118);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(31, 23);
            this.button3.TabIndex = 33;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tbEzSensorPath
            // 
            this.tbEzSensorPath.Location = new System.Drawing.Point(3, 119);
            this.tbEzSensorPath.Name = "tbEzSensorPath";
            this.tbEzSensorPath.Size = new System.Drawing.Size(371, 23);
            this.tbEzSensorPath.TabIndex = 32;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(1, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(282, 21);
            this.label8.TabIndex = 31;
            this.label8.Text = "EzSensor 시리얼 넘버 CSV 파일 경로";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(379, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(31, 23);
            this.button2.TabIndex = 30;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tbDressyPath
            // 
            this.tbDressyPath.Location = new System.Drawing.Point(3, 69);
            this.tbDressyPath.Name = "tbDressyPath";
            this.tbDressyPath.Size = new System.Drawing.Size(371, 23);
            this.tbDressyPath.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(1, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(264, 21);
            this.label7.TabIndex = 26;
            this.label7.Text = "Dressy 시리얼 넘버 CSV 파일 경로";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(412, 35);
            this.label6.TabIndex = 3;
            this.label6.Text = "PATH OPTION";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbSkipEntr
            // 
            this.cbSkipEntr.AutoSize = true;
            this.cbSkipEntr.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSkipEntr.Location = new System.Drawing.Point(6, 42);
            this.cbSkipEntr.Name = "cbSkipEntr";
            this.cbSkipEntr.Size = new System.Drawing.Size(175, 25);
            this.cbSkipEntr.TabIndex = 57;
            this.cbSkipEntr.Text = "입고 공정 작업 무시";
            this.cbSkipEntr.UseVisualStyleBackColor = true;
            // 
            // cbSkipAging
            // 
            this.cbSkipAging.AutoSize = true;
            this.cbSkipAging.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSkipAging.Location = new System.Drawing.Point(6, 68);
            this.cbSkipAging.Name = "cbSkipAging";
            this.cbSkipAging.Size = new System.Drawing.Size(188, 25);
            this.cbSkipAging.TabIndex = 58;
            this.cbSkipAging.Text = "Aging 공정 작업 무시";
            this.cbSkipAging.UseVisualStyleBackColor = true;
            // 
            // cbSkipMTF
            // 
            this.cbSkipMTF.AutoSize = true;
            this.cbSkipMTF.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSkipMTF.Location = new System.Drawing.Point(6, 95);
            this.cbSkipMTF.Name = "cbSkipMTF";
            this.cbSkipMTF.Size = new System.Drawing.Size(214, 25);
            this.cbSkipMTF.TabIndex = 59;
            this.cbSkipMTF.Text = "MTF/NPS 공정 작업 무시";
            this.cbSkipMTF.UseVisualStyleBackColor = true;
            // 
            // cbSkipCalib
            // 
            this.cbSkipCalib.AutoSize = true;
            this.cbSkipCalib.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSkipCalib.Location = new System.Drawing.Point(6, 122);
            this.cbSkipCalib.Name = "cbSkipCalib";
            this.cbSkipCalib.Size = new System.Drawing.Size(225, 25);
            this.cbSkipCalib.TabIndex = 60;
            this.cbSkipCalib.Text = "Calibration 공정 작업 무시";
            this.cbSkipCalib.UseVisualStyleBackColor = true;
            // 
            // cbSkipSkull
            // 
            this.cbSkipSkull.AutoSize = true;
            this.cbSkipSkull.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbSkipSkull.Location = new System.Drawing.Point(6, 150);
            this.cbSkipSkull.Name = "cbSkipSkull";
            this.cbSkipSkull.Size = new System.Drawing.Size(179, 25);
            this.cbSkipSkull.TabIndex = 61;
            this.cbSkipSkull.Text = "Skull 공정 작업 무시";
            this.cbSkipSkull.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.cbSkipCalib);
            this.panel4.Controls.Add(this.cbSkipMTF);
            this.panel4.Controls.Add(this.cbSkipAging);
            this.panel4.Controls.Add(this.cbSkipEntr);
            this.panel4.Controls.Add(this.cbSkipSkull);
            this.panel4.Location = new System.Drawing.Point(0, 461);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(414, 190);
            this.panel4.TabIndex = 62;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label9.Dock = System.Windows.Forms.DockStyle.Top;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(412, 35);
            this.label9.TabIndex = 3;
            this.label9.Text = "공정 SKIP OPTION";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.tbBuzzOffTime);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Location = new System.Drawing.Point(413, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(414, 138);
            this.panel5.TabIndex = 63;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(354, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 21);
            this.label1.TabIndex = 32;
            this.label1.Text = "sec";
            // 
            // tbBuzzOffTime
            // 
            this.tbBuzzOffTime.Location = new System.Drawing.Point(244, 45);
            this.tbBuzzOffTime.Name = "tbBuzzOffTime";
            this.tbBuzzOffTime.Size = new System.Drawing.Size(104, 23);
            this.tbBuzzOffTime.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 21);
            this.label3.TabIndex = 30;
            this.label3.Text = "타워 램프 알람 자동 오프 시간";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(-1, -1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(414, 35);
            this.label12.TabIndex = 0;
            this.label12.Text = "WORK OPTION";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.btRecordPath);
            this.panel6.Controls.Add(this.tbRecordPath);
            this.panel6.Controls.Add(this.label11);
            this.panel6.Controls.Add(this.cbUseRecord);
            this.panel6.Controls.Add(this.label10);
            this.panel6.Location = new System.Drawing.Point(413, 137);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(414, 170);
            this.panel6.TabIndex = 64;
            // 
            // btRecordPath
            // 
            this.btRecordPath.Location = new System.Drawing.Point(375, 107);
            this.btRecordPath.Name = "btRecordPath";
            this.btRecordPath.Size = new System.Drawing.Size(31, 23);
            this.btRecordPath.TabIndex = 31;
            this.btRecordPath.Text = "...";
            this.btRecordPath.UseVisualStyleBackColor = true;
            this.btRecordPath.Visible = false;
            this.btRecordPath.Click += new System.EventHandler(this.btRecordPath_Click);
            // 
            // tbRecordPath
            // 
            this.tbRecordPath.Location = new System.Drawing.Point(6, 107);
            this.tbRecordPath.Name = "tbRecordPath";
            this.tbRecordPath.Size = new System.Drawing.Size(368, 23);
            this.tbRecordPath.TabIndex = 27;
            this.tbRecordPath.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(2, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(156, 21);
            this.label11.TabIndex = 26;
            this.label11.Text = "녹화 파일 저장 경로";
            this.label11.Visible = false;
            // 
            // cbUseRecord
            // 
            this.cbUseRecord.AutoSize = true;
            this.cbUseRecord.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbUseRecord.Location = new System.Drawing.Point(9, 46);
            this.cbUseRecord.Name = "cbUseRecord";
            this.cbUseRecord.Size = new System.Drawing.Size(191, 25);
            this.cbUseRecord.TabIndex = 13;
            this.cbUseRecord.Text = "모니터 화면 녹화 사용";
            this.cbUseRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbUseRecord.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(412, 35);
            this.label10.TabIndex = 3;
            this.label10.Text = "RECORD OPTION";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVer);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Text = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOption_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbCrntLvNo;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbSetLvNo;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbBarcodeSkip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbIgnrCnctErr;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbXrayRptCnt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tbDressyPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbEzSensorPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbIgnrSerialErr;
        private System.Windows.Forms.CheckBox cbSkipEntr;
        private System.Windows.Forms.CheckBox cbSkipAging;
        private System.Windows.Forms.CheckBox cbSkipMTF;
        private System.Windows.Forms.CheckBox cbSkipCalib;
        private System.Windows.Forms.CheckBox cbSkipSkull;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbBuzzOffTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btRecordPath;
        private System.Windows.Forms.TextBox tbRecordPath;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cbUseRecord;
        private System.Windows.Forms.Label label10;
    }
}