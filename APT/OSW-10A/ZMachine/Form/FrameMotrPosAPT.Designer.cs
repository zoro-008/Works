namespace Machine
{
    partial class FrameMotrPosAPT
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
            this.pnMotr0 = new System.Windows.Forms.Panel();
            this.lbAxisNo = new System.Windows.Forms.Label();
            this.lbCmdPos = new System.Windows.Forms.Label();
            this.lbStat1 = new System.Windows.Forms.Label();
            this.lbStat2 = new System.Windows.Forms.Label();
            this.lbStat3 = new System.Windows.Forms.Label();
            this.lbStat4 = new System.Windows.Forms.Label();
            this.lbStat5 = new System.Windows.Forms.Label();
            this.lbStat7 = new System.Windows.Forms.Label();
            this.lbStat8 = new System.Windows.Forms.Label();
            this.lbAxisName = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.lbStat6 = new System.Windows.Forms.Label();
            this.btServoOn = new System.Windows.Forms.Button();
            this.btServoOff = new System.Windows.Forms.Button();
            this.btHome = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pnMotr0
            // 
            this.pnMotr0.Location = new System.Drawing.Point(-7, 43);
            this.pnMotr0.Name = "pnMotr0";
            this.pnMotr0.Size = new System.Drawing.Size(418, 500);
            this.pnMotr0.TabIndex = 0;
            // 
            // lbAxisNo
            // 
            this.lbAxisNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAxisNo.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAxisNo.ForeColor = System.Drawing.Color.Red;
            this.lbAxisNo.Location = new System.Drawing.Point(-1, -1);
            this.lbAxisNo.Name = "lbAxisNo";
            this.lbAxisNo.Size = new System.Drawing.Size(72, 20);
            this.lbAxisNo.TabIndex = 5;
            this.lbAxisNo.Text = "Axis 13";
            this.lbAxisNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCmdPos
            // 
            this.lbCmdPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCmdPos.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCmdPos.ForeColor = System.Drawing.Color.Blue;
            this.lbCmdPos.Location = new System.Drawing.Point(72, -1);
            this.lbCmdPos.Name = "lbCmdPos";
            this.lbCmdPos.Size = new System.Drawing.Size(99, 20);
            this.lbCmdPos.TabIndex = 6;
            this.lbCmdPos.Text = "141.243";
            this.lbCmdPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat1
            // 
            this.lbStat1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat1.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat1.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat1.Location = new System.Drawing.Point(172, -1);
            this.lbStat1.Name = "lbStat1";
            this.lbStat1.Size = new System.Drawing.Size(23, 20);
            this.lbStat1.TabIndex = 7;
            this.lbStat1.Text = "-L";
            this.lbStat1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat2
            // 
            this.lbStat2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat2.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat2.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat2.Location = new System.Drawing.Point(196, -1);
            this.lbStat2.Name = "lbStat2";
            this.lbStat2.Size = new System.Drawing.Size(23, 20);
            this.lbStat2.TabIndex = 8;
            this.lbStat2.Text = "O";
            this.lbStat2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat3
            // 
            this.lbStat3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat3.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat3.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat3.Location = new System.Drawing.Point(220, -1);
            this.lbStat3.Name = "lbStat3";
            this.lbStat3.Size = new System.Drawing.Size(23, 20);
            this.lbStat3.TabIndex = 9;
            this.lbStat3.Text = "+L";
            this.lbStat3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat4
            // 
            this.lbStat4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat4.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat4.ForeColor = System.Drawing.Color.Red;
            this.lbStat4.Location = new System.Drawing.Point(244, -1);
            this.lbStat4.Name = "lbStat4";
            this.lbStat4.Size = new System.Drawing.Size(23, 20);
            this.lbStat4.TabIndex = 10;
            this.lbStat4.Text = "H";
            this.lbStat4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat5
            // 
            this.lbStat5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat5.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat5.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat5.Location = new System.Drawing.Point(268, -1);
            this.lbStat5.Name = "lbStat5";
            this.lbStat5.Size = new System.Drawing.Size(32, 20);
            this.lbStat5.TabIndex = 11;
            this.lbStat5.Text = "Alm";
            this.lbStat5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat7
            // 
            this.lbStat7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat7.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat7.ForeColor = System.Drawing.Color.Red;
            this.lbStat7.Location = new System.Drawing.Point(336, -1);
            this.lbStat7.Name = "lbStat7";
            this.lbStat7.Size = new System.Drawing.Size(31, 20);
            this.lbStat7.TabIndex = 13;
            this.lbStat7.Text = "INP";
            this.lbStat7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat8
            // 
            this.lbStat8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat8.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat8.ForeColor = System.Drawing.Color.Red;
            this.lbStat8.Location = new System.Drawing.Point(368, -1);
            this.lbStat8.Name = "lbStat8";
            this.lbStat8.Size = new System.Drawing.Size(38, 20);
            this.lbStat8.TabIndex = 15;
            this.lbStat8.Text = "Svr";
            this.lbStat8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAxisName
            // 
            this.lbAxisName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAxisName.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAxisName.ForeColor = System.Drawing.Color.Blue;
            this.lbAxisName.Location = new System.Drawing.Point(-1, 21);
            this.lbAxisName.Name = "lbAxisName";
            this.lbAxisName.Size = new System.Drawing.Size(193, 20);
            this.lbAxisName.TabIndex = 16;
            this.lbAxisName.Text = "[13] Loader M/Z Z";
            this.lbAxisName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // lbStat6
            // 
            this.lbStat6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat6.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStat6.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat6.Location = new System.Drawing.Point(301, -1);
            this.lbStat6.Name = "lbStat6";
            this.lbStat6.Size = new System.Drawing.Size(34, 20);
            this.lbStat6.TabIndex = 12;
            this.lbStat6.Text = "Stp";
            this.lbStat6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btServoOn
            // 
            this.btServoOn.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOn.Location = new System.Drawing.Point(193, 20);
            this.btServoOn.Name = "btServoOn";
            this.btServoOn.Size = new System.Drawing.Size(70, 21);
            this.btServoOn.TabIndex = 0;
            this.btServoOn.Text = "Servo On";
            this.btServoOn.UseVisualStyleBackColor = true;
            this.btServoOn.Click += new System.EventHandler(this.btServoOn_Click);
            // 
            // btServoOff
            // 
            this.btServoOff.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOff.Location = new System.Drawing.Point(263, 20);
            this.btServoOff.Name = "btServoOff";
            this.btServoOff.Size = new System.Drawing.Size(70, 21);
            this.btServoOff.TabIndex = 17;
            this.btServoOff.Text = "Servo Off";
            this.btServoOff.UseVisualStyleBackColor = true;
            this.btServoOff.Click += new System.EventHandler(this.btServoOff_Click);
            // 
            // btHome
            // 
            this.btHome.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btHome.Location = new System.Drawing.Point(334, 20);
            this.btHome.Name = "btHome";
            this.btHome.Size = new System.Drawing.Size(70, 21);
            this.btHome.TabIndex = 18;
            this.btHome.Text = "Home";
            this.btHome.UseVisualStyleBackColor = true;
            this.btHome.Click += new System.EventHandler(this.btHome_Click);
            // 
            // FrameMotrPosAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(405, 530);
            this.Controls.Add(this.btHome);
            this.Controls.Add(this.btServoOff);
            this.Controls.Add(this.btServoOn);
            this.Controls.Add(this.lbStat6);
            this.Controls.Add(this.lbAxisName);
            this.Controls.Add(this.lbStat8);
            this.Controls.Add(this.lbStat7);
            this.Controls.Add(this.lbStat5);
            this.Controls.Add(this.lbStat4);
            this.Controls.Add(this.lbStat3);
            this.Controls.Add(this.lbStat2);
            this.Controls.Add(this.lbStat1);
            this.Controls.Add(this.lbCmdPos);
            this.Controls.Add(this.lbAxisNo);
            this.Controls.Add(this.pnMotr0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameMotrPosAPT";
            this.Text = "FrameMotrPosAPT";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnMotr0;
        private System.Windows.Forms.Label lbAxisNo;
        private System.Windows.Forms.Label lbCmdPos;
        private System.Windows.Forms.Label lbStat1;
        private System.Windows.Forms.Label lbStat2;
        private System.Windows.Forms.Label lbStat3;
        private System.Windows.Forms.Label lbStat4;
        private System.Windows.Forms.Label lbStat5;
        private System.Windows.Forms.Label lbStat7;
        private System.Windows.Forms.Label lbStat8;
        private System.Windows.Forms.Label lbAxisName;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Label lbStat6;
        private System.Windows.Forms.Button btServoOn;
        private System.Windows.Forms.Button btServoOff;
        private System.Windows.Forms.Button btHome;
    }
}