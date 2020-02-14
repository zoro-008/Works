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
            this.cbUseYBfPos = new System.Windows.Forms.CheckBox();
            this.cbUseRear = new System.Windows.Forms.CheckBox();
            this.cbUseFrnt = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbRewindRVel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRewindMode = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRewindYVel = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.panel1.Controls.Add(this.cbUseYBfPos);
            this.panel1.Controls.Add(this.cbUseRear);
            this.panel1.Controls.Add(this.cbUseFrnt);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 221);
            this.panel1.TabIndex = 30;
            // 
            // cbUseYBfPos
            // 
            this.cbUseYBfPos.AutoSize = true;
            this.cbUseYBfPos.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbUseYBfPos.Location = new System.Drawing.Point(11, 108);
            this.cbUseYBfPos.Name = "cbUseYBfPos";
            this.cbUseYBfPos.Size = new System.Drawing.Size(201, 25);
            this.cbUseYBfPos.TabIndex = 4;
            this.cbUseYBfPos.Text = "칼날 Y 대기포지션 사용";
            this.cbUseYBfPos.UseVisualStyleBackColor = true;
            // 
            // cbUseRear
            // 
            this.cbUseRear.AutoSize = true;
            this.cbUseRear.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbUseRear.Location = new System.Drawing.Point(11, 77);
            this.cbUseRear.Name = "cbUseRear";
            this.cbUseRear.Size = new System.Drawing.Size(121, 25);
            this.cbUseRear.TabIndex = 3;
            this.cbUseRear.Text = "뒷쪽 실 사용";
            this.cbUseRear.UseVisualStyleBackColor = true;
            // 
            // cbUseFrnt
            // 
            this.cbUseFrnt.AutoSize = true;
            this.cbUseFrnt.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbUseFrnt.Location = new System.Drawing.Point(11, 46);
            this.cbUseFrnt.Name = "cbUseFrnt";
            this.cbUseFrnt.Size = new System.Drawing.Size(121, 25);
            this.cbUseFrnt.TabIndex = 2;
            this.cbUseFrnt.Text = "앞쪽 실 사용";
            this.cbUseFrnt.UseVisualStyleBackColor = true;
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
            this.label27.Text = "OPTION";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbRewindYVel);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbRewindMode);
            this.panel2.Controls.Add(this.tbRewindRVel);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(1, 222);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 221);
            this.panel2.TabIndex = 31;
            // 
            // tbRewindRVel
            // 
            this.tbRewindRVel.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbRewindRVel.Location = new System.Drawing.Point(210, 78);
            this.tbRewindRVel.Name = "tbRewindRVel";
            this.tbRewindRVel.Size = new System.Drawing.Size(100, 29);
            this.tbRewindRVel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(-1, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(414, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "줄 되감기 모드";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbRewindMode
            // 
            this.cbRewindMode.AutoSize = true;
            this.cbRewindMode.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.cbRewindMode.Location = new System.Drawing.Point(10, 47);
            this.cbRewindMode.Name = "cbRewindMode";
            this.cbRewindMode.Size = new System.Drawing.Size(137, 25);
            this.cbRewindMode.TabIndex = 6;
            this.cbRewindMode.Text = "줄 되감기 모드";
            this.cbRewindMode.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(7, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 21);
            this.label2.TabIndex = 7;
            this.label2.Text = "줄 되감기 속도 (mm/sec)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(7, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(191, 21);
            this.label3.TabIndex = 9;
            this.label3.Text = "가이드 Y 속도 (mm/sec)";
            // 
            // tbRewindYVel
            // 
            this.tbRewindYVel.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbRewindYVel.Location = new System.Drawing.Point(210, 115);
            this.tbRewindYVel.Name = "tbRewindYVel";
            this.tbRewindYVel.Size = new System.Drawing.Size(100, 29);
            this.tbRewindYVel.TabIndex = 8;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1280, 863);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.CheckBox cbUseRear;
        private System.Windows.Forms.CheckBox cbUseFrnt;
        private System.Windows.Forms.CheckBox cbUseYBfPos;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbRewindMode;
        private System.Windows.Forms.TextBox tbRewindRVel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbRewindYVel;
    }
}