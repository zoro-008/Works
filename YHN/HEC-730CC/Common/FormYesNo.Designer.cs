namespace COMMON
{
    partial class FormYesNo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormYesNo));
            this.lbMsg = new System.Windows.Forms.Label();
            this.lbCaption = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btYes = new System.Windows.Forms.Button();
            this.btNo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMsg
            // 
            this.lbMsg.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.lbMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMsg.Font = new System.Drawing.Font("맑은 고딕 Semilight", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbMsg.Location = new System.Drawing.Point(0, 0);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.Size = new System.Drawing.Size(764, 202);
            this.lbMsg.TabIndex = 5;
            this.lbMsg.Text = "CAPTION\r\n\r\n";
            this.lbMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCaption
            // 
            this.lbCaption.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.lbCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbCaption.Font = new System.Drawing.Font("맑은 고딕 Semilight", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCaption.Image = ((System.Drawing.Image)(resources.GetObject("lbCaption.Image")));
            this.lbCaption.Location = new System.Drawing.Point(0, 0);
            this.lbCaption.Name = "lbCaption";
            this.lbCaption.Size = new System.Drawing.Size(764, 35);
            this.lbCaption.TabIndex = 6;
            this.lbCaption.Text = "CAPTION";
            this.lbCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 28);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // btYes
            // 
            this.btYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btYes.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btYes.Image = ((System.Drawing.Image)(resources.GetObject("btYes.Image")));
            this.btYes.Location = new System.Drawing.Point(349, 146);
            this.btYes.Name = "btYes";
            this.btYes.Size = new System.Drawing.Size(194, 44);
            this.btYes.TabIndex = 2;
            this.btYes.Text = "     YES";
            this.btYes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btYes.UseVisualStyleBackColor = true;
            this.btYes.Click += new System.EventHandler(this.btYes_Click_1);
            // 
            // btNo
            // 
            this.btNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btNo.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btNo.Image = ((System.Drawing.Image)(resources.GetObject("btNo.Image")));
            this.btNo.Location = new System.Drawing.Point(559, 146);
            this.btNo.Name = "btNo";
            this.btNo.Size = new System.Drawing.Size(194, 44);
            this.btNo.TabIndex = 1;
            this.btNo.Text = "      NO";
            this.btNo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btNo.UseVisualStyleBackColor = true;
            // 
            // FormYesNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(764, 202);
            this.Controls.Add(this.btNo);
            this.Controls.Add(this.btYes);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbCaption);
            this.Controls.Add(this.lbMsg);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormYesNo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormYesNo";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.Label lbCaption;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btYes;
        private System.Windows.Forms.Button btNo;
    }
}