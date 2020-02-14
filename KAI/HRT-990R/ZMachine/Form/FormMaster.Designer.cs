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
            this.tbImage = new System.Windows.Forms.TabControl();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.tbOneShotTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTrgOfs = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMsgData = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbCSSignal = new System.Windows.Forms.ComboBox();
            this.btMsgSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRcvDataId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btMsgClear = new System.Windows.Forms.Button();
            this.tbRcvData = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.btPartReset = new System.Windows.Forms.Button();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.btPartAutorun = new System.Windows.Forms.Button();
            this.btAllReset = new System.Windows.Forms.Button();
            this.btAllCheck = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.cbIdlerun = new System.Windows.Forms.CheckBox();
            this.Part = new System.Windows.Forms.TabPage();
            this.tbImage.SuspendLayout();
            this.Part.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbImage
            // 
            this.tbImage.Controls.Add(this.Part);
            this.tbImage.Location = new System.Drawing.Point(12, 12);
            this.tbImage.Name = "tbImage";
            this.tbImage.SelectedIndex = 0;
            this.tbImage.Size = new System.Drawing.Size(612, 416);
            this.tbImage.TabIndex = 19;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(168, 33);
            this.button1.TabIndex = 33;
            this.button1.Text = "Shot";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tbOneShotTime
            // 
            this.tbOneShotTime.Location = new System.Drawing.Point(76, 27);
            this.tbOneShotTime.Name = "tbOneShotTime";
            this.tbOneShotTime.Size = new System.Drawing.Size(102, 21);
            this.tbOneShotTime.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 35;
            // 
            // tbTrgOfs
            // 
            this.tbTrgOfs.Location = new System.Drawing.Point(76, 27);
            this.tbTrgOfs.Name = "tbTrgOfs";
            this.tbTrgOfs.Size = new System.Drawing.Size(79, 21);
            this.tbTrgOfs.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 12);
            this.label3.TabIndex = 35;
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 38;
            // 
            // tbMsgData
            // 
            this.tbMsgData.Location = new System.Drawing.Point(86, 53);
            this.tbMsgData.Name = "tbMsgData";
            this.tbMsgData.Size = new System.Drawing.Size(225, 21);
            this.tbMsgData.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 35;
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
            // btMsgSend
            // 
            this.btMsgSend.Location = new System.Drawing.Point(138, 130);
            this.btMsgSend.Name = "btMsgSend";
            this.btMsgSend.Size = new System.Drawing.Size(178, 33);
            this.btMsgSend.TabIndex = 36;
            this.btMsgSend.Text = "Send";
            this.btMsgSend.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 12);
            this.label1.TabIndex = 39;
            // 
            // tbRcvDataId
            // 
            this.tbRcvDataId.Location = new System.Drawing.Point(86, 80);
            this.tbRcvDataId.Name = "tbRcvDataId";
            this.tbRcvDataId.Size = new System.Drawing.Size(225, 21);
            this.tbRcvDataId.TabIndex = 40;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 12);
            this.label6.TabIndex = 41;
            // 
            // btMsgClear
            // 
            this.btMsgClear.Location = new System.Drawing.Point(11, 130);
            this.btMsgClear.Name = "btMsgClear";
            this.btMsgClear.Size = new System.Drawing.Size(121, 33);
            this.btMsgClear.TabIndex = 43;
            this.btMsgClear.Text = "Clear";
            this.btMsgClear.UseVisualStyleBackColor = true;
            // 
            // tbRcvData
            // 
            this.tbRcvData.Location = new System.Drawing.Point(86, 105);
            this.tbRcvData.Name = "tbRcvData";
            this.tbRcvData.Size = new System.Drawing.Size(225, 21);
            this.tbRcvData.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 12);
            this.label7.TabIndex = 45;
            // 
            // lvSequence
            // 
            this.lvSequence.Location = new System.Drawing.Point(133, 22);
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.Size = new System.Drawing.Size(465, 283);
            this.lvSequence.TabIndex = 19;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
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
            // Part
            // 
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbImage;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.TabPage Part;
        private System.Windows.Forms.CheckBox cbIdlerun;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btAllCheck;
        private System.Windows.Forms.Button btAllReset;
        private System.Windows.Forms.Button btPartAutorun;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.Button btPartReset;
        private System.Windows.Forms.ListView lvSequence;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbOneShotTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTrgOfs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMsgData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbCSSignal;
        private System.Windows.Forms.Button btMsgSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRcvDataId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btMsgClear;
        private System.Windows.Forms.TextBox tbRcvData;
        private System.Windows.Forms.Label label7;
    }
}