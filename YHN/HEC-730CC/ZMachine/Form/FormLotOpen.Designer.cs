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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbLotNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DgView = new System.Windows.Forms.DataGridView();
            this.LOTNAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CLEAR = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbSelDevice = new System.Windows.Forms.TextBox();
            this.btClear = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btLotOpen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgView)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbLotNo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.DgView);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Name = "panel1";
            // 
            // tbLotNo
            // 
            this.tbLotNo.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tbLotNo, "tbLotNo");
            this.tbLotNo.Name = "tbLotNo";
            this.tbLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotNo_KeyDown);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // DgView
            // 
            this.DgView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LOTNAME,
            this.CLEAR});
            resources.ApplyResources(this.DgView, "DgView");
            this.DgView.Name = "DgView";
            this.DgView.RowTemplate.Height = 23;
            this.DgView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.DgView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DgView_RowPostPaint);
            // 
            // LOTNAME
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LOTNAME.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.LOTNAME, "LOTNAME");
            this.LOTNAME.Name = "LOTNAME";
            this.LOTNAME.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // CLEAR
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CLEAR.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.CLEAR, "CLEAR");
            this.CLEAR.Name = "CLEAR";
            this.CLEAR.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CLEAR.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CLEAR.Text = "CLEAR";
            this.CLEAR.UseColumnTextForButtonValue = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbSelDevice);
            this.panel2.Controls.Add(this.btClear);
            this.panel2.Controls.Add(this.label2);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tbSelDevice
            // 
            this.tbSelDevice.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.tbSelDevice, "tbSelDevice");
            this.tbSelDevice.Name = "tbSelDevice";
            // 
            // btClear
            // 
            resources.ApplyResources(this.btClear, "btClear");
            this.btClear.Name = "btClear";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btLotOpen
            // 
            resources.ApplyResources(this.btLotOpen, "btLotOpen");
            this.btLotOpen.Name = "btLotOpen";
            this.btLotOpen.UseVisualStyleBackColor = true;
            this.btLotOpen.Click += new System.EventHandler(this.btLotOpen_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btCancel
            // 
            resources.ApplyResources(this.btCancel, "btCancel");
            this.btCancel.Name = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // FormLotOpen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btLotOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLotOpen";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormLotOpen_Load);
            this.Shown += new System.EventHandler(this.FormLotOpen_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btLotOpen;
        private System.Windows.Forms.TextBox tbLotNo;
        private System.Windows.Forms.TextBox tbSelDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.DataGridView DgView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.DataGridViewTextBoxColumn LOTNAME;
        private System.Windows.Forms.DataGridViewButtonColumn CLEAR;
    }
}