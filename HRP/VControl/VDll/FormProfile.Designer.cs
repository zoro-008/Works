namespace VDll
{
    partial class FormProfile
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbHor = new System.Windows.Forms.CheckBox();
            this.cbVer = new System.Windows.Forms.CheckBox();
            this.cbDif = new System.Windows.Forms.CheckBox();
            this.cbAvr = new System.Windows.Forms.CheckBox();
            this.tbAvrRange = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRaw = new System.Windows.Forms.CheckBox();
            this.ibProfile = new Emgu.CV.UI.ImageBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibProfile)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbHor);
            this.panel1.Controls.Add(this.cbVer);
            this.panel1.Controls.Add(this.cbDif);
            this.panel1.Controls.Add(this.cbAvr);
            this.panel1.Controls.Add(this.tbAvrRange);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbRaw);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 25);
            this.panel1.TabIndex = 0;
            // 
            // cbHor
            // 
            this.cbHor.AutoSize = true;
            this.cbHor.Location = new System.Drawing.Point(285, 4);
            this.cbHor.Name = "cbHor";
            this.cbHor.Size = new System.Drawing.Size(43, 16);
            this.cbHor.TabIndex = 6;
            this.cbHor.Text = "Hor";
            this.cbHor.UseVisualStyleBackColor = true;
            this.cbHor.CheckedChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // cbVer
            // 
            this.cbVer.AutoSize = true;
            this.cbVer.Location = new System.Drawing.Point(237, 4);
            this.cbVer.Name = "cbVer";
            this.cbVer.Size = new System.Drawing.Size(43, 16);
            this.cbVer.TabIndex = 5;
            this.cbVer.Text = "Ver";
            this.cbVer.UseVisualStyleBackColor = true;
            this.cbVer.CheckedChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // cbDif
            // 
            this.cbDif.AutoSize = true;
            this.cbDif.Location = new System.Drawing.Point(194, 4);
            this.cbDif.Name = "cbDif";
            this.cbDif.Size = new System.Drawing.Size(38, 16);
            this.cbDif.TabIndex = 4;
            this.cbDif.Text = "Dif";
            this.cbDif.UseVisualStyleBackColor = true;
            this.cbDif.CheckedChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // cbAvr
            // 
            this.cbAvr.AutoSize = true;
            this.cbAvr.Location = new System.Drawing.Point(147, 4);
            this.cbAvr.Name = "cbAvr";
            this.cbAvr.Size = new System.Drawing.Size(42, 16);
            this.cbAvr.TabIndex = 3;
            this.cbAvr.Text = "Avr";
            this.cbAvr.UseVisualStyleBackColor = true;
            this.cbAvr.CheckedChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // tbAvrRange
            // 
            this.tbAvrRange.Location = new System.Drawing.Point(63, 1);
            this.tbAvrRange.Name = "tbAvrRange";
            this.tbAvrRange.Size = new System.Drawing.Size(20, 21);
            this.tbAvrRange.TabIndex = 2;
            this.tbAvrRange.TextChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "AvrRange";
            // 
            // cbRaw
            // 
            this.cbRaw.AutoSize = true;
            this.cbRaw.Location = new System.Drawing.Point(93, 4);
            this.cbRaw.Name = "cbRaw";
            this.cbRaw.Size = new System.Drawing.Size(49, 16);
            this.cbRaw.TabIndex = 0;
            this.cbRaw.Text = "Raw";
            this.cbRaw.UseVisualStyleBackColor = true;
            this.cbRaw.CheckedChanged += new System.EventHandler(this.cbRaw_CheckedChanged);
            // 
            // ibProfile
            // 
            this.ibProfile.BackColor = System.Drawing.Color.DimGray;
            this.ibProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibProfile.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ibProfile.Location = new System.Drawing.Point(0, 25);
            this.ibProfile.Name = "ibProfile";
            this.ibProfile.Size = new System.Drawing.Size(786, 518);
            this.ibProfile.TabIndex = 5;
            this.ibProfile.TabStop = false;
            this.ibProfile.Paint += new System.Windows.Forms.PaintEventHandler(this.ibProfile_Paint);
            this.ibProfile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibProfile_MouseDown);
            this.ibProfile.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibProfile_MouseMove);
            this.ibProfile.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibProfile_MouseUp);
            // 
            // FormProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 543);
            this.Controls.Add(this.ibProfile);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "FormProfile";
            this.Text = "FormProfile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProfile_FormClosing);
            this.Move += new System.EventHandler(this.FormProfile_Move);
            this.Resize += new System.EventHandler(this.FormProfile_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibProfile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbHor;
        private System.Windows.Forms.CheckBox cbVer;
        private System.Windows.Forms.CheckBox cbDif;
        private System.Windows.Forms.CheckBox cbAvr;
        private System.Windows.Forms.TextBox tbAvrRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbRaw;
        private Emgu.CV.UI.ImageBox ibProfile;
    }
}