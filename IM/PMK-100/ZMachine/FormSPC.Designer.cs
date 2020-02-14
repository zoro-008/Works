namespace Machine
{
    partial class FormSPC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSPC));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lvLotInfo = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvLotDate = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbErrTime = new System.Windows.Forms.RadioButton();
            this.rbErrNo = new System.Windows.Forms.RadioButton();
            this.btErrDataView = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.edErrEndTime = new System.Windows.Forms.DateTimePicker();
            this.edErrSttTime = new System.Windows.Forms.DateTimePicker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lvErrData = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.sdCSV = new System.Windows.Forms.SaveFileDialog();
            this.sdXLS = new System.Windows.Forms.SaveFileDialog();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1279, 860);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1271, 826);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "WORK INFO";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lvLotInfo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(267, 8);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(997, 818);
            this.panel2.TabIndex = 1;
            // 
            // lvLotInfo
            // 
            this.lvLotInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLotInfo.Location = new System.Drawing.Point(0, 28);
            this.lvLotInfo.Name = "lvLotInfo";
            this.lvLotInfo.Size = new System.Drawing.Size(995, 788);
            this.lvLotInfo.TabIndex = 1;
            this.lvLotInfo.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(995, 28);
            this.label2.TabIndex = 0;
            this.label2.Text = "TRAY INFO";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lvLotDate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(7, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 817);
            this.panel1.TabIndex = 0;
            // 
            // lvLotDate
            // 
            this.lvLotDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLotDate.Location = new System.Drawing.Point(0, 28);
            this.lvLotDate.Name = "lvLotDate";
            this.lvLotDate.Size = new System.Drawing.Size(257, 787);
            this.lvLotDate.TabIndex = 3;
            this.lvLotDate.UseCompatibleStateImageBehavior = false;
            this.lvLotDate.SelectedIndexChanged += new System.EventHandler(this.lvLotDate_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "LOT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.btErrDataView);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.edErrEndTime);
            this.tabPage2.Controls.Add(this.edErrSttTime);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1271, 826);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ERR HISTORY";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbErrTime);
            this.groupBox1.Controls.Add(this.rbErrNo);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(901, 708);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 86);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sorting Option";
            // 
            // rbErrTime
            // 
            this.rbErrTime.AutoSize = true;
            this.rbErrTime.Checked = true;
            this.rbErrTime.Location = new System.Drawing.Point(6, 48);
            this.rbErrTime.Name = "rbErrTime";
            this.rbErrTime.Size = new System.Drawing.Size(85, 20);
            this.rbErrTime.TabIndex = 1;
            this.rbErrTime.TabStop = true;
            this.rbErrTime.Text = "Err Time";
            this.rbErrTime.UseVisualStyleBackColor = true;
            // 
            // rbErrNo
            // 
            this.rbErrNo.AutoSize = true;
            this.rbErrNo.Location = new System.Drawing.Point(6, 22);
            this.rbErrNo.Name = "rbErrNo";
            this.rbErrNo.Size = new System.Drawing.Size(72, 20);
            this.rbErrNo.TabIndex = 0;
            this.rbErrNo.Text = "Err No";
            this.rbErrNo.UseVisualStyleBackColor = true;
            // 
            // btErrDataView
            // 
            this.btErrDataView.Location = new System.Drawing.Point(667, 743);
            this.btErrDataView.Name = "btErrDataView";
            this.btErrDataView.Size = new System.Drawing.Size(216, 45);
            this.btErrDataView.TabIndex = 6;
            this.btErrDataView.Text = "Err Data View";
            this.btErrDataView.UseVisualStyleBackColor = true;
            this.btErrDataView.Click += new System.EventHandler(this.btErrDataView_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(448, 723);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 22);
            this.label5.TabIndex = 5;
            this.label5.Text = "Until";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(238, 723);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 22);
            this.label4.TabIndex = 4;
            this.label4.Text = "From";
            // 
            // edErrEndTime
            // 
            this.edErrEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.edErrEndTime.Location = new System.Drawing.Point(452, 761);
            this.edErrEndTime.Name = "edErrEndTime";
            this.edErrEndTime.Size = new System.Drawing.Size(197, 30);
            this.edErrEndTime.TabIndex = 3;
            this.edErrEndTime.Value = new System.DateTime(2017, 4, 28, 0, 0, 0, 0);
            // 
            // edErrSttTime
            // 
            this.edErrSttTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.edErrSttTime.Location = new System.Drawing.Point(242, 761);
            this.edErrSttTime.Name = "edErrSttTime";
            this.edErrSttTime.Size = new System.Drawing.Size(197, 30);
            this.edErrSttTime.TabIndex = 2;
            this.edErrSttTime.Value = new System.DateTime(2017, 4, 28, 0, 0, 0, 0);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lvErrData);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(6, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1259, 682);
            this.panel3.TabIndex = 1;
            // 
            // lvErrData
            // 
            this.lvErrData.Location = new System.Drawing.Point(-1, 41);
            this.lvErrData.Name = "lvErrData";
            this.lvErrData.Size = new System.Drawing.Size(1259, 640);
            this.lvErrData.TabIndex = 1;
            this.lvErrData.UseCompatibleStateImageBehavior = false;
            // 
            // label3
            // 
            this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1257, 41);
            this.label3.TabIndex = 0;
            this.label3.Text = "ERROR DATA";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormSPC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 863);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSPC";
            this.Text = "FormSPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSPC_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView lvLotInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbErrTime;
        private System.Windows.Forms.RadioButton rbErrNo;
        private System.Windows.Forms.Button btErrDataView;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker edErrEndTime;
        private System.Windows.Forms.DateTimePicker edErrSttTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lvErrData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog sdCSV;
        private System.Windows.Forms.SaveFileDialog sdXLS;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.ListView lvLotDate;
    }
}