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
            this.label3 = new System.Windows.Forms.Label();
            this.tbVisnBfDelay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSkipRear = new System.Windows.Forms.CheckBox();
            this.cbSkipFrnt = new System.Windows.Forms.CheckBox();
            this.cbUnlock = new System.Windows.Forms.CheckBox();
            this.lbVer = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.cbUseMultiHldr = new System.Windows.Forms.CheckBox();
            this.cbIgnrLeftPck = new System.Windows.Forms.CheckBox();
            this.cbIgnrRightPck = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbUseAtInsp = new System.Windows.Forms.CheckBox();
            this.cb1PntRpt = new System.Windows.Forms.CheckBox();
            this.cbTorqChck = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.tbGaugeTorq2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbGaugeTorq1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbSetMotrTorq2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSetMotrTorq1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel6 = new System.Windows.Forms.Panel();
            this.cbPlaceRetryCnt = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbPlaceVisnRetryCnt = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cbPickRetryCnt = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cbPickVisnRetryCnt = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbVisnBfDelay);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 103);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(347, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "ms";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbVisnBfDelay
            // 
            this.tbVisnBfDelay.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbVisnBfDelay.Location = new System.Drawing.Point(203, 52);
            this.tbVisnBfDelay.Name = "tbVisnBfDelay";
            this.tbVisnBfDelay.Size = new System.Drawing.Size(138, 26);
            this.tbVisnBfDelay.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(9, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Vision Before Delay";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(412, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "DELAY OPTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbSkipRear
            // 
            this.cbSkipRear.AutoSize = true;
            this.cbSkipRear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSkipRear.Location = new System.Drawing.Point(12, 82);
            this.cbSkipRear.Name = "cbSkipRear";
            this.cbSkipRear.Size = new System.Drawing.Size(183, 24);
            this.cbSkipRear.TabIndex = 6;
            this.cbSkipRear.Text = "Skip Rear Holder Tray";
            this.cbSkipRear.UseVisualStyleBackColor = true;
            // 
            // cbSkipFrnt
            // 
            this.cbSkipFrnt.AutoSize = true;
            this.cbSkipFrnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSkipFrnt.Location = new System.Drawing.Point(12, 52);
            this.cbSkipFrnt.Name = "cbSkipFrnt";
            this.cbSkipFrnt.Size = new System.Drawing.Size(186, 24);
            this.cbSkipFrnt.TabIndex = 5;
            this.cbSkipFrnt.Text = "Skip Front Holder Tray";
            this.cbSkipFrnt.UseVisualStyleBackColor = true;
            // 
            // cbUnlock
            // 
            this.cbUnlock.AutoSize = true;
            this.cbUnlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUnlock.Location = new System.Drawing.Point(263, 118);
            this.cbUnlock.Name = "cbUnlock";
            this.cbUnlock.Size = new System.Drawing.Size(127, 24);
            this.cbUnlock.TabIndex = 4;
            this.cbUnlock.Text = "UnLock Mode";
            this.cbUnlock.UseVisualStyleBackColor = true;
            this.cbUnlock.Visible = false;
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
            // cbUseMultiHldr
            // 
            this.cbUseMultiHldr.AutoSize = true;
            this.cbUseMultiHldr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUseMultiHldr.Location = new System.Drawing.Point(12, 79);
            this.cbUseMultiHldr.Name = "cbUseMultiHldr";
            this.cbUseMultiHldr.Size = new System.Drawing.Size(194, 24);
            this.cbUseMultiHldr.TabIndex = 7;
            this.cbUseMultiHldr.Text = "Using Multi Picker Work";
            this.cbUseMultiHldr.UseVisualStyleBackColor = true;
            // 
            // cbIgnrLeftPck
            // 
            this.cbIgnrLeftPck.AutoSize = true;
            this.cbIgnrLeftPck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIgnrLeftPck.Location = new System.Drawing.Point(12, 47);
            this.cbIgnrLeftPck.Name = "cbIgnrLeftPck";
            this.cbIgnrLeftPck.Size = new System.Drawing.Size(177, 24);
            this.cbIgnrLeftPck.TabIndex = 8;
            this.cbIgnrLeftPck.Text = "Not Using Left Picker";
            this.cbIgnrLeftPck.UseVisualStyleBackColor = true;
            // 
            // cbIgnrRightPck
            // 
            this.cbIgnrRightPck.AutoSize = true;
            this.cbIgnrRightPck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIgnrRightPck.Location = new System.Drawing.Point(12, 77);
            this.cbIgnrRightPck.Name = "cbIgnrRightPck";
            this.cbIgnrRightPck.Size = new System.Drawing.Size(187, 24);
            this.cbIgnrRightPck.TabIndex = 9;
            this.cbIgnrRightPck.Text = "Not Using Right Picker";
            this.cbIgnrRightPck.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cbUseAtInsp);
            this.panel2.Controls.Add(this.cb1PntRpt);
            this.panel2.Controls.Add(this.cbTorqChck);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.cbUnlock);
            this.panel2.Controls.Add(this.cbUseMultiHldr);
            this.panel2.Location = new System.Drawing.Point(1, 103);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 164);
            this.panel2.TabIndex = 4;
            // 
            // cbUseAtInsp
            // 
            this.cbUseAtInsp.AutoSize = true;
            this.cbUseAtInsp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUseAtInsp.Location = new System.Drawing.Point(12, 106);
            this.cbUseAtInsp.Name = "cbUseAtInsp";
            this.cbUseAtInsp.Size = new System.Drawing.Size(217, 24);
            this.cbUseAtInsp.TabIndex = 12;
            this.cbUseAtInsp.Text = "Use Last Vision Inspection";
            this.cbUseAtInsp.UseVisualStyleBackColor = true;
            // 
            // cb1PntRpt
            // 
            this.cb1PntRpt.AutoSize = true;
            this.cb1PntRpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb1PntRpt.Location = new System.Drawing.Point(215, 131);
            this.cb1PntRpt.Name = "cb1PntRpt";
            this.cb1PntRpt.Size = new System.Drawing.Size(184, 24);
            this.cb1PntRpt.TabIndex = 9;
            this.cb1PntRpt.Text = "Fail One Point Repeat";
            this.cb1PntRpt.UseVisualStyleBackColor = true;
            this.cb1PntRpt.Visible = false;
            // 
            // cbTorqChck
            // 
            this.cbTorqChck.AutoSize = true;
            this.cbTorqChck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTorqChck.Location = new System.Drawing.Point(12, 135);
            this.cbTorqChck.Name = "cbTorqChck";
            this.cbTorqChck.Size = new System.Drawing.Size(171, 24);
            this.cbTorqChck.TabIndex = 8;
            this.cbTorqChck.Text = "Torque Check Mode";
            this.cbTorqChck.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(412, 35);
            this.label6.TabIndex = 0;
            this.label6.Text = "WORK MODE";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.cbIgnrRightPck);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.cbIgnrLeftPck);
            this.panel3.Location = new System.Drawing.Point(1, 266);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(414, 119);
            this.panel3.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(412, 35);
            this.label4.TabIndex = 0;
            this.label4.Text = "PICKER SELECT OPTION";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.cbSkipRear);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.cbSkipFrnt);
            this.panel4.Location = new System.Drawing.Point(1, 384);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(414, 119);
            this.panel4.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(412, 35);
            this.label5.TabIndex = 0;
            this.label5.Text = "TRAY SKIP OPTION";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.tbGaugeTorq2);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.tbGaugeTorq1);
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.tbSetMotrTorq2);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.tbSetMotrTorq1);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Location = new System.Drawing.Point(1, 502);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(414, 167);
            this.panel5.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(9, 132);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(158, 20);
            this.label10.TabIndex = 8;
            this.label10.Text = "Gauge Torque Para2";
            // 
            // tbGaugeTorq2
            // 
            this.tbGaugeTorq2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbGaugeTorq2.Location = new System.Drawing.Point(215, 129);
            this.tbGaugeTorq2.Name = "tbGaugeTorq2";
            this.tbGaugeTorq2.Size = new System.Drawing.Size(100, 26);
            this.tbGaugeTorq2.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(9, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(158, 20);
            this.label11.TabIndex = 6;
            this.label11.Text = "Gauge Torque Para1";
            // 
            // tbGaugeTorq1
            // 
            this.tbGaugeTorq1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbGaugeTorq1.Location = new System.Drawing.Point(215, 71);
            this.tbGaugeTorq1.Name = "tbGaugeTorq1";
            this.tbGaugeTorq1.Size = new System.Drawing.Size(100, 26);
            this.tbGaugeTorq1.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(10, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(179, 20);
            this.label9.TabIndex = 4;
            this.label9.Text = "Set Motor Torque Para2";
            // 
            // tbSetMotrTorq2
            // 
            this.tbSetMotrTorq2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbSetMotrTorq2.Location = new System.Drawing.Point(215, 100);
            this.tbSetMotrTorq2.Name = "tbSetMotrTorq2";
            this.tbSetMotrTorq2.Size = new System.Drawing.Size(100, 26);
            this.tbSetMotrTorq2.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(10, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(179, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "Set Motor Torque Para1";
            // 
            // tbSetMotrTorq1
            // 
            this.tbSetMotrTorq1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbSetMotrTorq1.Location = new System.Drawing.Point(215, 42);
            this.tbSetMotrTorq1.Name = "tbSetMotrTorq1";
            this.tbSetMotrTorq1.Size = new System.Drawing.Size(100, 26);
            this.tbSetMotrTorq1.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(412, 35);
            this.label7.TabIndex = 0;
            this.label7.Text = "TORQUE CONVERT OPTION";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.cbPlaceRetryCnt);
            this.panel6.Controls.Add(this.label17);
            this.panel6.Controls.Add(this.cbPlaceVisnRetryCnt);
            this.panel6.Controls.Add(this.label16);
            this.panel6.Controls.Add(this.cbPickRetryCnt);
            this.panel6.Controls.Add(this.label15);
            this.panel6.Controls.Add(this.cbPickVisnRetryCnt);
            this.panel6.Controls.Add(this.label13);
            this.panel6.Controls.Add(this.label14);
            this.panel6.Location = new System.Drawing.Point(414, 103);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(414, 164);
            this.panel6.TabIndex = 8;
            // 
            // cbPlaceRetryCnt
            // 
            this.cbPlaceRetryCnt.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbPlaceRetryCnt.FormattingEnabled = true;
            this.cbPlaceRetryCnt.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbPlaceRetryCnt.Location = new System.Drawing.Point(278, 120);
            this.cbPlaceRetryCnt.Name = "cbPlaceRetryCnt";
            this.cbPlaceRetryCnt.Size = new System.Drawing.Size(92, 24);
            this.cbPlaceRetryCnt.TabIndex = 17;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label17.Location = new System.Drawing.Point(10, 124);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(137, 20);
            this.label17.TabIndex = 16;
            this.label17.Text = "Place Retry Count";
            // 
            // cbPlaceVisnRetryCnt
            // 
            this.cbPlaceVisnRetryCnt.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbPlaceVisnRetryCnt.FormattingEnabled = true;
            this.cbPlaceVisnRetryCnt.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbPlaceVisnRetryCnt.Location = new System.Drawing.Point(278, 95);
            this.cbPlaceVisnRetryCnt.Name = "cbPlaceVisnRetryCnt";
            this.cbPlaceVisnRetryCnt.Size = new System.Drawing.Size(92, 24);
            this.cbPlaceVisnRetryCnt.TabIndex = 15;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label16.Location = new System.Drawing.Point(10, 99);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(204, 20);
            this.label16.TabIndex = 14;
            this.label16.Text = "Vison for Place Retry Count";
            // 
            // cbPickRetryCnt
            // 
            this.cbPickRetryCnt.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbPickRetryCnt.FormattingEnabled = true;
            this.cbPickRetryCnt.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbPickRetryCnt.Location = new System.Drawing.Point(278, 70);
            this.cbPickRetryCnt.Name = "cbPickRetryCnt";
            this.cbPickRetryCnt.Size = new System.Drawing.Size(92, 24);
            this.cbPickRetryCnt.TabIndex = 13;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label15.Location = new System.Drawing.Point(9, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(127, 20);
            this.label15.TabIndex = 12;
            this.label15.Text = "Pick Retry Count";
            // 
            // cbPickVisnRetryCnt
            // 
            this.cbPickVisnRetryCnt.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbPickVisnRetryCnt.FormattingEnabled = true;
            this.cbPickVisnRetryCnt.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbPickVisnRetryCnt.Location = new System.Drawing.Point(278, 45);
            this.cbPickVisnRetryCnt.Name = "cbPickVisnRetryCnt";
            this.cbPickVisnRetryCnt.Size = new System.Drawing.Size(92, 24);
            this.cbPickVisnRetryCnt.TabIndex = 11;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(10, 49);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(194, 20);
            this.label13.TabIndex = 10;
            this.label13.Text = "Vison for Pick Retry Count";
            // 
            // label14
            // 
            this.label14.Dock = System.Windows.Forms.DockStyle.Top;
            this.label14.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label14.Image = ((System.Drawing.Image)(resources.GetObject("label14.Image")));
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(412, 35);
            this.label14.TabIndex = 0;
            this.label14.Text = "RETRY OPTION";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVer);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Text = "FormOption";
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

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbVisnBfDelay;
        private System.Windows.Forms.CheckBox cbUnlock;
        private System.Windows.Forms.CheckBox cbSkipRear;
        private System.Windows.Forms.CheckBox cbSkipFrnt;
        private System.Windows.Forms.CheckBox cbIgnrRightPck;
        private System.Windows.Forms.CheckBox cbIgnrLeftPck;
        private System.Windows.Forms.CheckBox cbUseMultiHldr;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbGaugeTorq2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbGaugeTorq1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbSetMotrTorq2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSetMotrTorq1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbTorqChck;
        private System.Windows.Forms.CheckBox cb1PntRpt;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.CheckBox cbUseAtInsp;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ComboBox cbPlaceRetryCnt;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cbPlaceVisnRetryCnt;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cbPickRetryCnt;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cbPickVisnRetryCnt;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
    }
}