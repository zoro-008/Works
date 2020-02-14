namespace Machine
{
    partial class FrameCylinderAPT
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
            this.lbCylName = new System.Windows.Forms.Label();
            this.btBwd = new System.Windows.Forms.Button();
            this.btFwd = new System.Windows.Forms.Button();
            this.btRepeat = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lbCylNo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCylName
            // 
            this.lbCylName.BackColor = System.Drawing.Color.DimGray;
            this.lbCylName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCylName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCylName.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylName.ForeColor = System.Drawing.Color.White;
            this.lbCylName.Location = new System.Drawing.Point(28, 0);
            this.lbCylName.Name = "lbCylName";
            this.lbCylName.Size = new System.Drawing.Size(212, 23);
            this.lbCylName.TabIndex = 1;
            this.lbCylName.Text = "Pusher";
            this.lbCylName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btBwd
            // 
            this.btBwd.BackColor = System.Drawing.SystemColors.Control;
            this.btBwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btBwd.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btBwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btBwd.Location = new System.Drawing.Point(0, 0);
            this.btBwd.Name = "btBwd";
            this.btBwd.Padding = new System.Windows.Forms.Padding(1);
            this.btBwd.Size = new System.Drawing.Size(95, 88);
            this.btBwd.TabIndex = 2;
            this.btBwd.Text = "BWD";
            this.btBwd.UseVisualStyleBackColor = true;
            // 
            // btFwd
            // 
            this.btFwd.BackColor = System.Drawing.SystemColors.Control;
            this.btFwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btFwd.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btFwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btFwd.Location = new System.Drawing.Point(95, 0);
            this.btFwd.Name = "btFwd";
            this.btFwd.Size = new System.Drawing.Size(95, 88);
            this.btFwd.TabIndex = 3;
            this.btFwd.Text = "FWD";
            this.btFwd.UseVisualStyleBackColor = true;
            this.btFwd.Click += new System.EventHandler(this.btFwd_Click);
            // 
            // btRepeat
            // 
            this.btRepeat.BackColor = System.Drawing.SystemColors.Control;
            this.btRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btRepeat.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btRepeat.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btRepeat.Location = new System.Drawing.Point(190, 0);
            this.btRepeat.Name = "btRepeat";
            this.btRepeat.Size = new System.Drawing.Size(85, 88);
            this.btRepeat.TabIndex = 4;
            this.btRepeat.Text = "REPEAT";
            this.btRepeat.UseVisualStyleBackColor = true;
            this.btRepeat.Click += new System.EventHandler(this.btRepeat_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbCylName);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.lbCylNo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(275, 23);
            this.panel1.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.textBox1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.textBox1.Location = new System.Drawing.Point(240, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(35, 23);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "1000";
            // 
            // lbCylNo
            // 
            this.lbCylNo.BackColor = System.Drawing.SystemColors.Control;
            this.lbCylNo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCylNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbCylNo.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylNo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lbCylNo.Location = new System.Drawing.Point(0, 0);
            this.lbCylNo.Name = "lbCylNo";
            this.lbCylNo.Size = new System.Drawing.Size(28, 23);
            this.lbCylNo.TabIndex = 0;
            this.lbCylNo.Text = "99";
            this.lbCylNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btRepeat);
            this.panel2.Controls.Add(this.btFwd);
            this.panel2.Controls.Add(this.btBwd);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 23);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(275, 88);
            this.panel2.TabIndex = 8;
            // 
            // FrameCylinderAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(275, 111);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameCylinderAPT";
            this.Text = "FrameCylinderAPT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrameCylinderAPT_FormClosing);
            this.Shown += new System.EventHandler(this.FrameCylinderAPT_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrameCylinderAPT_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbCylName;
        private System.Windows.Forms.Button btBwd;
        private System.Windows.Forms.Button btFwd;
        private System.Windows.Forms.Button btRepeat;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbCylNo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
    }
}