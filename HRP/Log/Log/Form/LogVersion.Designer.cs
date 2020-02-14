namespace Log
{
    partial class LogVersion
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
            this.lbDate = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbDate
            // 
            this.lbDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDate.Location = new System.Drawing.Point(12, 40);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(220, 27);
            this.lbDate.TabIndex = 4;
            this.lbDate.Text = "2016 06-22 04: 57:22";
            // 
            // lbVer
            // 
            this.lbVer.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbVer.Location = new System.Drawing.Point(12, 9);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(157, 23);
            this.lbVer.TabIndex = 3;
            this.lbVer.Text = "Version 1.1.1.1";
            // 
            // LogVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 77);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVer);
            this.Name = "LogVersion";
            this.Text = "dwKim";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbVer;
    }
}