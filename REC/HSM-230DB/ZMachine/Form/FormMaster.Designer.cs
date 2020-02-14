﻿namespace Machine
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
            this.tbImage = new System.Windows.Forms.TabControl();
            this.Part = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btForcedHomeEnd = new System.Windows.Forms.Button();
            this.cbIdlerun = new System.Windows.Forms.CheckBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btAllCheck = new System.Windows.Forms.Button();
            this.btAllReset = new System.Windows.Forms.Button();
            this.btPartAutorun = new System.Windows.Forms.Button();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.btPartReset = new System.Windows.Forms.Button();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.Vision = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btShowWeight = new System.Windows.Forms.Button();
            this.btSetZero = new System.Windows.Forms.Button();
            this.btSetHoldOff = new System.Windows.Forms.Button();
            this.btSetHoldOn = new System.Windows.Forms.Button();
            this.btWeightCheck = new System.Windows.Forms.Button();
            this.SigmaCM2 = new System.Windows.Forms.TabPage();
            this.tbCh = new System.Windows.Forms.TextBox();
            this.btSetLoadCh = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btGetHeight = new System.Windows.Forms.Button();
            this.btReZero = new System.Windows.Forms.Button();
            this.btCheckHeight = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cbBarcodeCh = new System.Windows.Forms.ComboBox();
            this.btShowMessage = new System.Windows.Forms.Button();
            this.btScanOff = new System.Windows.Forms.Button();
            this.btScanOn = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.tbTemp = new System.Windows.Forms.TextBox();
            this.btSetTemp = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tbImage.SuspendLayout();
            this.Part.SuspendLayout();
            this.Vision.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SigmaCM2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbImage
            // 
            this.tbImage.Controls.Add(this.Part);
            this.tbImage.Controls.Add(this.Vision);
            this.tbImage.Controls.Add(this.tabPage1);
            this.tbImage.Controls.Add(this.SigmaCM2);
            this.tbImage.Controls.Add(this.tabPage2);
            this.tbImage.Controls.Add(this.tabPage3);
            this.tbImage.Controls.Add(this.tabPage4);
            this.tbImage.Location = new System.Drawing.Point(12, 12);
            this.tbImage.Name = "tbImage";
            this.tbImage.SelectedIndex = 0;
            this.tbImage.Size = new System.Drawing.Size(612, 416);
            this.tbImage.TabIndex = 19;
            // 
            // Part
            // 
            this.Part.Controls.Add(this.button5);
            this.Part.Controls.Add(this.textBox1);
            this.Part.Controls.Add(this.btForcedHomeEnd);
            this.Part.Controls.Add(this.cbIdlerun);
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
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(253, 316);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(87, 33);
            this.button5.TabIndex = 29;
            this.button5.Text = "SSTG STEP Zero";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(133, 323);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 28;
            // 
            // btForcedHomeEnd
            // 
            this.btForcedHomeEnd.Location = new System.Drawing.Point(7, 323);
            this.btForcedHomeEnd.Name = "btForcedHomeEnd";
            this.btForcedHomeEnd.Size = new System.Drawing.Size(105, 61);
            this.btForcedHomeEnd.TabIndex = 27;
            this.btForcedHomeEnd.Text = "Forced HomeEnd";
            this.btForcedHomeEnd.UseVisualStyleBackColor = true;
            this.btForcedHomeEnd.Click += new System.EventHandler(this.btForcedHomeEnd_Click);
            // 
            // cbIdlerun
            // 
            this.cbIdlerun.AutoSize = true;
            this.cbIdlerun.Location = new System.Drawing.Point(7, 235);
            this.cbIdlerun.Name = "cbIdlerun";
            this.cbIdlerun.Size = new System.Drawing.Size(70, 16);
            this.cbIdlerun.TabIndex = 26;
            this.cbIdlerun.Text = "Idle Run";
            this.cbIdlerun.UseVisualStyleBackColor = true;
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
            this.cbDebug.Size = new System.Drawing.Size(96, 16);
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
            this.Vision.Controls.Add(this.button4);
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
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(403, 181);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(178, 33);
            this.button4.TabIndex = 37;
            this.button4.Text = "Send";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
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
            this.label7.Size = new System.Drawing.Size(79, 12);
            this.label7.TabIndex = 45;
            this.label7.Text = "Receive Data";
            // 
            // tbRcvData
            // 
            this.tbRcvData.Location = new System.Drawing.Point(86, 105);
            this.tbRcvData.Name = "tbRcvData";
            this.tbRcvData.Size = new System.Drawing.Size(225, 21);
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
            this.label6.Size = new System.Drawing.Size(64, 12);
            this.label6.TabIndex = 41;
            this.label6.Text = "Receive Id";
            // 
            // tbRcvDataId
            // 
            this.tbRcvDataId.Location = new System.Drawing.Point(86, 80);
            this.tbRcvDataId.Name = "tbRcvDataId";
            this.tbRcvDataId.Size = new System.Drawing.Size(225, 21);
            this.tbRcvDataId.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 12);
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
            this.cbCSSignal.Size = new System.Drawing.Size(166, 20);
            this.cbCSSignal.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 35;
            this.label5.Text = "Msg data";
            // 
            // tbMsgData
            // 
            this.tbMsgData.Location = new System.Drawing.Point(86, 53);
            this.tbMsgData.Name = "tbMsgData";
            this.tbMsgData.Size = new System.Drawing.Size(225, 21);
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
            this.label4.Size = new System.Drawing.Size(23, 12);
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
            this.label3.Size = new System.Drawing.Size(61, 12);
            this.label3.TabIndex = 35;
            this.label3.Text = "Triger Ofs";
            // 
            // tbTrgOfs
            // 
            this.tbTrgOfs.Location = new System.Drawing.Point(76, 27);
            this.tbTrgOfs.Name = "tbTrgOfs";
            this.tbTrgOfs.Size = new System.Drawing.Size(79, 21);
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
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 35;
            this.label2.Text = "Shot Time";
            // 
            // tbOneShotTime
            // 
            this.tbOneShotTime.Location = new System.Drawing.Point(76, 27);
            this.tbOneShotTime.Name = "tbOneShotTime";
            this.tbOneShotTime.Size = new System.Drawing.Size(102, 21);
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btShowWeight);
            this.tabPage1.Controls.Add(this.btSetZero);
            this.tabPage1.Controls.Add(this.btSetHoldOff);
            this.tabPage1.Controls.Add(this.btSetHoldOn);
            this.tabPage1.Controls.Add(this.btWeightCheck);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(604, 390);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "KBS205";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btShowWeight
            // 
            this.btShowWeight.Location = new System.Drawing.Point(126, 6);
            this.btShowWeight.Name = "btShowWeight";
            this.btShowWeight.Size = new System.Drawing.Size(114, 23);
            this.btShowWeight.TabIndex = 4;
            this.btShowWeight.Text = "Show Weight";
            this.btShowWeight.UseVisualStyleBackColor = true;
            this.btShowWeight.Click += new System.EventHandler(this.btShowWeight_Click);
            // 
            // btSetZero
            // 
            this.btSetZero.Location = new System.Drawing.Point(6, 64);
            this.btSetZero.Name = "btSetZero";
            this.btSetZero.Size = new System.Drawing.Size(114, 23);
            this.btSetZero.TabIndex = 3;
            this.btSetZero.Text = "Set Zero";
            this.btSetZero.UseVisualStyleBackColor = true;
            this.btSetZero.Click += new System.EventHandler(this.btSetZero_Click);
            // 
            // btSetHoldOff
            // 
            this.btSetHoldOff.Location = new System.Drawing.Point(126, 35);
            this.btSetHoldOff.Name = "btSetHoldOff";
            this.btSetHoldOff.Size = new System.Drawing.Size(114, 23);
            this.btSetHoldOff.TabIndex = 2;
            this.btSetHoldOff.Text = "Set Hold Off";
            this.btSetHoldOff.UseVisualStyleBackColor = true;
            this.btSetHoldOff.Click += new System.EventHandler(this.btSetHoldOff_Click);
            // 
            // btSetHoldOn
            // 
            this.btSetHoldOn.Location = new System.Drawing.Point(6, 35);
            this.btSetHoldOn.Name = "btSetHoldOn";
            this.btSetHoldOn.Size = new System.Drawing.Size(114, 23);
            this.btSetHoldOn.TabIndex = 1;
            this.btSetHoldOn.Text = "Set Hold On";
            this.btSetHoldOn.UseVisualStyleBackColor = true;
            this.btSetHoldOn.Click += new System.EventHandler(this.btSetHoldOn_Click);
            // 
            // btWeightCheck
            // 
            this.btWeightCheck.Location = new System.Drawing.Point(6, 6);
            this.btWeightCheck.Name = "btWeightCheck";
            this.btWeightCheck.Size = new System.Drawing.Size(114, 23);
            this.btWeightCheck.TabIndex = 0;
            this.btWeightCheck.Text = "Weight Check";
            this.btWeightCheck.UseVisualStyleBackColor = true;
            this.btWeightCheck.Click += new System.EventHandler(this.btWeightCheck_Click);
            // 
            // SigmaCM2
            // 
            this.SigmaCM2.Controls.Add(this.tbCh);
            this.SigmaCM2.Controls.Add(this.btSetLoadCh);
            this.SigmaCM2.Location = new System.Drawing.Point(4, 22);
            this.SigmaCM2.Name = "SigmaCM2";
            this.SigmaCM2.Padding = new System.Windows.Forms.Padding(3);
            this.SigmaCM2.Size = new System.Drawing.Size(604, 390);
            this.SigmaCM2.TabIndex = 4;
            this.SigmaCM2.Text = "SigmaCM2";
            this.SigmaCM2.UseVisualStyleBackColor = true;
            // 
            // tbCh
            // 
            this.tbCh.Location = new System.Drawing.Point(6, 8);
            this.tbCh.Name = "tbCh";
            this.tbCh.Size = new System.Drawing.Size(100, 21);
            this.tbCh.TabIndex = 1;
            this.tbCh.Text = "1";
            // 
            // btSetLoadCh
            // 
            this.btSetLoadCh.Location = new System.Drawing.Point(112, 6);
            this.btSetLoadCh.Name = "btSetLoadCh";
            this.btSetLoadCh.Size = new System.Drawing.Size(75, 23);
            this.btSetLoadCh.TabIndex = 0;
            this.btSetLoadCh.Text = "Set Load Ch";
            this.btSetLoadCh.UseVisualStyleBackColor = true;
            this.btSetLoadCh.Click += new System.EventHandler(this.btSetLoadCh_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btGetHeight);
            this.tabPage2.Controls.Add(this.btReZero);
            this.tabPage2.Controls.Add(this.btCheckHeight);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(604, 390);
            this.tabPage2.TabIndex = 5;
            this.tabPage2.Text = "LKG5001";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btGetHeight
            // 
            this.btGetHeight.Location = new System.Drawing.Point(238, 30);
            this.btGetHeight.Name = "btGetHeight";
            this.btGetHeight.Size = new System.Drawing.Size(154, 23);
            this.btGetHeight.TabIndex = 2;
            this.btGetHeight.Text = "Get Height";
            this.btGetHeight.UseVisualStyleBackColor = true;
            this.btGetHeight.Click += new System.EventHandler(this.btGetHeight_Click);
            // 
            // btReZero
            // 
            this.btReZero.Location = new System.Drawing.Point(78, 59);
            this.btReZero.Name = "btReZero";
            this.btReZero.Size = new System.Drawing.Size(154, 23);
            this.btReZero.TabIndex = 1;
            this.btReZero.Text = "ReZero";
            this.btReZero.UseVisualStyleBackColor = true;
            this.btReZero.Click += new System.EventHandler(this.btReZero_Click);
            // 
            // btCheckHeight
            // 
            this.btCheckHeight.Location = new System.Drawing.Point(78, 30);
            this.btCheckHeight.Name = "btCheckHeight";
            this.btCheckHeight.Size = new System.Drawing.Size(154, 23);
            this.btCheckHeight.TabIndex = 0;
            this.btCheckHeight.Text = "Send Check Height";
            this.btCheckHeight.UseVisualStyleBackColor = true;
            this.btCheckHeight.Click += new System.EventHandler(this.btCheckHeight_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbBarcodeCh);
            this.tabPage3.Controls.Add(this.btShowMessage);
            this.tabPage3.Controls.Add(this.btScanOff);
            this.tabPage3.Controls.Add(this.btScanOn);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(604, 390);
            this.tabPage3.TabIndex = 6;
            this.tabPage3.Text = "3310g";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbBarcodeCh
            // 
            this.cbBarcodeCh.FormattingEnabled = true;
            this.cbBarcodeCh.Items.AddRange(new object[] {
            "Wafer",
            "SubStrate"});
            this.cbBarcodeCh.Location = new System.Drawing.Point(189, 25);
            this.cbBarcodeCh.Name = "cbBarcodeCh";
            this.cbBarcodeCh.Size = new System.Drawing.Size(121, 20);
            this.cbBarcodeCh.TabIndex = 3;
            // 
            // btShowMessage
            // 
            this.btShowMessage.Location = new System.Drawing.Point(379, 93);
            this.btShowMessage.Name = "btShowMessage";
            this.btShowMessage.Size = new System.Drawing.Size(108, 28);
            this.btShowMessage.TabIndex = 2;
            this.btShowMessage.Text = "ShowMsg";
            this.btShowMessage.UseVisualStyleBackColor = true;
            this.btShowMessage.Click += new System.EventHandler(this.btShowMessage_Click);
            // 
            // btScanOff
            // 
            this.btScanOff.Location = new System.Drawing.Point(379, 59);
            this.btScanOff.Name = "btScanOff";
            this.btScanOff.Size = new System.Drawing.Size(108, 28);
            this.btScanOff.TabIndex = 1;
            this.btScanOff.Text = "Scan Off";
            this.btScanOff.UseVisualStyleBackColor = true;
            this.btScanOff.Click += new System.EventHandler(this.btScanOff_Click);
            // 
            // btScanOn
            // 
            this.btScanOn.Location = new System.Drawing.Point(379, 25);
            this.btScanOn.Name = "btScanOn";
            this.btScanOn.Size = new System.Drawing.Size(108, 28);
            this.btScanOn.TabIndex = 0;
            this.btScanOn.Text = "Scan On";
            this.btScanOn.UseVisualStyleBackColor = true;
            this.btScanOn.Click += new System.EventHandler(this.btScanOn_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.tbTemp);
            this.tabPage4.Controls.Add(this.btSetTemp);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(604, 390);
            this.tabPage4.TabIndex = 7;
            this.tabPage4.Text = "TK-4S";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(324, 44);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_2);
            // 
            // tbTemp
            // 
            this.tbTemp.Location = new System.Drawing.Point(218, 8);
            this.tbTemp.Name = "tbTemp";
            this.tbTemp.Size = new System.Drawing.Size(100, 21);
            this.tbTemp.TabIndex = 1;
            this.tbTemp.Text = "25";
            // 
            // btSetTemp
            // 
            this.btSetTemp.Location = new System.Drawing.Point(324, 6);
            this.btSetTemp.Name = "btSetTemp";
            this.btSetTemp.Size = new System.Drawing.Size(75, 23);
            this.btSetTemp.TabIndex = 0;
            this.btSetTemp.Text = "Set Temp";
            this.btSetTemp.UseVisualStyleBackColor = true;
            this.btSetTemp.Click += new System.EventHandler(this.btSetTemp_Click);
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
            this.Controls.Add(this.tbImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormMaster";
            this.Text = "FormMaster";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMaster_FormClosing);
            this.Shown += new System.EventHandler(this.FormMaster_Shown);
            this.tbImage.ResumeLayout(false);
            this.Part.ResumeLayout(false);
            this.Part.PerformLayout();
            this.Vision.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.SigmaCM2.ResumeLayout(false);
            this.SigmaCM2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbImage;
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
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox cbIdlerun;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btWeightCheck;
        private System.Windows.Forms.Button btSetHoldOn;
        private System.Windows.Forms.Button btSetHoldOff;
        private System.Windows.Forms.Button btSetZero;
        private System.Windows.Forms.Button btShowWeight;
        private System.Windows.Forms.TabPage SigmaCM2;
        private System.Windows.Forms.TextBox tbCh;
        private System.Windows.Forms.Button btSetLoadCh;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btCheckHeight;
        private System.Windows.Forms.Button btGetHeight;
        private System.Windows.Forms.Button btReZero;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btScanOn;
        private System.Windows.Forms.Button btScanOff;
        private System.Windows.Forms.Button btShowMessage;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox tbTemp;
        private System.Windows.Forms.Button btSetTemp;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btForcedHomeEnd;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cbBarcodeCh;

    }
}