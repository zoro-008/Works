namespace COMMON
{
    partial class FormOk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOk));
            this.lbMsg = new System.Windows.Forms.Label();
            this.lbCaption = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btOk = new System.Windows.Forms.Button();
            this.tmHide = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMsg
            // 
            this.lbMsg.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.lbMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMsg.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.lbCaption.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            // btOk
            // 
            this.btOk.BackColor = System.Drawing.Color.Transparent;
            this.btOk.FlatAppearance.BorderSize = 2;
            this.btOk.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btOk.ImageKey = "(없음)";
            this.btOk.Location = new System.Drawing.Point(562, 150);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(194, 44);
            this.btOk.TabIndex = 8;
            this.btOk.Text = "      OK";
            this.btOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOk.UseVisualStyleBackColor = false;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // tmHide
            // 
            this.tmHide.Tick += new System.EventHandler(this.tmHide_Tick);
            // 
            // FormOk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(764, 202);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbCaption);
            this.Controls.Add(this.lbMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOk";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormOk";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.Label lbCaption;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Timer tmHide;

    }
}