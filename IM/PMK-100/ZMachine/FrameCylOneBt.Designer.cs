namespace Machine
{
    partial class FraCylOneBt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FraCylOneBt));
            this.lbTitle = new System.Windows.Forms.Label();
            this.btAction = new System.Windows.Forms.Button();
            this.imDirection = new System.Windows.Forms.ImageList(this.components);
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.lbTitle.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.Location = new System.Drawing.Point(1, 1);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(99, 40);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "CYLINDER NAME";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btAction
            // 
            this.btAction.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btAction.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btAction.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btAction.ImageList = this.imDirection;
            this.btAction.Location = new System.Drawing.Point(-1, 41);
            this.btAction.Margin = new System.Windows.Forms.Padding(0);
            this.btAction.Name = "btAction";
            this.btAction.Size = new System.Drawing.Size(103, 85);
            this.btAction.TabIndex = 5;
            this.btAction.Text = "LEFT";
            this.btAction.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btAction.UseVisualStyleBackColor = true;
            this.btAction.Click += new System.EventHandler(this.btAction_Click);
            // 
            // imDirection
            // 
            this.imDirection.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imDirection.ImageStream")));
            this.imDirection.TransparentColor = System.Drawing.Color.Transparent;
            this.imDirection.Images.SetKeyName(0, "UP.bmp");
            this.imDirection.Images.SetKeyName(1, "DN.bmp");
            this.imDirection.Images.SetKeyName(2, "LEFT.bmp");
            this.imDirection.Images.SetKeyName(3, "RIGHT.bmp");
            this.imDirection.Images.SetKeyName(4, "CW.bmp");
            this.imDirection.Images.SetKeyName(5, "CCW.bmp");
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FraCylOneBt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(103, 127);
            this.Controls.Add(this.btAction);
            this.Controls.Add(this.lbTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FraCylOneBt";
            this.Text = "FraOutput";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FraCylOneBt_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btAction;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.ImageList imDirection;
    }
}