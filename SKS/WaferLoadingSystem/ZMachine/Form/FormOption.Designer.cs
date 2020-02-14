namespace Machine
{
    partial class FomOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FomOption));
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.btSave = new MetroFramework.Controls.MetroTile();
            this.cb = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.tbEquipName = new System.Windows.Forms.TextBox();
            this.tbEquipSerial = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbLoadingStop = new System.Windows.Forms.CheckBox();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbAutoQc = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.edUnFreezeMin = new System.Windows.Forms.NumericUpDown();
            this.edQcStartMin = new System.Windows.Forms.NumericUpDown();
            this.edQcStartHour = new System.Windows.Forms.NumericUpDown();
            this.metroTile3 = new MetroFramework.Controls.MetroTile();
            this.cb.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edUnFreezeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edQcStartMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edQcStartHour)).BeginInit();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // btSave
            // 
            resources.ApplyResources(this.btSave, "btSave");
            this.btSave.ActiveControl = null;
            this.btSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.btSave.Name = "btSave";
            this.btSave.Tag = "3";
            this.btSave.TileImage = ((System.Drawing.Image)(resources.GetObject("btSave.TileImage")));
            this.btSave.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btSave.UseSelectable = true;
            this.btSave.UseTileImage = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // cb
            // 
            resources.ApplyResources(this.cb, "cb");
            this.cb.BackColor = System.Drawing.Color.Black;
            this.cb.Controls.Add(this.panel3);
            this.cb.Controls.Add(this.panel2);
            this.cb.Controls.Add(this.panel1);
            this.cb.Name = "cb";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.metroTile4);
            this.panel3.Controls.Add(this.tbEquipName);
            this.panel3.Controls.Add(this.tbEquipSerial);
            this.panel3.Name = "panel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // metroTile4
            // 
            resources.ApplyResources(this.metroTile4, "metroTile4");
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Tag = "3";
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile4.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile4.UseSelectable = true;
            this.metroTile4.UseTileImage = true;
            // 
            // tbEquipName
            // 
            resources.ApplyResources(this.tbEquipName, "tbEquipName");
            this.tbEquipName.BackColor = System.Drawing.Color.Black;
            this.tbEquipName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbEquipName.ForeColor = System.Drawing.Color.White;
            this.tbEquipName.Name = "tbEquipName";
            // 
            // tbEquipSerial
            // 
            resources.ApplyResources(this.tbEquipSerial, "tbEquipSerial");
            this.tbEquipSerial.BackColor = System.Drawing.Color.Black;
            this.tbEquipSerial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbEquipSerial.ForeColor = System.Drawing.Color.White;
            this.tbEquipSerial.Name = "tbEquipSerial";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.cbLoadingStop);
            this.panel2.Controls.Add(this.cbIgnrDoor);
            this.panel2.Controls.Add(this.metroTile1);
            this.panel2.Name = "panel2";
            // 
            // cbLoadingStop
            // 
            resources.ApplyResources(this.cbLoadingStop, "cbLoadingStop");
            this.cbLoadingStop.ForeColor = System.Drawing.Color.White;
            this.cbLoadingStop.Name = "cbLoadingStop";
            this.cbLoadingStop.UseVisualStyleBackColor = true;
            // 
            // cbIgnrDoor
            // 
            resources.ApplyResources(this.cbIgnrDoor, "cbIgnrDoor");
            this.cbIgnrDoor.ForeColor = System.Drawing.Color.White;
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
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
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.cbAutoQc);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.edUnFreezeMin);
            this.panel1.Controls.Add(this.edQcStartMin);
            this.panel1.Controls.Add(this.edQcStartHour);
            this.panel1.Controls.Add(this.metroTile3);
            this.panel1.Name = "panel1";
            // 
            // cbAutoQc
            // 
            resources.ApplyResources(this.cbAutoQc, "cbAutoQc");
            this.cbAutoQc.ForeColor = System.Drawing.Color.White;
            this.cbAutoQc.Name = "cbAutoQc";
            this.cbAutoQc.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // edUnFreezeMin
            // 
            resources.ApplyResources(this.edUnFreezeMin, "edUnFreezeMin");
            this.edUnFreezeMin.BackColor = System.Drawing.Color.Black;
            this.edUnFreezeMin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edUnFreezeMin.ForeColor = System.Drawing.Color.White;
            this.edUnFreezeMin.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.edUnFreezeMin.Name = "edUnFreezeMin";
            // 
            // edQcStartMin
            // 
            resources.ApplyResources(this.edQcStartMin, "edQcStartMin");
            this.edQcStartMin.BackColor = System.Drawing.Color.Black;
            this.edQcStartMin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edQcStartMin.ForeColor = System.Drawing.Color.White;
            this.edQcStartMin.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.edQcStartMin.Name = "edQcStartMin";
            // 
            // edQcStartHour
            // 
            resources.ApplyResources(this.edQcStartHour, "edQcStartHour");
            this.edQcStartHour.BackColor = System.Drawing.Color.Black;
            this.edQcStartHour.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edQcStartHour.ForeColor = System.Drawing.Color.White;
            this.edQcStartHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.edQcStartHour.Name = "edQcStartHour";
            // 
            // metroTile3
            // 
            resources.ApplyResources(this.metroTile3, "metroTile3");
            this.metroTile3.ActiveControl = null;
            this.metroTile3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.metroTile3.Name = "metroTile3";
            this.metroTile3.Tag = "3";
            this.metroTile3.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile3.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile3.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile3.UseSelectable = true;
            this.metroTile3.UseTileImage = true;
            // 
            // FomOption
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.cb);
            this.Controls.Add(this.btSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FomOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOption_VisibleChanged);
            this.cb.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edUnFreezeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edQcStartMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edQcStartHour)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmUpdate;
        private MetroFramework.Controls.MetroTile btSave;
        private System.Windows.Forms.Panel cb;
        private System.Windows.Forms.TextBox tbEquipName;
        private System.Windows.Forms.TextBox tbEquipSerial;
        private System.Windows.Forms.Panel panel3;
        private MetroFramework.Controls.MetroTile metroTile4;
        private System.Windows.Forms.Panel panel2;
        private MetroFramework.Controls.MetroTile metroTile1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown edUnFreezeMin;
        private System.Windows.Forms.NumericUpDown edQcStartMin;
        private System.Windows.Forms.NumericUpDown edQcStartHour;
        private MetroFramework.Controls.MetroTile metroTile3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbAutoQc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbLoadingStop;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
    }
}