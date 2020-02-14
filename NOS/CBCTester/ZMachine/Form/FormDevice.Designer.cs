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
            this.metroTile9 = new MetroFramework.Controls.MetroTile();
            this.btNew = new MetroFramework.Controls.MetroTile();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btSetting = new MetroFramework.Controls.MetroTile();
            this.btDownload = new MetroFramework.Controls.MetroTile();
            this.pbStatus = new MetroFramework.Controls.MetroProgressBar();
            this.tbToName = new System.Windows.Forms.TextBox();
            this.btRename = new MetroFramework.Controls.MetroTile();
            this.tbFromName = new System.Windows.Forms.TextBox();
            this.metroTile2 = new MetroFramework.Controls.MetroTile();
            this.btDelete = new MetroFramework.Controls.MetroTile();
            this.btCopy = new MetroFramework.Controls.MetroTile();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.label6 = new System.Windows.Forms.Label();
            this.lbCrntDevice = new System.Windows.Forms.Label();
            this.lbSelDevice = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.lvDevice);
            this.panel3.Controls.Add(this.metroTile9);
            this.panel3.Name = "panel3";
            // 
            // lvDevice
            // 
            resources.ApplyResources(this.lvDevice, "lvDevice");
            this.lvDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lvDevice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvDevice.ForeColor = System.Drawing.Color.White;
            this.lvDevice.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvDevice.Name = "lvDevice";
            this.lvDevice.UseCompatibleStateImageBehavior = false;
            this.lvDevice.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvDevice_ColumnClick);
            this.lvDevice.SelectedIndexChanged += new System.EventHandler(this.lvDevice_SelectedIndexChanged);
            // 
            // metroTile9
            // 
            resources.ApplyResources(this.metroTile9, "metroTile9");
            this.metroTile9.ActiveControl = null;
            this.metroTile9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.metroTile9.Name = "metroTile9";
            this.metroTile9.Tag = "3";
            this.metroTile9.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile9.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile9.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile9.UseSelectable = true;
            this.metroTile9.UseTileImage = true;
            // 
            // btNew
            // 
            resources.ApplyResources(this.btNew, "btNew");
            this.btNew.ActiveControl = null;
            this.btNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btNew.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btNew.Name = "btNew";
            this.btNew.Tag = "0";
            this.btNew.TileImage = ((System.Drawing.Image)(resources.GetObject("btNew.TileImage")));
            this.btNew.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btNew.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btNew.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btNew.UseCustomBackColor = true;
            this.btNew.UseSelectable = true;
            this.btNew.UseTileImage = true;
            this.btNew.Click += new System.EventHandler(this.btNew_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.BackColor = System.Drawing.Color.Black;
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Name = "panel4";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.btSetting);
            this.panel2.Controls.Add(this.btDownload);
            this.panel2.Controls.Add(this.pbStatus);
            this.panel2.Controls.Add(this.tbToName);
            this.panel2.Controls.Add(this.btRename);
            this.panel2.Controls.Add(this.tbFromName);
            this.panel2.Controls.Add(this.metroTile2);
            this.panel2.Controls.Add(this.btDelete);
            this.panel2.Controls.Add(this.btCopy);
            this.panel2.Controls.Add(this.btNew);
            this.panel2.Controls.Add(this.metroTile1);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.lbCrntDevice);
            this.panel2.Controls.Add(this.lbSelDevice);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Name = "panel2";
            // 
            // btSetting
            // 
            resources.ApplyResources(this.btSetting, "btSetting");
            this.btSetting.ActiveControl = null;
            this.btSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.btSetting.Name = "btSetting";
            this.btSetting.Tag = "3";
            this.btSetting.TileImage = ((System.Drawing.Image)(resources.GetObject("btSetting.TileImage")));
            this.btSetting.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btSetting.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btSetting.UseSelectable = true;
            this.btSetting.UseTileImage = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // btDownload
            // 
            resources.ApplyResources(this.btDownload, "btDownload");
            this.btDownload.ActiveControl = null;
            this.btDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(188)))), ((int)(((byte)(0)))));
            this.btDownload.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btDownload.Name = "btDownload";
            this.btDownload.Tag = "0";
            this.btDownload.TileImage = ((System.Drawing.Image)(resources.GetObject("btDownload.TileImage")));
            this.btDownload.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btDownload.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btDownload.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btDownload.UseCustomBackColor = true;
            this.btDownload.UseSelectable = true;
            this.btDownload.UseTileImage = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // pbStatus
            // 
            resources.ApplyResources(this.pbStatus, "pbStatus");
            this.pbStatus.Name = "pbStatus";
            // 
            // tbToName
            // 
            resources.ApplyResources(this.tbToName, "tbToName");
            this.tbToName.BackColor = System.Drawing.Color.Black;
            this.tbToName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbToName.ForeColor = System.Drawing.Color.White;
            this.tbToName.Name = "tbToName";
            // 
            // btRename
            // 
            resources.ApplyResources(this.btRename, "btRename");
            this.btRename.ActiveControl = null;
            this.btRename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(188)))), ((int)(((byte)(0)))));
            this.btRename.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btRename.Name = "btRename";
            this.btRename.Tag = "0";
            this.btRename.TileImage = ((System.Drawing.Image)(resources.GetObject("btRename.TileImage")));
            this.btRename.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btRename.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btRename.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btRename.UseCustomBackColor = true;
            this.btRename.UseSelectable = true;
            this.btRename.UseTileImage = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // tbFromName
            // 
            resources.ApplyResources(this.tbFromName, "tbFromName");
            this.tbFromName.BackColor = System.Drawing.Color.Gray;
            this.tbFromName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFromName.ForeColor = System.Drawing.Color.White;
            this.tbFromName.Name = "tbFromName";
            // 
            // metroTile2
            // 
            resources.ApplyResources(this.metroTile2, "metroTile2");
            this.metroTile2.ActiveControl = null;
            this.metroTile2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.metroTile2.Name = "metroTile2";
            this.metroTile2.Tag = "3";
            this.metroTile2.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile2.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile2.UseSelectable = true;
            this.metroTile2.UseTileImage = true;
            // 
            // btDelete
            // 
            resources.ApplyResources(this.btDelete, "btDelete");
            this.btDelete.ActiveControl = null;
            this.btDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(65)))), ((int)(((byte)(153)))));
            this.btDelete.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btDelete.Name = "btDelete";
            this.btDelete.Tag = "0";
            this.btDelete.TileImage = ((System.Drawing.Image)(resources.GetObject("btDelete.TileImage")));
            this.btDelete.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btDelete.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btDelete.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btDelete.UseCustomBackColor = true;
            this.btDelete.UseSelectable = true;
            this.btDelete.UseTileImage = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btCopy
            // 
            resources.ApplyResources(this.btCopy, "btCopy");
            this.btCopy.ActiveControl = null;
            this.btCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(119)))), ((int)(((byte)(53)))));
            this.btCopy.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btCopy.Name = "btCopy";
            this.btCopy.Tag = "0";
            this.btCopy.TileImage = ((System.Drawing.Image)(resources.GetObject("btCopy.TileImage")));
            this.btCopy.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btCopy.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btCopy.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btCopy.UseCustomBackColor = true;
            this.btCopy.UseSelectable = true;
            this.btCopy.UseTileImage = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // metroTile1
            // 
            resources.ApplyResources(this.metroTile1, "metroTile1");
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Tag = "3";
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile1.UseSelectable = true;
            this.metroTile1.UseTileImage = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // lbCrntDevice
            // 
            resources.ApplyResources(this.lbCrntDevice, "lbCrntDevice");
            this.lbCrntDevice.BackColor = System.Drawing.Color.Black;
            this.lbCrntDevice.ForeColor = System.Drawing.Color.White;
            this.lbCrntDevice.Name = "lbCrntDevice";
            // 
            // lbSelDevice
            // 
            resources.ApplyResources(this.lbSelDevice, "lbSelDevice");
            this.lbSelDevice.BackColor = System.Drawing.Color.Black;
            this.lbSelDevice.ForeColor = System.Drawing.Color.White;
            this.lbSelDevice.Name = "lbSelDevice";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Name = "label7";
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
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.ListView lvDevice;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbCrntDevice;
        private System.Windows.Forms.Label lbSelDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private MetroFramework.Controls.MetroTile metroTile9;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroTile metroTile2;
        private MetroFramework.Controls.MetroTile btNew;
        private MetroFramework.Controls.MetroTile btRename;
        private MetroFramework.Controls.MetroTile btDelete;
        private MetroFramework.Controls.MetroTile btCopy;
        private MetroFramework.Controls.MetroTile btDownload;
        private MetroFramework.Controls.MetroProgressBar pbStatus;
        private System.Windows.Forms.TextBox tbToName;
        private System.Windows.Forms.TextBox tbFromName;
        private MetroFramework.Controls.MetroTile btSetting;
    }
}