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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLotOpen));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.tbEmployeeID = new System.Windows.Forms.TextBox();
            this.btLotAlias = new System.Windows.Forms.Button();
            this.btClearMaterialNo = new System.Windows.Forms.Button();
            this.btClearLotNo = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbLotAlias = new System.Windows.Forms.TextBox();
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
            this.lbConDB = new System.Windows.Forms.Label();
            this.tmMakeDMC1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.tbEmployeeID);
            this.panel1.Controls.Add(this.btLotAlias);
            this.panel1.Controls.Add(this.btClearMaterialNo);
            this.panel1.Controls.Add(this.btClearLotNo);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbLotAlias);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tbMaterialNo);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbLotNo);
            this.panel1.Controls.Add(this.tbSelDevice);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(5, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 167);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(341, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 28);
            this.button1.TabIndex = 20;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbEmployeeID
            // 
            this.tbEmployeeID.BackColor = System.Drawing.SystemColors.Window;
            this.tbEmployeeID.Location = new System.Drawing.Point(139, 133);
            this.tbEmployeeID.Name = "tbEmployeeID";
            this.tbEmployeeID.Size = new System.Drawing.Size(199, 26);
            this.tbEmployeeID.TabIndex = 4;
            this.tbEmployeeID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbEmployeeID_KeyDown);
            // 
            // btLotAlias
            // 
            this.btLotAlias.Location = new System.Drawing.Point(341, 100);
            this.btLotAlias.Name = "btLotAlias";
            this.btLotAlias.Size = new System.Drawing.Size(63, 28);
            this.btLotAlias.TabIndex = 18;
            this.btLotAlias.Text = "Clear";
            this.btLotAlias.UseVisualStyleBackColor = true;
            this.btLotAlias.Click += new System.EventHandler(this.btLotAlias_Click);
            // 
            // btClearMaterialNo
            // 
            this.btClearMaterialNo.Location = new System.Drawing.Point(341, 69);
            this.btClearMaterialNo.Name = "btClearMaterialNo";
            this.btClearMaterialNo.Size = new System.Drawing.Size(63, 28);
            this.btClearMaterialNo.TabIndex = 17;
            this.btClearMaterialNo.Text = "Clear";
            this.btClearMaterialNo.UseVisualStyleBackColor = true;
            this.btClearMaterialNo.Click += new System.EventHandler(this.btClearMaterialNo_Click);
            // 
            // btClearLotNo
            // 
            this.btClearLotNo.Location = new System.Drawing.Point(341, 37);
            this.btClearLotNo.Name = "btClearLotNo";
            this.btClearLotNo.Size = new System.Drawing.Size(63, 28);
            this.btClearLotNo.TabIndex = 16;
            this.btClearLotNo.Text = "Clear";
            this.btClearLotNo.UseVisualStyleBackColor = true;
            this.btClearLotNo.Click += new System.EventHandler(this.btClearLotNo_Click);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(7, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Employee ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLotAlias
            // 
            this.tbLotAlias.BackColor = System.Drawing.SystemColors.Window;
            this.tbLotAlias.Location = new System.Drawing.Point(139, 101);
            this.tbLotAlias.Name = "tbLotAlias";
            this.tbLotAlias.Size = new System.Drawing.Size(199, 26);
            this.tbLotAlias.TabIndex = 3;
            this.tbLotAlias.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotAlias_KeyDown);
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(7, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 27);
            this.label5.TabIndex = 13;
            this.label5.Text = "Lot Alias";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbMaterialNo
            // 
            this.tbMaterialNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbMaterialNo.Location = new System.Drawing.Point(139, 69);
            this.tbMaterialNo.Name = "tbMaterialNo";
            this.tbMaterialNo.Size = new System.Drawing.Size(199, 26);
            this.tbMaterialNo.TabIndex = 2;
            this.tbMaterialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbMaterialNo_KeyDown);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(7, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 27);
            this.label4.TabIndex = 11;
            this.label4.Text = "Material No.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLotNo
            // 
            this.tbLotNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbLotNo.Location = new System.Drawing.Point(139, 38);
            this.tbLotNo.Name = "tbLotNo";
            this.tbLotNo.Size = new System.Drawing.Size(199, 26);
            this.tbLotNo.TabIndex = 1;
            this.tbLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotNo_KeyDown);
            // 
            // tbSelDevice
            // 
            this.tbSelDevice.BackColor = System.Drawing.Color.LightGray;
            this.tbSelDevice.Location = new System.Drawing.Point(139, 7);
            this.tbSelDevice.Name = "tbSelDevice";
            this.tbSelDevice.ReadOnly = true;
            this.tbSelDevice.Size = new System.Drawing.Size(267, 26);
            this.tbSelDevice.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(7, 37);
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
            this.btLotOpen.Location = new System.Drawing.Point(5, 245);
            this.btLotOpen.Name = "btLotOpen";
            this.btLotOpen.Size = new System.Drawing.Size(198, 50);
            this.btLotOpen.TabIndex = 5;
            this.btLotOpen.Text = "START LOT";
            this.btLotOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLotOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLotOpen.UseVisualStyleBackColor = true;
            this.btLotOpen.Click += new System.EventHandler(this.btLotOpen_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(417, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "START LOT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            this.btCancel.Image = ((System.Drawing.Image)(resources.GetObject("btCancel.Image")));
            this.btCancel.Location = new System.Drawing.Point(209, 245);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(198, 50);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "CLOSE";
            this.btCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lvLot
            // 
            this.lvLot.Location = new System.Drawing.Point(27, 297);
            this.lvLot.Name = "lvLot";
            this.lvLot.Size = new System.Drawing.Size(410, 169);
            this.lvLot.TabIndex = 12;
            this.lvLot.UseCompatibleStateImageBehavior = false;
            this.lvLot.Visible = false;
            // 
            // btDelete
            // 
            this.btDelete.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.Image")));
            this.btDelete.Location = new System.Drawing.Point(209, 290);
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
            this.btAdd.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.Image")));
            this.btAdd.Location = new System.Drawing.Point(5, 290);
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
            // lbConDB
            // 
            this.lbConDB.AutoSize = true;
            this.lbConDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConDB.ForeColor = System.Drawing.Color.Green;
            this.lbConDB.Location = new System.Drawing.Point(10, 218);
            this.lbConDB.Name = "lbConDB";
            this.lbConDB.Size = new System.Drawing.Size(173, 22);
            this.lbConDB.TabIndex = 14;
            this.lbConDB.Text = "Connecting To DB...";
            this.lbConDB.Visible = false;
            // 
            // tmMakeDMC1
            // 
            this.tmMakeDMC1.Enabled = true;
            this.tmMakeDMC1.Interval = 500;
            this.tmMakeDMC1.Tick += new System.EventHandler(this.tmMakeDMC1_Tick);
            // 
            // FormLotOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(417, 326);
            this.Controls.Add(this.lbConDB);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.lvLot);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btLotOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLotOpen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormLotOpen_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormLotOpen_VisibleChanged);
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbLotAlias;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMaterialNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ListView lvLot;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btLotAlias;
        private System.Windows.Forms.Button btClearMaterialNo;
        private System.Windows.Forms.Button btClearLotNo;
        private System.Windows.Forms.Label lbConDB;
        private System.Windows.Forms.Timer tmMakeDMC1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbEmployeeID;
    }
}