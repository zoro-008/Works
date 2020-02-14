namespace Machine
{
    partial class FormVision
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
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.ibCam = new Emgu.CV.UI.ImageBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).BeginInit();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            // 
            // ibCam
            // 
            this.ibCam.BackColor = System.Drawing.Color.DimGray;
            this.ibCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibCam.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.PanAndZoom;
            this.ibCam.Location = new System.Drawing.Point(0, 0);
            this.ibCam.Name = "ibCam";
            this.ibCam.Size = new System.Drawing.Size(1068, 459);
            this.ibCam.TabIndex = 3;
            this.ibCam.TabStop = false;
            this.ibCam.Paint += new System.Windows.Forms.PaintEventHandler(this.ibCam_Paint);
            this.ibCam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseDown);
            this.ibCam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseMove);
            this.ibCam.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibCam_MouseUp);
            this.ibCam.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ibCam_PreviewKeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(30, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 54);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 95);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 56);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // FormVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 459);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ibCam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormVision";
            this.Text = "FormVision";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVision_FormClosing);
            this.Shown += new System.EventHandler(this.FormVision_Shown);
            this.Resize += new System.EventHandler(this.FormVision_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ibCam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmUpdate;
        private Emgu.CV.UI.ImageBox ibCam;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}