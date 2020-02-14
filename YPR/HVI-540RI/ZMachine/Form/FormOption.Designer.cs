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
            this.cbPaintSkip = new System.Windows.Forms.CheckBox();
            this.cbMARKSkip = new System.Windows.Forms.CheckBox();
            this.cbVISNSkip = new System.Windows.Forms.CheckBox();
            this.cbLoadingStop = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbPaintName1 = new System.Windows.Forms.TextBox();
            this.tbPaintName5 = new System.Windows.Forms.TextBox();
            this.tbPaintName4 = new System.Windows.Forms.TextBox();
            this.tbPaintName3 = new System.Windows.Forms.TextBox();
            this.tbPaintName2 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cbStopBarcRead = new System.Windows.Forms.CheckBox();
            this.cbIgnrBarcode = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbVisnTurnCylTest = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.panel1.Controls.Add(this.cbVisnTurnCylTest);
            this.panel1.Controls.Add(this.cbPaintSkip);
            this.panel1.Controls.Add(this.cbMARKSkip);
            this.panel1.Controls.Add(this.cbVISNSkip);
            this.panel1.Controls.Add(this.cbLoadingStop);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 203);
            this.panel1.TabIndex = 30;
            // 
            // cbPaintSkip
            // 
            this.cbPaintSkip.AutoSize = true;
            this.cbPaintSkip.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbPaintSkip.Location = new System.Drawing.Point(10, 135);
            this.cbPaintSkip.Name = "cbPaintSkip";
            this.cbPaintSkip.Size = new System.Drawing.Size(130, 25);
            this.cbPaintSkip.TabIndex = 7;
            this.cbPaintSkip.Text = "Painting Skip";
            this.cbPaintSkip.UseVisualStyleBackColor = true;
            // 
            // cbMARKSkip
            // 
            this.cbMARKSkip.AutoSize = true;
            this.cbMARKSkip.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbMARKSkip.Location = new System.Drawing.Point(10, 107);
            this.cbMARKSkip.Name = "cbMARKSkip";
            this.cbMARKSkip.Size = new System.Drawing.Size(130, 25);
            this.cbMARKSkip.TabIndex = 6;
            this.cbMARKSkip.Text = "Marking Skip";
            this.cbMARKSkip.UseVisualStyleBackColor = true;
            // 
            // cbVISNSkip
            // 
            this.cbVISNSkip.AutoSize = true;
            this.cbVISNSkip.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbVISNSkip.Location = new System.Drawing.Point(10, 78);
            this.cbVISNSkip.Name = "cbVISNSkip";
            this.cbVISNSkip.Size = new System.Drawing.Size(113, 25);
            this.cbVISNSkip.TabIndex = 5;
            this.cbVISNSkip.Text = "Vision Skip";
            this.cbVISNSkip.UseVisualStyleBackColor = true;
            // 
            // cbLoadingStop
            // 
            this.cbLoadingStop.AutoSize = true;
            this.cbLoadingStop.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbLoadingStop.Location = new System.Drawing.Point(10, 49);
            this.cbLoadingStop.Name = "cbLoadingStop";
            this.cbLoadingStop.Size = new System.Drawing.Size(122, 25);
            this.cbLoadingStop.TabIndex = 2;
            this.cbLoadingStop.Text = "Loader Stop";
            this.cbLoadingStop.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.LightCoral;
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label27.Location = new System.Drawing.Point(-1, -1);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(414, 35);
            this.label27.TabIndex = 0;
            this.label27.Text = "OPTION";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LightCoral;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(-1, -1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(414, 35);
            this.label4.TabIndex = 0;
            this.label4.Text = "PAINT Name";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 21);
            this.label3.TabIndex = 4;
            this.label3.Text = "No. 1 PAINT COLOR NAME";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(10, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(218, 21);
            this.label11.TabIndex = 11;
            this.label11.Text = "No. 4 PAINT COLOR NAME";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(10, 107);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(218, 21);
            this.label12.TabIndex = 12;
            this.label12.Text = "No. 3 PAINT COLOR NAME";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(10, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(218, 21);
            this.label13.TabIndex = 13;
            this.label13.Text = "No. 2 PAINT COLOR NAME";
            // 
            // tbPaintName1
            // 
            this.tbPaintName1.Location = new System.Drawing.Point(252, 49);
            this.tbPaintName1.Name = "tbPaintName1";
            this.tbPaintName1.Size = new System.Drawing.Size(140, 23);
            this.tbPaintName1.TabIndex = 14;
            // 
            // tbPaintName5
            // 
            this.tbPaintName5.Location = new System.Drawing.Point(252, 165);
            this.tbPaintName5.Name = "tbPaintName5";
            this.tbPaintName5.Size = new System.Drawing.Size(140, 23);
            this.tbPaintName5.TabIndex = 18;
            // 
            // tbPaintName4
            // 
            this.tbPaintName4.Location = new System.Drawing.Point(252, 136);
            this.tbPaintName4.Name = "tbPaintName4";
            this.tbPaintName4.Size = new System.Drawing.Size(140, 23);
            this.tbPaintName4.TabIndex = 17;
            // 
            // tbPaintName3
            // 
            this.tbPaintName3.Location = new System.Drawing.Point(252, 107);
            this.tbPaintName3.Name = "tbPaintName3";
            this.tbPaintName3.Size = new System.Drawing.Size(140, 23);
            this.tbPaintName3.TabIndex = 16;
            // 
            // tbPaintName2
            // 
            this.tbPaintName2.Location = new System.Drawing.Point(252, 78);
            this.tbPaintName2.Name = "tbPaintName2";
            this.tbPaintName2.Size = new System.Drawing.Size(140, 23);
            this.tbPaintName2.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tbPaintName2);
            this.panel2.Controls.Add(this.tbPaintName3);
            this.panel2.Controls.Add(this.tbPaintName4);
            this.panel2.Controls.Add(this.tbPaintName5);
            this.panel2.Controls.Add(this.tbPaintName1);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(422, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 203);
            this.panel2.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(10, 165);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(218, 21);
            this.label10.TabIndex = 10;
            this.label10.Text = "No. 5 PAINT COLOR NAME";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.cbStopBarcRead);
            this.panel3.Controls.Add(this.cbIgnrBarcode);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(2, 212);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(834, 143);
            this.panel3.TabIndex = 31;
            // 
            // cbStopBarcRead
            // 
            this.cbStopBarcRead.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbStopBarcRead.Location = new System.Drawing.Point(10, 80);
            this.cbStopBarcRead.Name = "cbStopBarcRead";
            this.cbStopBarcRead.Size = new System.Drawing.Size(435, 53);
            this.cbStopBarcRead.TabIndex = 3;
            this.cbStopBarcRead.Text = "READING BARCODE USING THE STOPPED POSITION (NEED TO USE BARCORD)";
            this.cbStopBarcRead.UseVisualStyleBackColor = true;
            // 
            // cbIgnrBarcode
            // 
            this.cbIgnrBarcode.AutoSize = true;
            this.cbIgnrBarcode.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbIgnrBarcode.Location = new System.Drawing.Point(10, 49);
            this.cbIgnrBarcode.Name = "cbIgnrBarcode";
            this.cbIgnrBarcode.Size = new System.Drawing.Size(170, 25);
            this.cbIgnrBarcode.TabIndex = 2;
            this.cbIgnrBarcode.Text = "IGNORE BARCODE";
            this.cbIgnrBarcode.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(-1, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(834, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "BARCODE OPTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbVisnTurnCylTest
            // 
            this.cbVisnTurnCylTest.AutoSize = true;
            this.cbVisnTurnCylTest.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbVisnTurnCylTest.Location = new System.Drawing.Point(10, 164);
            this.cbVisnTurnCylTest.Name = "cbVisnTurnCylTest";
            this.cbVisnTurnCylTest.Size = new System.Drawing.Size(335, 25);
            this.cbVisnTurnCylTest.TabIndex = 8;
            this.cbVisnTurnCylTest.Text = "Turn Rotary Cylinder Test in Vision Skip";
            this.cbVisnTurnCylTest.UseVisualStyleBackColor = true;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbMARKSkip;
        private System.Windows.Forms.CheckBox cbVISNSkip;
        private System.Windows.Forms.CheckBox cbLoadingStop;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbPaintName1;
        private System.Windows.Forms.TextBox tbPaintName5;
        private System.Windows.Forms.TextBox tbPaintName4;
        private System.Windows.Forms.TextBox tbPaintName3;
        private System.Windows.Forms.TextBox tbPaintName2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbPaintSkip;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox cbIgnrBarcode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbStopBarcRead;
        private System.Windows.Forms.CheckBox cbVisnTurnCylTest;
    }
}