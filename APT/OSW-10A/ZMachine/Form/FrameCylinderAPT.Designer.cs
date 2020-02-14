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
            this.lbCylNo = new System.Windows.Forms.Label();
            this.lbCylName = new System.Windows.Forms.Label();
            this.btFwd = new System.Windows.Forms.Button();
            this.btBwd = new System.Windows.Forms.Button();
            this.btRepeat = new System.Windows.Forms.Button();
            this.lbBwd = new System.Windows.Forms.Label();
            this.lbFwd = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbCylNo
            // 
            this.lbCylNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCylNo.Font = new System.Drawing.Font("돋움체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylNo.Location = new System.Drawing.Point(0, 0);
            this.lbCylNo.Name = "lbCylNo";
            this.lbCylNo.Size = new System.Drawing.Size(22, 15);
            this.lbCylNo.TabIndex = 0;
            this.lbCylNo.Text = "0";
            this.lbCylNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCylName
            // 
            this.lbCylName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCylName.Font = new System.Drawing.Font("돋움체", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCylName.ForeColor = System.Drawing.Color.Blue;
            this.lbCylName.Location = new System.Drawing.Point(26, 0);
            this.lbCylName.Name = "lbCylName";
            this.lbCylName.Size = new System.Drawing.Size(165, 15);
            this.lbCylName.TabIndex = 1;
            this.lbCylName.Text = "Pusher";
            this.lbCylName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btFwd
            // 
            this.btFwd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btFwd.Font = new System.Drawing.Font("돋움체", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btFwd.ForeColor = System.Drawing.Color.Blue;
            this.btFwd.Location = new System.Drawing.Point(0, 17);
            this.btFwd.Name = "btFwd";
            this.btFwd.Size = new System.Drawing.Size(42, 41);
            this.btFwd.TabIndex = 2;
            this.btFwd.Text = "FWD";
            this.btFwd.UseVisualStyleBackColor = true;
            this.btFwd.Click += new System.EventHandler(this.btFwd_Click);
            // 
            // btBwd
            // 
            this.btBwd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBwd.Font = new System.Drawing.Font("돋움체", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btBwd.ForeColor = System.Drawing.Color.Blue;
            this.btBwd.Location = new System.Drawing.Point(106, 17);
            this.btBwd.Name = "btBwd";
            this.btBwd.Size = new System.Drawing.Size(42, 41);
            this.btBwd.TabIndex = 3;
            this.btBwd.Text = "BWD";
            this.btBwd.UseVisualStyleBackColor = true;
            this.btBwd.Click += new System.EventHandler(this.btBwd_Click);
            // 
            // btRepeat
            // 
            this.btRepeat.BackColor = System.Drawing.Color.SeaShell;
            this.btRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRepeat.Font = new System.Drawing.Font("돋움체", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btRepeat.ForeColor = System.Drawing.Color.Blue;
            this.btRepeat.Location = new System.Drawing.Point(149, 17);
            this.btRepeat.Name = "btRepeat";
            this.btRepeat.Size = new System.Drawing.Size(42, 41);
            this.btRepeat.TabIndex = 4;
            this.btRepeat.Text = "Repeat";
            this.btRepeat.UseVisualStyleBackColor = false;
            this.btRepeat.Click += new System.EventHandler(this.btRepeat_Click);
            // 
            // lbBwd
            // 
            this.lbBwd.BackColor = System.Drawing.Color.ForestGreen;
            this.lbBwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbBwd.Font = new System.Drawing.Font("돋움체", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbBwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbBwd.Location = new System.Drawing.Point(43, 17);
            this.lbBwd.Name = "lbBwd";
            this.lbBwd.Size = new System.Drawing.Size(30, 41);
            this.lbBwd.TabIndex = 5;
            this.lbBwd.Text = "BWD";
            this.lbBwd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFwd
            // 
            this.lbFwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFwd.Font = new System.Drawing.Font("돋움체", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFwd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbFwd.Location = new System.Drawing.Point(74, 17);
            this.lbFwd.Name = "lbFwd";
            this.lbFwd.Size = new System.Drawing.Size(30, 41);
            this.lbFwd.TabIndex = 6;
            this.lbFwd.Text = "FWD";
            this.lbFwd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FrameCylinderAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(191, 58);
            this.Controls.Add(this.lbFwd);
            this.Controls.Add(this.lbBwd);
            this.Controls.Add(this.btRepeat);
            this.Controls.Add(this.btBwd);
            this.Controls.Add(this.btFwd);
            this.Controls.Add(this.lbCylName);
            this.Controls.Add(this.lbCylNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameCylinderAPT";
            this.Text = "FrameCylinderAPT";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbCylNo;
        private System.Windows.Forms.Label lbCylName;
        private System.Windows.Forms.Button btFwd;
        private System.Windows.Forms.Button btBwd;
        private System.Windows.Forms.Button btRepeat;
        private System.Windows.Forms.Label lbBwd;
        private System.Windows.Forms.Label lbFwd;
        private System.Windows.Forms.Timer tmUpdate;
    }
}