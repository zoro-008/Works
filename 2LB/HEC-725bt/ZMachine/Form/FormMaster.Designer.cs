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
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
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
            this.button5.Text = "iLDRSplyCnt Zero";
            this.button5.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.CheckBox cbIdlerun;
        private System.Windows.Forms.Button btForcedHomeEnd;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;

    }
}