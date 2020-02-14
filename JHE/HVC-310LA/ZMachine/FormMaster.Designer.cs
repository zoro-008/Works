namespace Machine
{
    partial class FormMaster
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
            this.tcMaster = new System.Windows.Forms.TabControl();
            this.Part = new System.Windows.Forms.TabPage();
            this.btSave = new System.Windows.Forms.Button();
            this.btAllCheck = new System.Windows.Forms.Button();
            this.btAllReset = new System.Windows.Forms.Button();
            this.btPartAutorun = new System.Windows.Forms.Button();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.btPartReset = new System.Windows.Forms.Button();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.Vision = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbRcvData = new System.Windows.Forms.TextBox();
            this.btMsgClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbRcvDataId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btMsgSend = new System.Windows.Forms.Button();
            this.cbCSSignal = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbMsgData = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTrgOfs = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbOneShotTime = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Option = new System.Windows.Forms.TabPage();
            this.cbUseEccntrCorr = new System.Windows.Forms.CheckBox();
            this.cbUseUnlock = new System.Windows.Forms.CheckBox();
            this.btOptnSave = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbMaxTorqueTime = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbMaxTorqueLimit = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMaxTorqueMax = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btEccentricInsp = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lvTable = new System.Windows.Forms.ListView();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tcMaster.SuspendLayout();
            this.Part.SuspendLayout();
            this.Vision.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Option.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMaster
            // 
            this.tcMaster.Controls.Add(this.Part);
            this.tcMaster.Controls.Add(this.Vision);
            this.tcMaster.Controls.Add(this.Option);
            this.tcMaster.Controls.Add(this.tabPage1);
            this.tcMaster.Controls.Add(this.tabPage2);
            this.tcMaster.Location = new System.Drawing.Point(12, 12);
            this.tcMaster.Name = "tcMaster";
            this.tcMaster.SelectedIndex = 0;
            this.tcMaster.Size = new System.Drawing.Size(612, 416);
            this.tcMaster.TabIndex = 19;
            // 
            // Part
            // 
            this.Part.Controls.Add(this.btSave);
            this.Part.Controls.Add(this.btAllCheck);
            this.Part.Controls.Add(this.btAllReset);
            this.Part.Controls.Add(this.btPartAutorun);
            this.Part.Controls.Add(this.cbDebug);
            this.Part.Controls.Add(this.btPartReset);
            this.Part.Controls.Add(this.lvSequence);
            this.Part.Location = new System.Drawing.Point(4, 22);
            this.Part.Name = "Part";
            this.Part.Padding = new System.Windows.Forms.Padding(3);
            this.Part.Size = new System.Drawing.Size(604, 390);
            this.Part.TabIndex = 0;
            this.Part.Text = "Part";
            this.Part.UseVisualStyleBackColor = true;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(425, 323);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(173, 34);
            this.btSave.TabIndex = 25;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btAllCheck
            // 
            this.btAllCheck.Location = new System.Drawing.Point(7, 175);
            this.btAllCheck.Name = "btAllCheck";
            this.btAllCheck.Size = new System.Drawing.Size(105, 23);
            this.btAllCheck.TabIndex = 24;
            this.btAllCheck.Text = "All Check";
            this.btAllCheck.UseVisualStyleBackColor = true;
            this.btAllCheck.Click += new System.EventHandler(this.btAllCheck_Click);
            // 
            // btAllReset
            // 
            this.btAllReset.Location = new System.Drawing.Point(7, 126);
            this.btAllReset.Name = "btAllReset";
            this.btAllReset.Size = new System.Drawing.Size(105, 43);
            this.btAllReset.TabIndex = 23;
            this.btAllReset.Text = "All Reset";
            this.btAllReset.UseVisualStyleBackColor = true;
            this.btAllReset.Click += new System.EventHandler(this.btAllReset_Click);
            // 
            // btPartAutorun
            // 
            this.btPartAutorun.Location = new System.Drawing.Point(7, 51);
            this.btPartAutorun.Name = "btPartAutorun";
            this.btPartAutorun.Size = new System.Drawing.Size(105, 43);
            this.btPartAutorun.TabIndex = 22;
            this.btPartAutorun.Text = "Part Autorun";
            this.btPartAutorun.UseVisualStyleBackColor = true;
            this.btPartAutorun.Click += new System.EventHandler(this.btPartAutorun_Click);
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(7, 213);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(88, 17);
            this.cbDebug.TabIndex = 21;
            this.cbDebug.Text = "Debug Mode";
            this.cbDebug.UseVisualStyleBackColor = true;
            // 
            // btPartReset
            // 
            this.btPartReset.Location = new System.Drawing.Point(7, 22);
            this.btPartReset.Name = "btPartReset";
            this.btPartReset.Size = new System.Drawing.Size(105, 23);
            this.btPartReset.TabIndex = 20;
            this.btPartReset.Text = "Part Reset";
            this.btPartReset.UseVisualStyleBackColor = true;
            this.btPartReset.Click += new System.EventHandler(this.btPartReset_Click);
            // 
            // lvSequence
            // 
            this.lvSequence.Location = new System.Drawing.Point(133, 22);
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.Size = new System.Drawing.Size(465, 283);
            this.lvSequence.TabIndex = 19;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
            // 
            // Vision
            // 
            this.Vision.Controls.Add(this.groupBox3);
            this.Vision.Controls.Add(this.groupBox2);
            this.Vision.Controls.Add(this.groupBox1);
            this.Vision.Location = new System.Drawing.Point(4, 22);
            this.Vision.Name = "Vision";
            this.Vision.Padding = new System.Windows.Forms.Padding(3);
            this.Vision.Size = new System.Drawing.Size(604, 390);
            this.Vision.TabIndex = 1;
            this.Vision.Text = "Vision";
            this.Vision.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.tbRcvData);
            this.groupBox3.Controls.Add(this.btMsgClear);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tbRcvDataId);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btMsgSend);
            this.groupBox3.Controls.Add(this.cbCSSignal);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.tbMsgData);
            this.groupBox3.Location = new System.Drawing.Point(20, 145);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(332, 178);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Message Communication";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 45;
            this.label7.Text = "Receive Data";
            // 
            // tbRcvData
            // 
            this.tbRcvData.Location = new System.Drawing.Point(86, 105);
            this.tbRcvData.Name = "tbRcvData";
            this.tbRcvData.Size = new System.Drawing.Size(225, 20);
            this.tbRcvData.TabIndex = 44;
            // 
            // btMsgClear
            // 
            this.btMsgClear.Location = new System.Drawing.Point(11, 130);
            this.btMsgClear.Name = "btMsgClear";
            this.btMsgClear.Size = new System.Drawing.Size(121, 33);
            this.btMsgClear.TabIndex = 43;
            this.btMsgClear.Text = "Clear";
            this.btMsgClear.UseVisualStyleBackColor = true;
            this.btMsgClear.Click += new System.EventHandler(this.button4_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Receive Id";
            // 
            // tbRcvDataId
            // 
            this.tbRcvDataId.Location = new System.Drawing.Point(86, 80);
            this.tbRcvDataId.Name = "tbRcvDataId";
            this.tbRcvDataId.Size = new System.Drawing.Size(225, 20);
            this.tbRcvDataId.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Communication signal";
            // 
            // btMsgSend
            // 
            this.btMsgSend.Location = new System.Drawing.Point(138, 130);
            this.btMsgSend.Name = "btMsgSend";
            this.btMsgSend.Size = new System.Drawing.Size(178, 33);
            this.btMsgSend.TabIndex = 36;
            this.btMsgSend.Text = "Send";
            this.btMsgSend.UseVisualStyleBackColor = true;
            this.btMsgSend.Click += new System.EventHandler(this.btMsgSend_Click);
            // 
            // cbCSSignal
            // 
            this.cbCSSignal.FormattingEnabled = true;
            this.cbCSSignal.Items.AddRange(new object[] {
            "Busy  ",
            "Ready ",
            "Reset ",
            "Change",
            "Inspection",
            "END"});
            this.cbCSSignal.Location = new System.Drawing.Point(145, 20);
            this.cbCSSignal.Name = "cbCSSignal";
            this.cbCSSignal.Size = new System.Drawing.Size(166, 21);
            this.cbCSSignal.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "Msg data";
            // 
            // tbMsgData
            // 
            this.tbMsgData.Location = new System.Drawing.Point(86, 53);
            this.tbMsgData.Name = "tbMsgData";
            this.tbMsgData.Size = new System.Drawing.Size(225, 20);
            this.tbMsgData.TabIndex = 34;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbTrgOfs);
            this.groupBox2.Location = new System.Drawing.Point(20, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 109);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Triger Offset";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "ms";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 33);
            this.button2.TabIndex = 36;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Triger Ofs";
            // 
            // tbTrgOfs
            // 
            this.tbTrgOfs.Location = new System.Drawing.Point(76, 27);
            this.tbTrgOfs.Name = "tbTrgOfs";
            this.tbTrgOfs.Size = new System.Drawing.Size(79, 20);
            this.tbTrgOfs.TabIndex = 34;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbOneShotTime);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(235, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 109);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "One Shot Triger";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Shot Time";
            // 
            // tbOneShotTime
            // 
            this.tbOneShotTime.Location = new System.Drawing.Point(76, 27);
            this.tbOneShotTime.Name = "tbOneShotTime";
            this.tbOneShotTime.Size = new System.Drawing.Size(102, 20);
            this.tbOneShotTime.TabIndex = 34;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(168, 33);
            this.button1.TabIndex = 33;
            this.button1.Text = "Shot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Option
            // 
            this.Option.Controls.Add(this.cbUseEccntrCorr);
            this.Option.Controls.Add(this.cbUseUnlock);
            this.Option.Controls.Add(this.btOptnSave);
            this.Option.Controls.Add(this.label12);
            this.Option.Controls.Add(this.label13);
            this.Option.Controls.Add(this.tbMaxTorqueTime);
            this.Option.Controls.Add(this.label11);
            this.Option.Controls.Add(this.tbMaxTorqueLimit);
            this.Option.Controls.Add(this.label8);
            this.Option.Controls.Add(this.tbMaxTorqueMax);
            this.Option.Location = new System.Drawing.Point(4, 22);
            this.Option.Name = "Option";
            this.Option.Size = new System.Drawing.Size(604, 390);
            this.Option.TabIndex = 2;
            this.Option.Text = "Option";
            this.Option.UseVisualStyleBackColor = true;
            // 
            // cbUseEccntrCorr
            // 
            this.cbUseEccntrCorr.AutoSize = true;
            this.cbUseEccntrCorr.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUseEccntrCorr.Location = new System.Drawing.Point(18, 173);
            this.cbUseEccntrCorr.Name = "cbUseEccntrCorr";
            this.cbUseEccntrCorr.Size = new System.Drawing.Size(234, 20);
            this.cbUseEccntrCorr.TabIndex = 30;
            this.cbUseEccntrCorr.Text = "Use Eccentric Correction";
            this.cbUseEccntrCorr.UseVisualStyleBackColor = true;
            // 
            // cbUseUnlock
            // 
            this.cbUseUnlock.AutoSize = true;
            this.cbUseUnlock.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUseUnlock.Location = new System.Drawing.Point(18, 138);
            this.cbUseUnlock.Name = "cbUseUnlock";
            this.cbUseUnlock.Size = new System.Drawing.Size(172, 20);
            this.cbUseUnlock.TabIndex = 27;
            this.cbUseUnlock.Text = "Use Unlock Mode";
            this.cbUseUnlock.UseVisualStyleBackColor = true;
            this.cbUseUnlock.Visible = false;
            // 
            // btOptnSave
            // 
            this.btOptnSave.Location = new System.Drawing.Point(425, 323);
            this.btOptnSave.Name = "btOptnSave";
            this.btOptnSave.Size = new System.Drawing.Size(173, 34);
            this.btOptnSave.TabIndex = 26;
            this.btOptnSave.Text = "Save";
            this.btOptnSave.UseVisualStyleBackColor = true;
            this.btOptnSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(330, 104);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 16);
            this.label12.TabIndex = 8;
            this.label12.Text = "ms";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(15, 104);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(191, 16);
            this.label13.TabIndex = 7;
            this.label13.Text = "Maximum Torque Time";
            // 
            // tbMaxTorqueTime
            // 
            this.tbMaxTorqueTime.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbMaxTorqueTime.Location = new System.Drawing.Point(212, 101);
            this.tbMaxTorqueTime.Name = "tbMaxTorqueTime";
            this.tbMaxTorqueTime.Size = new System.Drawing.Size(100, 26);
            this.tbMaxTorqueTime.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(15, 66);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(191, 16);
            this.label11.TabIndex = 4;
            this.label11.Text = "Maximum Torque Limit";
            // 
            // tbMaxTorqueLimit
            // 
            this.tbMaxTorqueLimit.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbMaxTorqueLimit.Location = new System.Drawing.Point(212, 63);
            this.tbMaxTorqueLimit.Name = "tbMaxTorqueLimit";
            this.tbMaxTorqueLimit.Size = new System.Drawing.Size(100, 26);
            this.tbMaxTorqueLimit.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(15, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "Maximum Torque Max";
            // 
            // tbMaxTorqueMax
            // 
            this.tbMaxTorqueMax.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbMaxTorqueMax.Location = new System.Drawing.Point(212, 27);
            this.tbMaxTorqueMax.Name = "tbMaxTorqueMax";
            this.tbMaxTorqueMax.Size = new System.Drawing.Size(100, 26);
            this.tbMaxTorqueMax.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(604, 390);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "TorqueMotr";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(14, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "Inposition";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(211, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 26);
            this.textBox1.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(14, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Alarm";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox2.Location = new System.Drawing.Point(211, 10);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 26);
            this.textBox2.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btEccentricInsp);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.lvTable);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(604, 390);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "LookUpTable";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btEccentricInsp
            // 
            this.btEccentricInsp.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEccentricInsp.Location = new System.Drawing.Point(192, 351);
            this.btEccentricInsp.Name = "btEccentricInsp";
            this.btEccentricInsp.Size = new System.Drawing.Size(222, 34);
            this.btEccentricInsp.TabIndex = 36;
            this.btEccentricInsp.Tag = "16";
            this.btEccentricInsp.Text = "Start Eccentric Insp.";
            this.btEccentricInsp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEccentricInsp.UseVisualStyleBackColor = true;
            this.btEccentricInsp.Click += new System.EventHandler(this.btEccentricInsp_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(425, 351);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(173, 34);
            this.button3.TabIndex = 27;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btSave_Click);
            // 
            // lvTable
            // 
            this.lvTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvTable.Location = new System.Drawing.Point(0, 0);
            this.lvTable.Name = "lvTable";
            this.lvTable.Size = new System.Drawing.Size(604, 287);
            this.lvTable.TabIndex = 0;
            this.lvTable.UseCompatibleStateImageBehavior = false;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(636, 440);
            this.Controls.Add(this.tcMaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormMaster";
            this.Text = "FormMaster";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMaster_FormClosing);
            this.Shown += new System.EventHandler(this.FormMaster_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormMaster_VisibleChanged);
            this.tcMaster.ResumeLayout(false);
            this.Part.ResumeLayout(false);
            this.Part.PerformLayout();
            this.Vision.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Option.ResumeLayout(false);
            this.Option.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcMaster;
        private System.Windows.Forms.TabPage Part;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btAllCheck;
        private System.Windows.Forms.Button btAllReset;
        private System.Windows.Forms.Button btPartAutorun;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.Button btPartReset;
        private System.Windows.Forms.ListView lvSequence;
        private System.Windows.Forms.TabPage Vision;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbOneShotTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTrgOfs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCSSignal;
        private System.Windows.Forms.Button btMsgSend;
        private System.Windows.Forms.Button btMsgClear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbRcvDataId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMsgData;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbRcvData;
        private System.Windows.Forms.TabPage Option;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbMaxTorqueTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbMaxTorqueLimit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMaxTorqueMax;
        private System.Windows.Forms.Button btOptnSave;
        private System.Windows.Forms.CheckBox cbUseUnlock;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListView lvTable;
        private System.Windows.Forms.CheckBox cbUseEccntrCorr;
        private System.Windows.Forms.Button btEccentricInsp;
    }
}