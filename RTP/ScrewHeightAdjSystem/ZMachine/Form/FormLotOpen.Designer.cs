﻿namespace Machine
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbWorkCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLotNo = new System.Windows.Forms.TextBox();
            this.tbSelDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btLotOpen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btRework = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbWorkCount);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbLotNo);
            this.panel1.Controls.Add(this.tbSelDevice);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Name = "panel1";
            // 
            // tbWorkCount
            // 
            this.tbWorkCount.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tbWorkCount, "tbWorkCount");
            this.tbWorkCount.Name = "tbWorkCount";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tbLotNo
            // 
            this.tbLotNo.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tbLotNo, "tbLotNo");
            this.tbLotNo.Name = "tbLotNo";
            this.tbLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLotNo_KeyDown);
            // 
            // tbSelDevice
            // 
            this.tbSelDevice.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.tbSelDevice, "tbSelDevice");
            this.tbSelDevice.Name = "tbSelDevice";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // btRework
            // 
            resources.ApplyResources(this.btRework, "btRework");
            this.btRework.Image = global::Machine.Properties.Resources.Oper;
            this.btRework.Name = "btRework";
            this.btRework.UseVisualStyleBackColor = true;
            this.btRework.Click += new System.EventHandler(this.btRework_Click);
            // 
            // FormLotOpen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.btRework);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btLotOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLotOpen";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormLotOpen_Load);
            this.Shown += new System.EventHandler(this.FormLotOpen_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormLotOpen_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.TextBox tbWorkCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btRework;
    }
}