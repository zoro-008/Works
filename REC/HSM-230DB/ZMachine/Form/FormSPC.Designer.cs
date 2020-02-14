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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btWorkDataView = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dpWrkEndTime = new System.Windows.Forms.DateTimePicker();
            this.dpWrkSttTime = new System.Windows.Forms.DateTimePicker();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lvWrkData = new System.Windows.Forms.ListView();
            this.label8 = new System.Windows.Forms.Label();
            this.sdCSV = new System.Windows.Forms.SaveFileDialog();
            this.sdXLS = new System.Windows.Forms.SaveFileDialog();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvLotDate = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lvLotInfo = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1279, 860);
            this.tabControl1.TabIndex = 0;
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
            this.edErrEndTime.Value = new System.DateTime(2017, 6, 8, 0, 0, 0, 0);
            // 
            // edErrSttTime
            // 
            this.edErrSttTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.edErrSttTime.Location = new System.Drawing.Point(242, 761);
            this.edErrSttTime.Name = "edErrSttTime";
            this.edErrSttTime.Size = new System.Drawing.Size(197, 30);
            this.edErrSttTime.TabIndex = 2;
            this.edErrSttTime.Value = new System.DateTime(2017, 6, 8, 0, 0, 0, 0);
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btWorkDataView);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.dpWrkEndTime);
            this.tabPage3.Controls.Add(this.dpWrkSttTime);
            this.tabPage3.Controls.Add(this.panel4);
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1271, 826);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "WORK";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btWorkDataView
            // 
            this.btWorkDataView.Location = new System.Drawing.Point(667, 743);
            this.btWorkDataView.Name = "btWorkDataView";
            this.btWorkDataView.Size = new System.Drawing.Size(216, 45);
            this.btWorkDataView.TabIndex = 12;
            this.btWorkDataView.Text = "Work Data View";
            this.btWorkDataView.UseVisualStyleBackColor = true;
            this.btWorkDataView.Click += new System.EventHandler(this.btWorkDataView_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(448, 723);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 22);
            this.label6.TabIndex = 11;
            this.label6.Text = "Until";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(238, 723);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 22);
            this.label7.TabIndex = 10;
            this.label7.Text = "From";
            // 
            // dpWrkEndTime
            // 
            this.dpWrkEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpWrkEndTime.Location = new System.Drawing.Point(452, 761);
            this.dpWrkEndTime.Name = "dpWrkEndTime";
            this.dpWrkEndTime.Size = new System.Drawing.Size(197, 30);
            this.dpWrkEndTime.TabIndex = 9;
            this.dpWrkEndTime.Value = new System.DateTime(2017, 6, 8, 0, 0, 0, 0);
            // 
            // dpWrkSttTime
            // 
            this.dpWrkSttTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpWrkSttTime.Location = new System.Drawing.Point(242, 761);
            this.dpWrkSttTime.Name = "dpWrkSttTime";
            this.dpWrkSttTime.Size = new System.Drawing.Size(197, 30);
            this.dpWrkSttTime.TabIndex = 8;
            this.dpWrkSttTime.Value = new System.DateTime(2017, 6, 8, 0, 0, 0, 0);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lvWrkData);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Location = new System.Drawing.Point(6, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1259, 682);
            this.panel4.TabIndex = 7;
            // 
            // lvWrkData
            // 
            this.lvWrkData.Location = new System.Drawing.Point(-1, 41);
            this.lvWrkData.Name = "lvWrkData";
            this.lvWrkData.Size = new System.Drawing.Size(1259, 640);
            this.lvWrkData.TabIndex = 1;
            this.lvWrkData.UseCompatibleStateImageBehavior = false;
            // 
            // label8
            // 
            this.label8.Image = ((System.Drawing.Image)(resources.GetObject("label8.Image")));
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(1257, 41);
            this.label8.TabIndex = 0;
            this.label8.Text = "WORK DATA";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel2);
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 30);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1271, 826);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lvLotDate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 817);
            this.panel1.TabIndex = 1;
            this.panel1.Visible = false;
            // 
            // lvLotDate
            // 
            this.lvLotDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLotDate.Location = new System.Drawing.Point(0, 28);
            this.lvLotDate.Name = "lvLotDate";
            this.lvLotDate.Size = new System.Drawing.Size(257, 787);
            this.lvLotDate.TabIndex = 3;
            this.lvLotDate.UseCompatibleStateImageBehavior = false;
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
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lvLotInfo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(265, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(997, 818);
            this.panel2.TabIndex = 2;
            this.panel2.Visible = false;
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
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
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
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btWorkDataView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dpWrkEndTime;
        private System.Windows.Forms.DateTimePicker dpWrkSttTime;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListView lvWrkData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView lvLotInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvLotDate;
        private System.Windows.Forms.Label label1;
    }
}