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
            this.lbFwd = new System.Windows.Forms.Label();
            this.lbBwd = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbCylNo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCylName
            // 
            this.lbCylName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCylName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCylName.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylName.ForeColor = System.Drawing.Color.Blue;
            this.lbCylName.Location = new System.Drawing.Point(22, 0);
            this.lbCylName.Name = "lbCylName";
            this.lbCylName.Size = new System.Drawing.Size(218, 20);
            this.lbCylName.TabIndex = 1;
            this.lbCylName.Text = "Pusher";
            this.lbCylName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btBwd
            // 
            this.btBwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btBwd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBwd.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btBwd.ForeColor = System.Drawing.Color.Blue;
            this.btBwd.Location = new System.Drawing.Point(0, 0);
            this.btBwd.Name = "btBwd";
            this.btBwd.Size = new System.Drawing.Size(70, 55);
            this.btBwd.TabIndex = 2;
            this.btBwd.Text = "BWD";
            this.btBwd.UseVisualStyleBackColor = true;
            // 
            // btFwd
            // 
            this.btFwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btFwd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btFwd.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btFwd.ForeColor = System.Drawing.Color.Blue;
            this.btFwd.Location = new System.Drawing.Point(100, 0);
            this.btFwd.Name = "btFwd";
            this.btFwd.Size = new System.Drawing.Size(70, 55);
            this.btFwd.TabIndex = 3;
            this.btFwd.Text = "FWD";
            this.btFwd.UseVisualStyleBackColor = true;
            // 
            // btRepeat
            // 
            this.btRepeat.BackColor = System.Drawing.Color.SeaShell;
            this.btRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRepeat.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btRepeat.ForeColor = System.Drawing.Color.Blue;
            this.btRepeat.Location = new System.Drawing.Point(200, 0);
            this.btRepeat.Name = "btRepeat";
            this.btRepeat.Size = new System.Drawing.Size(40, 55);
            this.btRepeat.TabIndex = 4;
            this.btRepeat.Text = "REPEAT";
            this.btRepeat.UseVisualStyleBackColor = false;
            this.btRepeat.Click += new System.EventHandler(this.btRepeat_Click);
            // 
            // lbFwd
            // 
            this.lbFwd.BackColor = System.Drawing.Color.ForestGreen;
            this.lbFwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbFwd.Font = new System.Drawing.Font("맑은 고딕", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbFwd.Location = new System.Drawing.Point(170, 0);
            this.lbFwd.Name = "lbFwd";
            this.lbFwd.Size = new System.Drawing.Size(30, 55);
            this.lbFwd.TabIndex = 5;
            this.lbFwd.Text = "FWD";
            this.lbFwd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbBwd
            // 
            this.lbBwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbBwd.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbBwd.Font = new System.Drawing.Font("맑은 고딕", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbBwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbBwd.Location = new System.Drawing.Point(70, 0);
            this.lbBwd.Name = "lbBwd";
            this.lbBwd.Size = new System.Drawing.Size(30, 55);
            this.lbBwd.TabIndex = 6;
            this.lbBwd.Text = "BWD";
            this.lbBwd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbCylName);
            this.panel1.Controls.Add(this.lbCylNo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 20);
            this.panel1.TabIndex = 7;
            // 
            // lbCylNo
            // 
            this.lbCylNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCylNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbCylNo.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylNo.Location = new System.Drawing.Point(0, 0);
            this.lbCylNo.Name = "lbCylNo";
            this.lbCylNo.Size = new System.Drawing.Size(22, 20);
            this.lbCylNo.TabIndex = 0;
            this.lbCylNo.Text = "0";
            this.lbCylNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btRepeat);
            this.panel2.Controls.Add(this.lbFwd);
            this.panel2.Controls.Add(this.btFwd);
            this.panel2.Controls.Add(this.lbBwd);
            this.panel2.Controls.Add(this.btBwd);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 55);
            this.panel2.TabIndex = 8;
            // 
            // FrameCylinderAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 75);
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
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbCylName;
        private System.Windows.Forms.Button btBwd;
        private System.Windows.Forms.Button btFwd;
        private System.Windows.Forms.Button btRepeat;
        private System.Windows.Forms.Label lbFwd;
        private System.Windows.Forms.Label lbBwd;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbCylNo;
        private System.Windows.Forms.Panel panel2;
    }
}