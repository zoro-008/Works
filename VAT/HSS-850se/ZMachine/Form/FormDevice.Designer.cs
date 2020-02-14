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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnRecipe = new System.Windows.Forms.Panel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbCrntDevice = new System.Windows.Forms.Label();
            this.lbSelDevice = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btDownload = new System.Windows.Forms.Button();
            this.pbStatus = new System.Windows.Forms.ProgressBar();
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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pnDevSet = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lvDevice);
            this.panel3.Controls.Add(this.label2);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // lvDevice
            // 
            this.lvDevice.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.lvDevice, "lvDevice");
            this.lvDevice.Name = "lvDevice";
            this.lvDevice.UseCompatibleStateImageBehavior = false;
            this.lvDevice.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvDevice_ColumnClick);
            this.lvDevice.SelectedIndexChanged += new System.EventHandler(this.lvDevice_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LightCoral;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.tabControl1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnRecipe);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnRecipe
            // 
            this.pnRecipe.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pnRecipe, "pnRecipe");
            this.pnRecipe.Name = "pnRecipe";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel3);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.lbCrntDevice);
            this.panel2.Controls.Add(this.lbSelDevice);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btDownload);
            this.panel2.Controls.Add(this.pbStatus);
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
            this.panel2.Name = "panel2";
            // 
            // lbCrntDevice
            // 
            this.lbCrntDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lbCrntDevice, "lbCrntDevice");
            this.lbCrntDevice.Name = "lbCrntDevice";
            // 
            // lbSelDevice
            // 
            this.lbSelDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lbSelDevice, "lbSelDevice");
            this.lbSelDevice.Name = "lbSelDevice";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btDownload
            // 
            resources.ApplyResources(this.btDownload, "btDownload");
            this.btDownload.Name = "btDownload";
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // pbStatus
            // 
            resources.ApplyResources(this.pbStatus, "pbStatus");
            this.pbStatus.Name = "pbStatus";
            // 
            // tbToName
            // 
            this.tbToName.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tbToName, "tbToName");
            this.tbToName.Name = "tbToName";
            // 
            // tbFromName
            // 
            this.tbFromName.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.tbFromName, "tbFromName");
            this.tbFromName.Name = "tbFromName";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LightCoral;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btCopy
            // 
            resources.ApplyResources(this.btCopy, "btCopy");
            this.btCopy.Name = "btCopy";
            this.btCopy.Tag = "";
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btRename
            // 
            resources.ApplyResources(this.btRename, "btRename");
            this.btRename.Name = "btRename";
            this.btRename.Tag = "";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            resources.ApplyResources(this.btDelete, "btDelete");
            this.btDelete.Name = "btDelete";
            this.btDelete.Tag = "";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btSetting
            // 
            resources.ApplyResources(this.btSetting, "btSetting");
            this.btSetting.Name = "btSetting";
            this.btSetting.Tag = "9";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // btNew
            // 
            resources.ApplyResources(this.btNew, "btNew");
            this.btNew.Name = "btNew";
            this.btNew.Tag = "";
            this.btNew.UseVisualStyleBackColor = true;
            this.btNew.Click += new System.EventHandler(this.btNew_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.pnDevSet);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pnDevSet
            // 
            this.pnDevSet.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pnDevSet, "pnDevSet");
            this.pnDevSet.Name = "pnDevSet";
            // 
            // FormDevice
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel4);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDevice";
            this.TransparencyKey = System.Drawing.Color.White;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDevice_FormClosing);
            this.Shown += new System.EventHandler(this.FormDevice_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormDevice_VisibleChanged);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCrntDevice;
        private System.Windows.Forms.Label lbSelDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.ProgressBar pbStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel pnRecipe;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel pnDevSet;
    }
}