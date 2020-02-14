namespace Machine
{
    partial class FormOkNg
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
            this.lbOkNg = new System.Windows.Forms.Label();
            this.lbText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbOkNg
            // 
            this.lbOkNg.BackColor = System.Drawing.Color.Lime;
            this.lbOkNg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOkNg.Font = new System.Drawing.Font("Malgun Gothic", 45F);
            this.lbOkNg.Location = new System.Drawing.Point(0, 0);
            this.lbOkNg.Name = "lbOkNg";
            this.lbOkNg.Size = new System.Drawing.Size(334, 151);
            this.lbOkNg.TabIndex = 0;
            this.lbOkNg.Text = "OK";
            this.lbOkNg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbText
            // 
            this.lbText.AutoSize = true;
            this.lbText.BackColor = System.Drawing.Color.Transparent;
            this.lbText.Font = new System.Drawing.Font("Malgun Gothic", 9F);
            this.lbText.Location = new System.Drawing.Point(12, 130);
            this.lbText.Name = "lbText";
            this.lbText.Size = new System.Drawing.Size(135, 15);
            this.lbText.TabIndex = 1;
            this.lbText.Text = "(최소 토크값 도달 못함)";
            // 
            // FormOkNg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(334, 151);
            this.Controls.Add(this.lbText);
            this.Controls.Add(this.lbOkNg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 130);
            this.Name = "FormOkNg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormOkNg";
            this.VisibleChanged += new System.EventHandler(this.FormOkNg_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbOkNg;
        private System.Windows.Forms.Label lbText;
    }
}