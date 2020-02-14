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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Part = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.tbWorkCnt = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.cbIdlerun = new System.Windows.Forms.CheckBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btAllCheck = new System.Windows.Forms.Button();
            this.btAllReset = new System.Windows.Forms.Button();
            this.btPartAutorun = new System.Windows.Forms.Button();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.btPartReset = new System.Windows.Forms.Button();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.Rs232 = new System.Windows.Forms.TabPage();
            this.btSendCycle = new System.Windows.Forms.Button();
            this.cbCycle = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btSendCombobox = new System.Windows.Forms.Button();
            this.cbSendMsg = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRecive = new System.Windows.Forms.TextBox();
            this.tbSendMsg = new System.Windows.Forms.TextBox();
            this.btSendTextbox = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btBarOff = new System.Windows.Forms.Button();
            this.lbBarReadingText = new System.Windows.Forms.Label();
            this.btBarOn = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.Part.SuspendLayout();
            this.Rs232.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.Part);
            this.tabControl1.Controls.Add(this.Rs232);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(612, 416);
            this.tabControl1.TabIndex = 19;
            // 
            // Part
            // 
            this.Part.Controls.Add(this.label11);
            this.Part.Controls.Add(this.tbWorkCnt);
            this.Part.Controls.Add(this.button5);
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 300);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Tray Work Count";
            // 
            // tbWorkCnt
            // 
            this.tbWorkCnt.Location = new System.Drawing.Point(93, 297);
            this.tbWorkCnt.Name = "tbWorkCnt";
            this.tbWorkCnt.Size = new System.Drawing.Size(35, 20);
            this.tbWorkCnt.TabIndex = 28;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(7, 268);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(105, 23);
            this.button5.TabIndex = 27;
            this.button5.Text = "Tray Count Reset";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // cbIdlerun
            // 
            this.cbIdlerun.AutoSize = true;
            this.cbIdlerun.Location = new System.Drawing.Point(7, 235);
            this.cbIdlerun.Name = "cbIdlerun";
            this.cbIdlerun.Size = new System.Drawing.Size(66, 17);
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
            // Rs232
            // 
            this.Rs232.Controls.Add(this.btSendCycle);
            this.Rs232.Controls.Add(this.cbCycle);
            this.Rs232.Controls.Add(this.label2);
            this.Rs232.Controls.Add(this.button1);
            this.Rs232.Controls.Add(this.textBox1);
            this.Rs232.Controls.Add(this.btSendCombobox);
            this.Rs232.Controls.Add(this.cbSendMsg);
            this.Rs232.Controls.Add(this.label1);
            this.Rs232.Controls.Add(this.label9);
            this.Rs232.Controls.Add(this.label8);
            this.Rs232.Controls.Add(this.tbRecive);
            this.Rs232.Controls.Add(this.tbSendMsg);
            this.Rs232.Controls.Add(this.btSendTextbox);
            this.Rs232.Location = new System.Drawing.Point(4, 22);
            this.Rs232.Name = "Rs232";
            this.Rs232.Size = new System.Drawing.Size(604, 390);
            this.Rs232.TabIndex = 2;
            this.Rs232.Text = "LaserMark";
            this.Rs232.UseVisualStyleBackColor = true;
            // 
            // btSendCycle
            // 
            this.btSendCycle.Location = new System.Drawing.Point(425, 227);
            this.btSendCycle.Name = "btSendCycle";
            this.btSendCycle.Size = new System.Drawing.Size(159, 23);
            this.btSendCycle.TabIndex = 13;
            this.btSendCycle.Text = "Send Cycle";
            this.btSendCycle.UseVisualStyleBackColor = true;
            this.btSendCycle.Click += new System.EventHandler(this.btSendCycle_Click);
            // 
            // cbCycle
            // 
            this.cbCycle.FormattingEnabled = true;
            this.cbCycle.Items.AddRange(new object[] {
            "None       ",
            "ProjectLoad",
            "SetLotNo   ",
            "SetSerialNo",
            "Mark "});
            this.cbCycle.Location = new System.Drawing.Point(112, 229);
            this.cbCycle.Name = "cbCycle";
            this.cbCycle.Size = new System.Drawing.Size(289, 21);
            this.cbCycle.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Data To Marker";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(425, 82);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Send to Marker";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 82);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(386, 125);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "sunkye\r\nwon\r\nsdfsdf\r\nfdgsdfg";
            // 
            // btSendCombobox
            // 
            this.btSendCombobox.Location = new System.Drawing.Point(425, 14);
            this.btSendCombobox.Name = "btSendCombobox";
            this.btSendCombobox.Size = new System.Drawing.Size(159, 23);
            this.btSendCombobox.TabIndex = 8;
            this.btSendCombobox.Text = "Send to Marker";
            this.btSendCombobox.UseVisualStyleBackColor = true;
            this.btSendCombobox.Click += new System.EventHandler(this.btSendCombobox_Click);
            // 
            // cbSendMsg
            // 
            this.cbSendMsg.FormattingEnabled = true;
            this.cbSendMsg.Items.AddRange(new object[] {
            "GETCURRENTPROJECT",
            "LOADPROJECT \"store:MsgStore1/Format1\"",
            "MARK START",
            "BEGINTRANS",
            "SETTEXT \"LotNo0\" \"I8J09a\"",
            "SETTEXT \"SerialNo0\" \"00098\"",
            "SETTEXT \"DMC0\" \"I8J09a00098\"",
            "EXECTRANS",
            "TRIGGER",
            "SETMSG 1 1",
            "SETMSG 1 0",
            "SETMSG 2 1",
            "SETMSG 2 0",
            "SETMSG 3 1",
            "SETMSG 3 0"});
            this.cbSendMsg.Location = new System.Drawing.Point(112, 16);
            this.cbSendMsg.Name = "cbSendMsg";
            this.cbSendMsg.Size = new System.Drawing.Size(289, 21);
            this.cbSendMsg.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Data To Marker";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 342);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Received Text";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Data To Marker";
            // 
            // tbRecive
            // 
            this.tbRecive.Location = new System.Drawing.Point(112, 339);
            this.tbRecive.Name = "tbRecive";
            this.tbRecive.Size = new System.Drawing.Size(472, 20);
            this.tbRecive.TabIndex = 2;
            // 
            // tbSendMsg
            // 
            this.tbSendMsg.Location = new System.Drawing.Point(112, 45);
            this.tbSendMsg.Name = "tbSendMsg";
            this.tbSendMsg.Size = new System.Drawing.Size(289, 20);
            this.tbSendMsg.TabIndex = 1;
            this.tbSendMsg.Text = "GETCURRENTPROJECT";
            this.tbSendMsg.TextChanged += new System.EventHandler(this.tbSendMsg_TextChanged);
            // 
            // btSendTextbox
            // 
            this.btSendTextbox.Location = new System.Drawing.Point(425, 45);
            this.btSendTextbox.Name = "btSendTextbox";
            this.btSendTextbox.Size = new System.Drawing.Size(159, 23);
            this.btSendTextbox.TabIndex = 0;
            this.btSendTextbox.Text = "Send to Marker";
            this.btSendTextbox.UseVisualStyleBackColor = true;
            this.btSendTextbox.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btBarOff);
            this.tabPage1.Controls.Add(this.lbBarReadingText);
            this.tabPage1.Controls.Add(this.btBarOn);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(604, 390);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Barcode";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btBarOff
            // 
            this.btBarOff.Location = new System.Drawing.Point(125, 70);
            this.btBarOff.Name = "btBarOff";
            this.btBarOff.Size = new System.Drawing.Size(75, 23);
            this.btBarOff.TabIndex = 9;
            this.btBarOff.Text = "LOFF";
            this.btBarOff.UseVisualStyleBackColor = true;
            this.btBarOff.Click += new System.EventHandler(this.btBarOff_Click);
            // 
            // lbBarReadingText
            // 
            this.lbBarReadingText.AutoSize = true;
            this.lbBarReadingText.Location = new System.Drawing.Point(41, 42);
            this.lbBarReadingText.Name = "lbBarReadingText";
            this.lbBarReadingText.Size = new System.Drawing.Size(71, 13);
            this.lbBarReadingText.TabIndex = 8;
            this.lbBarReadingText.Text = "Reading Text";
            // 
            // btBarOn
            // 
            this.btBarOn.Location = new System.Drawing.Point(44, 70);
            this.btBarOn.Name = "btBarOn";
            this.btBarOn.Size = new System.Drawing.Size(75, 23);
            this.btBarOn.TabIndex = 7;
            this.btBarOn.Text = "LON";
            this.btBarOn.UseVisualStyleBackColor = true;
            this.btBarOn.Click += new System.EventHandler(this.btBarOn_Click);
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
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormMaster";
            this.Text = "FormMaster";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMaster_FormClosing);
            this.Shown += new System.EventHandler(this.FormMaster_Shown);
            this.tabControl1.ResumeLayout(false);
            this.Part.ResumeLayout(false);
            this.Part.PerformLayout();
            this.Rs232.ResumeLayout(false);
            this.Rs232.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Part;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btAllCheck;
        private System.Windows.Forms.Button btAllReset;
        private System.Windows.Forms.Button btPartAutorun;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.Button btPartReset;
        private System.Windows.Forms.ListView lvSequence;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.TabPage Rs232;
        private System.Windows.Forms.TextBox tbSendMsg;
        private System.Windows.Forms.Button btSendTextbox;
        private System.Windows.Forms.CheckBox cbIdlerun;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRecive;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbWorkCnt;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btBarOff;
        private System.Windows.Forms.Label lbBarReadingText;
        private System.Windows.Forms.Button btBarOn;
        private System.Windows.Forms.Button btSendCombobox;
        private System.Windows.Forms.ComboBox cbSendMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btSendCycle;
        private System.Windows.Forms.ComboBox cbCycle;
        private System.Windows.Forms.Label label2;
    }
}