﻿namespace Machine
{
    partial class FraMotr
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LbCmdPos = new System.Windows.Forms.Label();
            this.LbEncPos = new System.Windows.Forms.Label();
            this.cbSlow = new System.Windows.Forms.CheckBox();
            this.LbStat1 = new System.Windows.Forms.Label();
            this.LbStat2 = new System.Windows.Forms.Label();
            this.LbStat3 = new System.Windows.Forms.Label();
            this.LbStat4 = new System.Windows.Forms.Label();
            this.LbStat5 = new System.Windows.Forms.Label();
            this.LbStat6 = new System.Windows.Forms.Label();
            this.LbStat7 = new System.Windows.Forms.Label();
            this.btPos = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.btNeg = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "CMD POS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(0, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "ENC POS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbCmdPos
            // 
            this.LbCmdPos.BackColor = System.Drawing.Color.Silver;
            this.LbCmdPos.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbCmdPos.Location = new System.Drawing.Point(83, 31);
            this.LbCmdPos.Name = "LbCmdPos";
            this.LbCmdPos.Size = new System.Drawing.Size(142, 23);
            this.LbCmdPos.TabIndex = 5;
            this.LbCmdPos.Text = "0.0";
            this.LbCmdPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbEncPos
            // 
            this.LbEncPos.BackColor = System.Drawing.Color.Silver;
            this.LbEncPos.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbEncPos.Location = new System.Drawing.Point(82, 58);
            this.LbEncPos.Name = "LbEncPos";
            this.LbEncPos.Size = new System.Drawing.Size(142, 23);
            this.LbEncPos.TabIndex = 6;
            this.LbEncPos.Text = "0.0";
            this.LbEncPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbSlow
            // 
            this.cbSlow.AutoSize = true;
            this.cbSlow.Location = new System.Drawing.Point(214, 8);
            this.cbSlow.Name = "cbSlow";
            this.cbSlow.Size = new System.Drawing.Size(15, 14);
            this.cbSlow.TabIndex = 7;
            this.cbSlow.UseVisualStyleBackColor = true;
            this.cbSlow.Click += new System.EventHandler(this.cbSlow_Click);
            // 
            // LbStat1
            // 
            this.LbStat1.BackColor = System.Drawing.Color.Silver;
            this.LbStat1.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat1.Location = new System.Drawing.Point(1, 5);
            this.LbStat1.Name = "LbStat1";
            this.LbStat1.Size = new System.Drawing.Size(29, 21);
            this.LbStat1.TabIndex = 8;
            this.LbStat1.Text = "-L";
            this.LbStat1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat2
            // 
            this.LbStat2.BackColor = System.Drawing.Color.Silver;
            this.LbStat2.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.LbStat2.Location = new System.Drawing.Point(31, 5);
            this.LbStat2.Name = "LbStat2";
            this.LbStat2.Size = new System.Drawing.Size(29, 21);
            this.LbStat2.TabIndex = 9;
            this.LbStat2.Text = "HM";
            this.LbStat2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat3
            // 
            this.LbStat3.BackColor = System.Drawing.Color.Silver;
            this.LbStat3.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat3.Location = new System.Drawing.Point(61, 5);
            this.LbStat3.Name = "LbStat3";
            this.LbStat3.Size = new System.Drawing.Size(29, 21);
            this.LbStat3.TabIndex = 10;
            this.LbStat3.Text = "+L";
            this.LbStat3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat4
            // 
            this.LbStat4.BackColor = System.Drawing.Color.Silver;
            this.LbStat4.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat4.Location = new System.Drawing.Point(91, 5);
            this.LbStat4.Name = "LbStat4";
            this.LbStat4.Size = new System.Drawing.Size(29, 21);
            this.LbStat4.TabIndex = 11;
            this.LbStat4.Text = "AR";
            this.LbStat4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat5
            // 
            this.LbStat5.BackColor = System.Drawing.Color.Silver;
            this.LbStat5.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat5.Location = new System.Drawing.Point(121, 5);
            this.LbStat5.Name = "LbStat5";
            this.LbStat5.Size = new System.Drawing.Size(29, 21);
            this.LbStat5.TabIndex = 12;
            this.LbStat5.Text = "SV";
            this.LbStat5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat6
            // 
            this.LbStat6.BackColor = System.Drawing.Color.Silver;
            this.LbStat6.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat6.Location = new System.Drawing.Point(151, 5);
            this.LbStat6.Name = "LbStat6";
            this.LbStat6.Size = new System.Drawing.Size(29, 21);
            this.LbStat6.TabIndex = 13;
            this.LbStat6.Text = "ST";
            this.LbStat6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LbStat7
            // 
            this.LbStat7.BackColor = System.Drawing.Color.Silver;
            this.LbStat7.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbStat7.Location = new System.Drawing.Point(181, 5);
            this.LbStat7.Name = "LbStat7";
            this.LbStat7.Size = new System.Drawing.Size(29, 21);
            this.LbStat7.TabIndex = 14;
            this.LbStat7.Text = "HD";
            this.LbStat7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btPos
            // 
            this.btPos.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btPos.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btPos.Location = new System.Drawing.Point(134, 83);
            this.btPos.Name = "btPos";
            this.btPos.Size = new System.Drawing.Size(95, 41);
            this.btPos.TabIndex = 1;
            this.btPos.Tag = "0";
            this.btPos.Text = "RIGHT(+)";
            this.btPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btPos.UseVisualStyleBackColor = true;
            this.btPos.Click += new System.EventHandler(this.btPos_Click);
            this.btPos.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btPos_MouseDown);
            this.btPos.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btPos_MouseUp);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // btNeg
            // 
            this.btNeg.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btNeg.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btNeg.Location = new System.Drawing.Point(1, 83);
            this.btNeg.Name = "btNeg";
            this.btNeg.Size = new System.Drawing.Size(95, 41);
            this.btNeg.TabIndex = 0;
            this.btNeg.Tag = "0";
            this.btNeg.Text = "LEFT(-)";
            this.btNeg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btNeg.UseCompatibleTextRendering = true;
            this.btNeg.UseVisualStyleBackColor = true;
            this.btNeg.Click += new System.EventHandler(this.btNeg_Click);
            this.btNeg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btNeg_MouseDown);
            this.btNeg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btNeg_MouseUp);
            // 
            // btStop
            // 
            this.btStop.Image = global::Machine.Properties.Resources.Stop;
            this.btStop.Location = new System.Drawing.Point(95, 83);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(40, 41);
            this.btStop.TabIndex = 2;
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // FraMotr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(230, 125);
            this.Controls.Add(this.LbStat7);
            this.Controls.Add(this.LbStat6);
            this.Controls.Add(this.LbStat5);
            this.Controls.Add(this.LbStat4);
            this.Controls.Add(this.LbStat3);
            this.Controls.Add(this.LbStat2);
            this.Controls.Add(this.LbStat1);
            this.Controls.Add(this.cbSlow);
            this.Controls.Add(this.LbEncPos);
            this.Controls.Add(this.LbCmdPos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btPos);
            this.Controls.Add(this.btNeg);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(2, 0);
            this.Name = "FraMotr";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FraMotr";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FraMotr_FormClosing);
            this.Shown += new System.EventHandler(this.FraMotr_Shown);
            this.VisibleChanged += new System.EventHandler(this.FraMotr_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btNeg;
        private System.Windows.Forms.Button btPos;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LbCmdPos;
        private System.Windows.Forms.Label LbEncPos;
        private System.Windows.Forms.CheckBox cbSlow;
        private System.Windows.Forms.Label LbStat1;
        private System.Windows.Forms.Label LbStat2;
        private System.Windows.Forms.Label LbStat3;
        private System.Windows.Forms.Label LbStat4;
        private System.Windows.Forms.Label LbStat5;
        private System.Windows.Forms.Label LbStat6;
        private System.Windows.Forms.Label LbStat7;
        private System.Windows.Forms.Timer tmUpdate;
    }
}