namespace Machine
{
    partial class FormVer
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
            this.tbVer = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // tbVer
            // 
            this.tbVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVer.Location = new System.Drawing.Point(0, 0);
            this.tbVer.Name = "tbVer";
            this.tbVer.Size = new System.Drawing.Size(844, 506);
            this.tbVer.TabIndex = 1;
            this.tbVer.Text = "";
            this.tbVer.VisibleChanged += new System.EventHandler(this.richTextBox1_VisibleChanged);
            // 
            // FormVer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 506);
            this.Controls.Add(this.tbVer);
            this.Name = "FormVer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormVer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbVer;
    }
}