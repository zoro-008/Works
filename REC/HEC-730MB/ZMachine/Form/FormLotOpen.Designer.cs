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
            this.tbTargetBin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbMaterialNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLotNo = new System.Windows.Forms.TextBox();
            this.tbSelDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btLotOpen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.lvLot = new System.Windows.Forms.ListView();
            this.btDelete = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbLotNo);
            this.panel1.Controls.Add(this.tbSelDevice);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(5, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 79);
            this.panel1.TabIndex = 1;
            // 
            // tbEmployeeID
            // 
            this.tbEmployeeID.BackColor = System.Drawing.SystemColors.Window;
            this.tbEmployeeID.Enabled = false;
            this.tbEmployeeID.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbEmployeeID.Location = new System.Drawing.Point(565, 46);
            this.tbEmployeeID.Name = "tbEmployeeID";
            this.tbEmployeeID.Size = new System.Drawing.Size(267, 29);
            this.tbEmployeeID.TabIndex = 1;
            this.tbEmployeeID.Visible = false;
            this.tbEmployeeID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbEmployeeID_KeyDown);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label6.Location = new System.Drawing.Point(433, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Employee ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Visible = false;
            // 
            // tbTargetBin
            // 
            this.tbTargetBin.BackColor = System.Drawing.SystemColors.Window;
            this.tbTargetBin.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbTargetBin.Location = new System.Drawing.Point(565, 140);
            this.tbTargetBin.Name = "tbTargetBin";
            this.tbTargetBin.Size = new System.Drawing.Size(267, 29);
            this.tbTargetBin.TabIndex = 4;
            this.tbTargetBin.Visible = false;
            this.tbTargetBin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbTargetBin_KeyDown);
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label5.Location = new System.Drawing.Point(433, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 27);
            this.label5.TabIndex = 13;
            this.label5.Text = "Target Bin";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Visible = false;
            // 
            // tbMaterialNo
            // 
            this.tbMaterialNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbMaterialNo.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbMaterialNo.Location = new System.Drawing.Point(565, 77);
            this.tbMaterialNo.Name = "tbMaterialNo";
            this.tbMaterialNo.Size = new System.Drawing.Size(267, 29);
            this.tbMaterialNo.TabIndex = 2;
            this.tbMaterialNo.Visible = false;
            this.tbMaterialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbMaterialNo_KeyDown);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label4.Location = new System.Drawing.Point(433, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 27);
            this.label4.TabIndex = 11;
            this.label4.Text = "Material No.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Visible = false;
            // 
            // tbLotNo
            // 
            this.tbLotNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbLotNo.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbLotNo.Location = new System.Drawing.Point(136, 42);
            this.tbLotNo.Name = "tbLotNo";
            this.tbLotNo.Size = new System.Drawing.Size(267, 29);
            this.tbLotNo.TabIndex = 3;
            this.tbLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotNo_KeyDown);
            // 
            // tbSelDevice
            // 
            this.tbSelDevice.BackColor = System.Drawing.Color.LightGray;
            this.tbSelDevice.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.tbSelDevice.Location = new System.Drawing.Point(136, 7);
            this.tbSelDevice.Name = "tbSelDevice";
            this.tbSelDevice.Size = new System.Drawing.Size(267, 29);
            this.tbSelDevice.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label3.Location = new System.Drawing.Point(4, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 27);
            this.label3.TabIndex = 2;
            this.label3.Text = "LOT NO";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "DEVICE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btLotOpen
            // 
            this.btLotOpen.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.btLotOpen.Image = ((System.Drawing.Image)(resources.GetObject("btLotOpen.Image")));
            this.btLotOpen.Location = new System.Drawing.Point(5, 123);
            this.btLotOpen.Name = "btLotOpen";
            this.btLotOpen.Size = new System.Drawing.Size(205, 50);
            this.btLotOpen.TabIndex = 9;
            this.btLotOpen.Text = "LOT OPEN";
            this.btLotOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLotOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLotOpen.UseVisualStyleBackColor = true;
            this.btLotOpen.Click += new System.EventHandler(this.btLotOpen_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("맑은 고딕 Semilight", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(421, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "LOT OPEN";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            this.btCancel.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.btCancel.Image = ((System.Drawing.Image)(resources.GetObject("btCancel.Image")));
            this.btCancel.Location = new System.Drawing.Point(212, 123);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(205, 50);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "CLOSE";
            this.btCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lvLot
            // 
            this.lvLot.Location = new System.Drawing.Point(434, 244);
            this.lvLot.Name = "lvLot";
            this.lvLot.Size = new System.Drawing.Size(410, 169);
            this.lvLot.TabIndex = 12;
            this.lvLot.UseCompatibleStateImageBehavior = false;
            this.lvLot.Visible = false;
            // 
            // btDelete
            // 
            this.btDelete.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btDelete.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.Image")));
            this.btDelete.Location = new System.Drawing.Point(637, 188);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(198, 50);
            this.btDelete.TabIndex = 13;
            this.btDelete.Text = "DELETE SELECTED LOT";
            this.btDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Visible = false;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btAdd
            // 
            this.btAdd.Font = new System.Drawing.Font("맑은 고딕", 13F);
            this.btAdd.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.Image")));
            this.btAdd.Location = new System.Drawing.Point(433, 188);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(198, 50);
            this.btAdd.TabIndex = 5;
            this.btAdd.Text = "ADD LOT";
            this.btAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Visible = false;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // FormLotOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(421, 178);
            this.Controls.Add(this.tbEmployeeID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.tbTargetBin);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lvLot);
            this.Controls.Add(this.tbMaterialNo);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btLotOpen);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLotOpen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormLotOpen_Load);
            this.Shown += new System.EventHandler(this.FormLotOpen_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox tbTargetBin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMaterialNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ListView lvLot;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btAdd;
    }
}