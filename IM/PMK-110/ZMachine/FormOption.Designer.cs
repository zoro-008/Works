namespace Machine
{
    partial class FormOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbBarSkip = new System.Windows.Forms.CheckBox();
            this.cbMarkSkip = new System.Windows.Forms.CheckBox();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbVer = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbBarSkip);
            this.panel1.Controls.Add(this.cbMarkSkip);
            this.panel1.Controls.Add(this.cbIgnrDoor);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 238);
            this.panel1.TabIndex = 0;
            // 
            // cbBarSkip
            // 
            this.cbBarSkip.AutoSize = true;
            this.cbBarSkip.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbBarSkip.Location = new System.Drawing.Point(10, 126);
            this.cbBarSkip.Name = "cbBarSkip";
            this.cbBarSkip.Size = new System.Drawing.Size(206, 20);
            this.cbBarSkip.TabIndex = 4;
            this.cbBarSkip.Text = "Skip Barcode Reading";
            this.cbBarSkip.UseVisualStyleBackColor = true;
            // 
            // cbMarkSkip
            // 
            this.cbMarkSkip.AutoSize = true;
            this.cbMarkSkip.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbMarkSkip.Location = new System.Drawing.Point(10, 100);
            this.cbMarkSkip.Name = "cbMarkSkip";
            this.cbMarkSkip.Size = new System.Drawing.Size(130, 20);
            this.cbMarkSkip.TabIndex = 2;
            this.cbMarkSkip.Text = "Skip Marking";
            this.cbMarkSkip.UseVisualStyleBackColor = true;
            // 
            // cbIgnrDoor
            // 
            this.cbIgnrDoor.AutoSize = true;
            this.cbIgnrDoor.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbIgnrDoor.Location = new System.Drawing.Point(10, 48);
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.Size = new System.Drawing.Size(172, 20);
            this.cbIgnrDoor.TabIndex = 1;
            this.cbIgnrDoor.Text = "Ignore Door Open";
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(-1, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(414, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "OPTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVer
            // 
            this.lbVer.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbVer.Location = new System.Drawing.Point(713, 786);
            this.lbVer.Name = "lbVer";
            this.lbVer.Size = new System.Drawing.Size(157, 23);
            this.lbVer.TabIndex = 1;
            this.lbVer.Text = "VER : 1.1.1.1";
            // 
            // lbDate
            // 
            this.lbDate.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDate.Location = new System.Drawing.Point(714, 816);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(220, 16);
            this.lbDate.TabIndex = 2;
            this.lbDate.Text = "2016_06_22_PM 4: 57";
            // 
            // btSave
            // 
            this.btSave.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSave.Image = ((System.Drawing.Image)(resources.GetObject("btSave.Image")));
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSave.Location = new System.Drawing.Point(952, 769);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(305, 73);
            this.btSave.TabIndex = 3;
            this.btSave.Text = "SAVE";
            this.btSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVer);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Text = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbVer;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbBarSkip;
        private System.Windows.Forms.CheckBox cbMarkSkip;
    }
}