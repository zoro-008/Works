namespace Machine
{
    partial class FormDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDevice));
            this.panel3 = new System.Windows.Forms.Panel();
            this.lvDevice = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbToName = new System.Windows.Forms.TextBox();
            this.tbFromName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btCopy = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btSetting = new System.Windows.Forms.Button();
            this.btNew = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbCrntDevice = new System.Windows.Forms.Label();
            this.lbSelDevice = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btDownload = new System.Windows.Forms.Button();
            this.pbStatus = new System.Windows.Forms.ProgressBar();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lvDevice);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(1, 195);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(636, 671);
            this.panel3.TabIndex = 3;
            // 
            // lvDevice
            // 
            this.lvDevice.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvDevice.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDevice.Location = new System.Drawing.Point(-1, 34);
            this.lvDevice.Name = "lvDevice";
            this.lvDevice.Size = new System.Drawing.Size(636, 634);
            this.lvDevice.TabIndex = 1;
            this.lvDevice.UseCompatibleStateImageBehavior = false;
            this.lvDevice.SelectedIndexChanged += new System.EventHandler(this.lvDevice_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label2.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(635, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "RECIPE LIST";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1272, 866);
            this.panel4.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tbToName);
            this.panel2.Controls.Add(this.tbFromName);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.btCopy);
            this.panel2.Controls.Add(this.btRename);
            this.panel2.Controls.Add(this.btDelete);
            this.panel2.Controls.Add(this.btSetting);
            this.panel2.Controls.Add(this.btNew);
            this.panel2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel2.Location = new System.Drawing.Point(636, -1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(641, 865);
            this.panel2.TabIndex = 4;
            // 
            // tbToName
            // 
            this.tbToName.BackColor = System.Drawing.SystemColors.Window;
            this.tbToName.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbToName.Location = new System.Drawing.Point(204, 126);
            this.tbToName.Multiline = true;
            this.tbToName.Name = "tbToName";
            this.tbToName.Size = new System.Drawing.Size(346, 32);
            this.tbToName.TabIndex = 16;
            // 
            // tbFromName
            // 
            this.tbFromName.BackColor = System.Drawing.Color.Silver;
            this.tbFromName.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbFromName.Location = new System.Drawing.Point(204, 67);
            this.tbFromName.Multiline = true;
            this.tbFromName.Name = "tbFromName";
            this.tbFromName.Size = new System.Drawing.Size(346, 32);
            this.tbFromName.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(32, 130);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 24);
            this.label7.TabIndex = 14;
            this.label7.Text = "TO NAME";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label3.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(636, 35);
            this.label3.TabIndex = 3;
            this.label3.Text = "RECIPE EDIT";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(32, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 24);
            this.label6.TabIndex = 13;
            this.label6.Text = "FROM NAME";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btCopy
            // 
            this.btCopy.Font = new System.Drawing.Font("굴림", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btCopy.Image = ((System.Drawing.Image)(resources.GetObject("btCopy.Image")));
            this.btCopy.Location = new System.Drawing.Point(322, 200);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(276, 89);
            this.btCopy.TabIndex = 12;
            this.btCopy.Tag = "";
            this.btCopy.Text = "COPY";
            this.btCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btRename
            // 
            this.btRename.Font = new System.Drawing.Font("굴림", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btRename.Image = ((System.Drawing.Image)(resources.GetObject("btRename.Image")));
            this.btRename.Location = new System.Drawing.Point(40, 296);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(276, 89);
            this.btRename.TabIndex = 11;
            this.btRename.Tag = "";
            this.btRename.Text = "RENAME";
            this.btRename.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btRename.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            this.btDelete.Font = new System.Drawing.Font("굴림", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btDelete.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.Image")));
            this.btDelete.Location = new System.Drawing.Point(322, 296);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(276, 89);
            this.btDelete.TabIndex = 10;
            this.btDelete.Tag = "";
            this.btDelete.Text = "DELETE";
            this.btDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btSetting
            // 
            this.btSetting.Font = new System.Drawing.Font("굴림", 23F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btSetting.Image = ((System.Drawing.Image)(resources.GetObject("btSetting.Image")));
            this.btSetting.Location = new System.Drawing.Point(132, 687);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(378, 109);
            this.btSetting.TabIndex = 9;
            this.btSetting.Tag = "9";
            this.btSetting.Text = "RECIPE DATA SETTING FORM";
            this.btSetting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSetting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // btNew
            // 
            this.btNew.Font = new System.Drawing.Font("굴림", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btNew.Image = ((System.Drawing.Image)(resources.GetObject("btNew.Image")));
            this.btNew.Location = new System.Drawing.Point(40, 200);
            this.btNew.Name = "btNew";
            this.btNew.Size = new System.Drawing.Size(276, 89);
            this.btNew.TabIndex = 5;
            this.btNew.Tag = "";
            this.btNew.Text = "NEW";
            this.btNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btNew.UseVisualStyleBackColor = true;
            this.btNew.Click += new System.EventHandler(this.btNew_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lbCrntDevice);
            this.panel1.Controls.Add(this.lbSelDevice);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btDownload);
            this.panel1.Controls.Add(this.pbStatus);
            this.panel1.Location = new System.Drawing.Point(1, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(636, 197);
            this.panel1.TabIndex = 1;
            // 
            // lbCrntDevice
            // 
            this.lbCrntDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCrntDevice.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCrntDevice.Location = new System.Drawing.Point(228, 45);
            this.lbCrntDevice.Name = "lbCrntDevice";
            this.lbCrntDevice.Size = new System.Drawing.Size(237, 38);
            this.lbCrntDevice.TabIndex = 8;
            this.lbCrntDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSelDevice
            // 
            this.lbSelDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbSelDevice.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSelDevice.Location = new System.Drawing.Point(228, 89);
            this.lbSelDevice.Name = "lbSelDevice";
            this.lbSelDevice.Size = new System.Drawing.Size(237, 38);
            this.lbSelDevice.TabIndex = 7;
            this.lbSelDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(24, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(193, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "SELECTED RECIPE";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(24, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(193, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "CURRENT RECIPE";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(633, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "RECIPE SELECTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btDownload
            // 
            this.btDownload.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.btDownload.Image = ((System.Drawing.Image)(resources.GetObject("btDownload.Image")));
            this.btDownload.Location = new System.Drawing.Point(476, 46);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(152, 81);
            this.btDownload.TabIndex = 4;
            this.btDownload.Text = "DOWN LOAD";
            this.btDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(9, 147);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(618, 36);
            this.pbStatus.TabIndex = 3;
            // 
            // FormDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1272, 866);
            this.Controls.Add(this.panel4);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDevice";
            this.Text = "FormDevice";
            this.TransparencyKey = System.Drawing.Color.White;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDevice_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FormDevice_VisibleChanged);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.ListView lvDevice;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbToName;
        private System.Windows.Forms.TextBox tbFromName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Button btNew;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbCrntDevice;
        private System.Windows.Forms.Label lbSelDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.ProgressBar pbStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;


    }
}