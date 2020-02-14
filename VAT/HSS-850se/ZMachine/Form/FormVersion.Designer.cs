namespace Machine
{
    partial class FormVersion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVersion));
            this.tbVer = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // tbVer
            // 
            this.tbVer.BackColor = System.Drawing.SystemColors.Control;
            this.tbVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVer.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbVer.Location = new System.Drawing.Point(0, 0);
            this.tbVer.Name = "tbVer";
            this.tbVer.ReadOnly = true;
            this.tbVer.Size = new System.Drawing.Size(900, 625);
            this.tbVer.TabIndex = 1;
            this.tbVer.Text = resources.GetString("tbVer.Text");
            this.tbVer.TextChanged += new System.EventHandler(this.tbVer_TextChanged);
            // 
            // FormVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 625);
            this.Controls.Add(this.tbVer);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormVersion";
            this.ShowIcon = false;
            this.Text = "VERSION INFOMATION";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbVer;
    }
}