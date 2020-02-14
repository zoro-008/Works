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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.LsrSttPos = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.LsrMaxDelay = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.edLaserCheck = new System.Windows.Forms.NumericUpDown();
            this.edMagnetCheck = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cbIgnrDoor = new System.Windows.Forms.CheckBox();
            this.label34 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.tbEquipName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbEquipSerial = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.cbIgnrSptr = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LsrSttPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LsrMaxDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edLaserCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edMagnetCheck)).BeginInit();
            this.panel7.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel7);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.LsrSttPos);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.LsrMaxDelay);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label40);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label39);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.edLaserCheck);
            this.panel2.Controls.Add(this.edMagnetCheck);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label2);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // LsrSttPos
            // 
            this.LsrSttPos.DecimalPlaces = 1;
            resources.ApplyResources(this.LsrSttPos, "LsrSttPos");
            this.LsrSttPos.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.LsrSttPos.Name = "LsrSttPos";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // LsrMaxDelay
            // 
            resources.ApplyResources(this.LsrMaxDelay, "LsrMaxDelay");
            this.LsrMaxDelay.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.LsrMaxDelay.Name = "LsrMaxDelay";
            this.LsrMaxDelay.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.LightCoral;
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // edLaserCheck
            // 
            resources.ApplyResources(this.edLaserCheck, "edLaserCheck");
            this.edLaserCheck.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.edLaserCheck.Name = "edLaserCheck";
            // 
            // edMagnetCheck
            // 
            resources.ApplyResources(this.edMagnetCheck, "edMagnetCheck");
            this.edMagnetCheck.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.edMagnetCheck.Name = "edMagnetCheck";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.cbIgnrDoor);
            this.panel7.Controls.Add(this.label34);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // cbIgnrDoor
            // 
            resources.ApplyResources(this.cbIgnrDoor, "cbIgnrDoor");
            this.cbIgnrDoor.Name = "cbIgnrDoor";
            this.cbIgnrDoor.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.LightCoral;
            this.label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.panel1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.cbIgnrSptr);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbEquipName);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tbEquipSerial);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tbEquipName
            // 
            resources.ApplyResources(this.tbEquipName, "tbEquipName");
            this.tbEquipName.Name = "tbEquipName";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LightCoral;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // tbEquipSerial
            // 
            resources.ApplyResources(this.tbEquipSerial, "tbEquipSerial");
            this.tbEquipSerial.Name = "tbEquipSerial";
            // 
            // btSave
            // 
            resources.ApplyResources(this.btSave, "btSave");
            this.btSave.Name = "btSave";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // cbIgnrSptr
            // 
            resources.ApplyResources(this.cbIgnrSptr, "cbIgnrSptr");
            this.cbIgnrSptr.Name = "cbIgnrSptr";
            this.cbIgnrSptr.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // FormOption
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOption_VisibleChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LsrSttPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LsrMaxDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edLaserCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edMagnetCheck)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cbIgnrDoor;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbEquipName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbEquipSerial;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.NumericUpDown LsrMaxDelay;
        private System.Windows.Forms.NumericUpDown edMagnetCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown edLaserCheck;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown LsrSttPos;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cbIgnrSptr;
        private System.Windows.Forms.Label label12;
    }
}