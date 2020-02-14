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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnMotr0
            // 
            this.pnMotr0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMotr0.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pnMotr0.Location = new System.Drawing.Point(0, 0);
            this.pnMotr0.Margin = new System.Windows.Forms.Padding(0);
            this.pnMotr0.Name = "pnMotr0";
            this.pnMotr0.Size = new System.Drawing.Size(977, 205);
            this.pnMotr0.TabIndex = 0;
            // 
            // lbAxisNo
            // 
            this.lbAxisNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAxisNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbAxisNo.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAxisNo.ForeColor = System.Drawing.Color.Red;
            this.lbAxisNo.Location = new System.Drawing.Point(0, 0);
            this.lbAxisNo.Name = "lbAxisNo";
            this.lbAxisNo.Size = new System.Drawing.Size(72, 25);
            this.lbAxisNo.TabIndex = 5;
            this.lbAxisNo.Text = "Axis 13";
            this.lbAxisNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCmdPos
            // 
            this.lbCmdPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCmdPos.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbCmdPos.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCmdPos.ForeColor = System.Drawing.Color.Blue;
            this.lbCmdPos.Location = new System.Drawing.Point(302, 0);
            this.lbCmdPos.Name = "lbCmdPos";
            this.lbCmdPos.Size = new System.Drawing.Size(152, 25);
            this.lbCmdPos.TabIndex = 6;
            this.lbCmdPos.Text = "141.243";
            this.lbCmdPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat1
            // 
            this.lbStat1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat1.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat1.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat1.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat1.Location = new System.Drawing.Point(454, 0);
            this.lbStat1.Name = "lbStat1";
            this.lbStat1.Size = new System.Drawing.Size(30, 25);
            this.lbStat1.TabIndex = 7;
            this.lbStat1.Text = "-L";
            this.lbStat1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat2
            // 
            this.lbStat2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat2.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat2.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat2.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat2.Location = new System.Drawing.Point(484, 0);
            this.lbStat2.Name = "lbStat2";
            this.lbStat2.Size = new System.Drawing.Size(30, 25);
            this.lbStat2.TabIndex = 8;
            this.lbStat2.Text = "O";
            this.lbStat2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat3
            // 
            this.lbStat3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat3.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat3.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat3.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat3.Location = new System.Drawing.Point(514, 0);
            this.lbStat3.Name = "lbStat3";
            this.lbStat3.Size = new System.Drawing.Size(30, 25);
            this.lbStat3.TabIndex = 9;
            this.lbStat3.Text = "+L";
            this.lbStat3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat4
            // 
            this.lbStat4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat4.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat4.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat4.ForeColor = System.Drawing.Color.Red;
            this.lbStat4.Location = new System.Drawing.Point(544, 0);
            this.lbStat4.Name = "lbStat4";
            this.lbStat4.Size = new System.Drawing.Size(30, 25);
            this.lbStat4.TabIndex = 10;
            this.lbStat4.Text = "H";
            this.lbStat4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat5
            // 
            this.lbStat5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat5.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat5.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat5.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat5.Location = new System.Drawing.Point(574, 0);
            this.lbStat5.Name = "lbStat5";
            this.lbStat5.Size = new System.Drawing.Size(40, 25);
            this.lbStat5.TabIndex = 11;
            this.lbStat5.Text = "ALM";
            this.lbStat5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat7
            // 
            this.lbStat7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat7.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat7.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat7.ForeColor = System.Drawing.Color.Red;
            this.lbStat7.Location = new System.Drawing.Point(654, 0);
            this.lbStat7.Name = "lbStat7";
            this.lbStat7.Size = new System.Drawing.Size(40, 25);
            this.lbStat7.TabIndex = 13;
            this.lbStat7.Text = "INP";
            this.lbStat7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStat8
            // 
            this.lbStat8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStat8.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat8.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat8.ForeColor = System.Drawing.Color.Red;
            this.lbStat8.Location = new System.Drawing.Point(694, 0);
            this.lbStat8.Name = "lbStat8";
            this.lbStat8.Size = new System.Drawing.Size(40, 25);
            this.lbStat8.TabIndex = 15;
            this.lbStat8.Text = "SVR";
            this.lbStat8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAxisName
            // 
            this.lbAxisName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAxisName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbAxisName.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAxisName.ForeColor = System.Drawing.Color.Blue;
            this.lbAxisName.Location = new System.Drawing.Point(72, 0);
            this.lbAxisName.Name = "lbAxisName";
            this.lbAxisName.Size = new System.Drawing.Size(230, 25);
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
            this.lbStat6.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbStat6.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStat6.ForeColor = System.Drawing.Color.DarkGray;
            this.lbStat6.Location = new System.Drawing.Point(614, 0);
            this.lbStat6.Name = "lbStat6";
            this.lbStat6.Size = new System.Drawing.Size(40, 25);
            this.lbStat6.TabIndex = 12;
            this.lbStat6.Text = "STP";
            this.lbStat6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btServoOn
            // 
            this.btServoOn.Dock = System.Windows.Forms.DockStyle.Left;
            this.btServoOn.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btServoOn.Location = new System.Drawing.Point(734, 0);
            this.btServoOn.Name = "btServoOn";
            this.btServoOn.Size = new System.Drawing.Size(81, 25);
            this.btServoOn.TabIndex = 0;
            this.btServoOn.Text = "Servo On";
            this.btServoOn.UseVisualStyleBackColor = true;
            this.btServoOn.Click += new System.EventHandler(this.btServoOn_Click);
            // 
            // btServoOff
            // 
            this.btServoOff.Dock = System.Windows.Forms.DockStyle.Left;
            this.btServoOff.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btServoOff.Location = new System.Drawing.Point(815, 0);
            this.btServoOff.Name = "btServoOff";
            this.btServoOff.Size = new System.Drawing.Size(81, 25);
            this.btServoOff.TabIndex = 17;
            this.btServoOff.Text = "Servo Off";
            this.btServoOff.UseVisualStyleBackColor = true;
            this.btServoOff.Click += new System.EventHandler(this.btServoOff_Click);
            // 
            // btHome
            // 
            this.btHome.Dock = System.Windows.Forms.DockStyle.Left;
            this.btHome.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btHome.Location = new System.Drawing.Point(896, 0);
            this.btHome.Name = "btHome";
            this.btHome.Size = new System.Drawing.Size(81, 25);
            this.btHome.TabIndex = 18;
            this.btHome.Text = "Home";
            this.btHome.UseVisualStyleBackColor = true;
            this.btHome.Click += new System.EventHandler(this.btHome_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btHome);
            this.panel1.Controls.Add(this.btServoOff);
            this.panel1.Controls.Add(this.btServoOn);
            this.panel1.Controls.Add(this.lbStat8);
            this.panel1.Controls.Add(this.lbStat7);
            this.panel1.Controls.Add(this.lbStat6);
            this.panel1.Controls.Add(this.lbStat5);
            this.panel1.Controls.Add(this.lbStat4);
            this.panel1.Controls.Add(this.lbStat3);
            this.panel1.Controls.Add(this.lbStat2);
            this.panel1.Controls.Add(this.lbStat1);
            this.panel1.Controls.Add(this.lbCmdPos);
            this.panel1.Controls.Add(this.lbAxisName);
            this.panel1.Controls.Add(this.lbAxisNo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(977, 25);
            this.panel1.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(977, 230);
            this.panel2.TabIndex = 20;
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.Controls.Add(this.pnMotr0);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 25);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(977, 205);
            this.panel3.TabIndex = 0;
            // 
            // FrameMotrPosAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(20, 20);
            this.AutoScrollMinSize = new System.Drawing.Size(20, 20);
            this.ClientSize = new System.Drawing.Size(977, 230);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameMotrPosAPT";
            this.Text = "FrameMotrPosAPT";
            this.Shown += new System.EventHandler(this.FrameMotrPosAPT_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrameMotrPosAPT_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}