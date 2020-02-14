namespace Machine
{
    partial class FormOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.cb = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tbEquipSerial = new System.Windows.Forms.TextBox();
            this.tbEquipName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cbLoadingStop = new System.Windows.Forms.CheckBox();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.label81 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.tbFindBoltWork1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFindBoltWork2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFindBoltWork3 = new System.Windows.Forms.NumericUpDown();
            this.cb.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork3)).BeginInit();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // cb
            // 
            resources.ApplyResources(this.cb, "cb");
            this.cb.BackColor = System.Drawing.SystemColors.Control;
            this.cb.Controls.Add(this.panel8);
            this.cb.Controls.Add(this.panel6);
            this.cb.Name = "cb";
            // 
            // panel8
            // 
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.tbEquipSerial);
            this.panel8.Controls.Add(this.tbEquipName);
            this.panel8.Controls.Add(this.label5);
            this.panel8.Controls.Add(this.label1);
            this.panel8.Controls.Add(this.label80);
            this.panel8.Name = "panel8";
            // 
            // tbEquipSerial
            // 
            resources.ApplyResources(this.tbEquipSerial, "tbEquipSerial");
            this.tbEquipSerial.Name = "tbEquipSerial";
            // 
            // tbEquipName
            // 
            resources.ApplyResources(this.tbEquipName, "tbEquipName");
            this.tbEquipName.Name = "tbEquipName";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Name = "label5";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Name = "label1";
            // 
            // label80
            // 
            resources.ApplyResources(this.label80, "label80");
            this.label80.BackColor = System.Drawing.Color.LightCoral;
            this.label80.Name = "label80";
            // 
            // panel6
            // 
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.tbFindBoltWork3);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.tbFindBoltWork2);
            this.panel6.Controls.Add(this.label81);
            this.panel6.Controls.Add(this.label74);
            this.panel6.Controls.Add(this.tbFindBoltWork1);
            this.panel6.Controls.Add(this.cbLoadingStop);
            this.panel6.Controls.Add(this.cbIgnrDoor);
            this.panel6.Controls.Add(this.label27);
            this.panel6.Name = "panel6";
            // 
            // cbLoadingStop
            // 
            resources.ApplyResources(this.cbLoadingStop, "cbLoadingStop");
            this.cbLoadingStop.ForeColor = System.Drawing.Color.Black;
            this.cbLoadingStop.Name = "cbLoadingStop";
            this.cbLoadingStop.UseVisualStyleBackColor = true;
            // 
            // cbIgnrDoor
            // 
            resources.ApplyResources(this.cbIgnrDoor, "cbIgnrDoor");
            this.cbIgnrDoor.ForeColor = System.Drawing.Color.Black;
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.BackColor = System.Drawing.Color.LightCoral;
            this.label27.Name = "label27";
            // 
            // btSave
            // 
            resources.ApplyResources(this.btSave, "btSave");
            this.btSave.Name = "btSave";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label81
            // 
            resources.ApplyResources(this.label81, "label81");
            this.label81.ForeColor = System.Drawing.Color.Black;
            this.label81.Name = "label81";
            // 
            // label74
            // 
            resources.ApplyResources(this.label74, "label74");
            this.label74.ForeColor = System.Drawing.Color.Black;
            this.label74.Name = "label74";
            // 
            // tbFindBoltWork1
            // 
            resources.ApplyResources(this.tbFindBoltWork1, "tbFindBoltWork1");
            this.tbFindBoltWork1.DecimalPlaces = 1;
            this.tbFindBoltWork1.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.tbFindBoltWork1.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.tbFindBoltWork1.Name = "tbFindBoltWork1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Name = "label3";
            // 
            // tbFindBoltWork2
            // 
            resources.ApplyResources(this.tbFindBoltWork2, "tbFindBoltWork2");
            this.tbFindBoltWork2.DecimalPlaces = 1;
            this.tbFindBoltWork2.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.tbFindBoltWork2.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.tbFindBoltWork2.Name = "tbFindBoltWork2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Name = "label6";
            // 
            // tbFindBoltWork3
            // 
            resources.ApplyResources(this.tbFindBoltWork3, "tbFindBoltWork3");
            this.tbFindBoltWork3.DecimalPlaces = 1;
            this.tbFindBoltWork3.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.tbFindBoltWork3.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.tbFindBoltWork3.Name = "tbFindBoltWork3";
            // 
            // FormOption
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.cb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOption_VisibleChanged);
            this.cb.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFindBoltWork3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel cb;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbEquipSerial;
        private System.Windows.Forms.TextBox tbEquipName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.CheckBox cbLoadingStop;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown tbFindBoltWork3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown tbFindBoltWork2;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.NumericUpDown tbFindBoltWork1;
    }
}