namespace Machine
{
    partial class FormDispCh
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDspVacPres = new System.Windows.Forms.TextBox();
            this.tbDspPrsPres = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btSigmaMode = new System.Windows.Forms.Button();
            this.btCheckAmount = new System.Windows.Forms.Button();
            this.lbAmount = new System.Windows.Forms.Label();
            this.btSetDspData = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbDspVacPres);
            this.panel1.Controls.Add(this.tbDspPrsPres);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(19, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(283, 53);
            this.panel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(239, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "kPa";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(239, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "kPa";
            // 
            // tbDspVacPres
            // 
            this.tbDspVacPres.Enabled = false;
            this.tbDspVacPres.Location = new System.Drawing.Point(165, 27);
            this.tbDspVacPres.Name = "tbDspVacPres";
            this.tbDspVacPres.Size = new System.Drawing.Size(69, 21);
            this.tbDspVacPres.TabIndex = 3;
            // 
            // tbDspPrsPres
            // 
            this.tbDspPrsPres.Enabled = false;
            this.tbDspPrsPres.Location = new System.Drawing.Point(165, 4);
            this.tbDspPrsPres.Name = "tbDspPrsPres";
            this.tbDspPrsPres.Size = new System.Drawing.Size(69, 21);
            this.tbDspPrsPres.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(2, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dispensor Vacuum";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(2, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dispensor Pressure";
            // 
            // btSigmaMode
            // 
            this.btSigmaMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btSigmaMode.Location = new System.Drawing.Point(26, 6);
            this.btSigmaMode.Name = "btSigmaMode";
            this.btSigmaMode.Size = new System.Drawing.Size(104, 25);
            this.btSigmaMode.TabIndex = 1;
            this.btSigmaMode.Text = "SIGMA ON";
            this.btSigmaMode.UseVisualStyleBackColor = true;
            this.btSigmaMode.Click += new System.EventHandler(this.btSigmaMode_Click);
            // 
            // btCheckAmount
            // 
            this.btCheckAmount.Location = new System.Drawing.Point(132, 6);
            this.btCheckAmount.Name = "btCheckAmount";
            this.btCheckAmount.Size = new System.Drawing.Size(104, 25);
            this.btCheckAmount.TabIndex = 2;
            this.btCheckAmount.Text = "CHECK EPOXY";
            this.btCheckAmount.UseVisualStyleBackColor = true;
            this.btCheckAmount.Click += new System.EventHandler(this.btCheckAmount_Click);
            // 
            // lbAmount
            // 
            this.lbAmount.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAmount.Location = new System.Drawing.Point(240, 11);
            this.lbAmount.Name = "lbAmount";
            this.lbAmount.Size = new System.Drawing.Size(62, 18);
            this.lbAmount.TabIndex = 3;
            this.lbAmount.Text = "%";
            // 
            // btSetDspData
            // 
            this.btSetDspData.Location = new System.Drawing.Point(198, 94);
            this.btSetDspData.Name = "btSetDspData";
            this.btSetDspData.Size = new System.Drawing.Size(104, 25);
            this.btSetDspData.TabIndex = 4;
            this.btSetDspData.Text = "Set Dispensor";
            this.btSetDspData.UseVisualStyleBackColor = true;
            this.btSetDspData.Click += new System.EventHandler(this.btSetDspData_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FormDispCh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 152);
            this.Controls.Add(this.btSetDspData);
            this.Controls.Add(this.lbAmount);
            this.Controls.Add(this.btCheckAmount);
            this.Controls.Add(this.btSigmaMode);
            this.Controls.Add(this.panel1);
            this.Name = "FormDispCh";
            this.Text = "FormDispCh";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDspVacPres;
        private System.Windows.Forms.TextBox tbDspPrsPres;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSigmaMode;
        private System.Windows.Forms.Button btCheckAmount;
        private System.Windows.Forms.Label lbAmount;
        private System.Windows.Forms.Button btSetDspData;
        private System.Windows.Forms.Timer tmUpdate;
    }
}