namespace Machine
{
    partial class FormLotOpen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLotOpen));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbEmployeeID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbLotNo = new System.Windows.Forms.TextBox();
            this.tbSelDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btLotOpen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbEmployeeID);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbLotNo);
            this.panel1.Controls.Add(this.tbSelDevice);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(5, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 104);
            this.panel1.TabIndex = 1;
            // 
            // tbEmployeeID
            // 
            this.tbEmployeeID.BackColor = System.Drawing.SystemColors.Window;
            this.tbEmployeeID.Enabled = false;
            this.tbEmployeeID.Location = new System.Drawing.Point(139, 38);
            this.tbEmployeeID.Name = "tbEmployeeID";
            this.tbEmployeeID.Size = new System.Drawing.Size(267, 26);
            this.tbEmployeeID.TabIndex = 1;
            this.tbEmployeeID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbEmployeeID_KeyDown);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(7, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Employee ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLotNo
            // 
            this.tbLotNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbLotNo.Location = new System.Drawing.Point(139, 69);
            this.tbLotNo.Name = "tbLotNo";
            this.tbLotNo.Size = new System.Drawing.Size(267, 26);
            this.tbLotNo.TabIndex = 3;
            this.tbLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotNo_KeyDown);
            // 
            // tbSelDevice
            // 
            this.tbSelDevice.BackColor = System.Drawing.Color.LightGray;
            this.tbSelDevice.Location = new System.Drawing.Point(139, 7);
            this.tbSelDevice.Name = "tbSelDevice";
            this.tbSelDevice.Size = new System.Drawing.Size(267, 26);
            this.tbSelDevice.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(7, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 27);
            this.label3.TabIndex = 2;
            this.label3.Text = "LOT No.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(7, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "DEVICE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btLotOpen
            // 
            this.btLotOpen.Image = ((System.Drawing.Image)(resources.GetObject("btLotOpen.Image")));
            this.btLotOpen.Location = new System.Drawing.Point(5, 149);
            this.btLotOpen.Name = "btLotOpen";
            this.btLotOpen.Size = new System.Drawing.Size(198, 50);
            this.btLotOpen.TabIndex = 9;
            this.btLotOpen.Text = "LOT OPEN";
            this.btLotOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLotOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLotOpen.UseVisualStyleBackColor = true;
            this.btLotOpen.Click += new System.EventHandler(this.btLotOpen_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(417, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "LOT OPEN";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            this.btCancel.Image = ((System.Drawing.Image)(resources.GetObject("btCancel.Image")));
            this.btCancel.Location = new System.Drawing.Point(209, 149);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(198, 50);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "CLOSE";
            this.btCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // FormLotOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(417, 206);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btLotOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLotOpen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormLotOpen_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btLotOpen;
        private System.Windows.Forms.TextBox tbLotNo;
        private System.Windows.Forms.TextBox tbSelDevice;
        private System.Windows.Forms.TextBox tbEmployeeID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
    }
}