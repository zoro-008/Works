namespace SML
{
    partial class FormPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPassword));
            this.pnChange = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNewPass = new System.Windows.Forms.TextBox();
            this.tbOldPass = new System.Windows.Forms.TextBox();
            this.pnInput = new System.Windows.Forms.Panel();
            this.lbInputPass = new System.Windows.Forms.Label();
            this.tbInputPass = new System.Windows.Forms.TextBox();
            this.btEsc = new System.Windows.Forms.Button();
            this.btEnter = new System.Windows.Forms.Button();
            this.btChange = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnChange.SuspendLayout();
            this.pnInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnChange
            // 
            resources.ApplyResources(this.pnChange, "pnChange");
            this.pnChange.BackColor = System.Drawing.SystemColors.Control;
            this.pnChange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnChange.Controls.Add(this.label3);
            this.pnChange.Controls.Add(this.label2);
            this.pnChange.Controls.Add(this.tbNewPass);
            this.pnChange.Controls.Add(this.tbOldPass);
            this.pnChange.Controls.Add(this.label1);
            this.pnChange.Name = "pnChange";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Name = "label2";
            // 
            // tbNewPass
            // 
            resources.ApplyResources(this.tbNewPass, "tbNewPass");
            this.tbNewPass.Name = "tbNewPass";
            // 
            // tbOldPass
            // 
            resources.ApplyResources(this.tbOldPass, "tbOldPass");
            this.tbOldPass.Name = "tbOldPass";
            // 
            // pnInput
            // 
            resources.ApplyResources(this.pnInput, "pnInput");
            this.pnInput.BackColor = System.Drawing.SystemColors.Control;
            this.pnInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnInput.Controls.Add(this.lbInputPass);
            this.pnInput.Controls.Add(this.tbInputPass);
            this.pnInput.Controls.Add(this.label6);
            this.pnInput.Name = "pnInput";
            // 
            // lbInputPass
            // 
            resources.ApplyResources(this.lbInputPass, "lbInputPass");
            this.lbInputPass.ForeColor = System.Drawing.Color.Navy;
            this.lbInputPass.Name = "lbInputPass";
            // 
            // tbInputPass
            // 
            resources.ApplyResources(this.tbInputPass, "tbInputPass");
            this.tbInputPass.Name = "tbInputPass";
            this.tbInputPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbInputPass_KeyDown);
            // 
            // btEsc
            // 
            resources.ApplyResources(this.btEsc, "btEsc");
            this.btEsc.Name = "btEsc";
            this.btEsc.UseVisualStyleBackColor = true;
            this.btEsc.Click += new System.EventHandler(this.btEsc_Click);
            // 
            // btEnter
            // 
            resources.ApplyResources(this.btEnter, "btEnter");
            this.btEnter.Name = "btEnter";
            this.btEnter.UseVisualStyleBackColor = true;
            this.btEnter.Click += new System.EventHandler(this.btEnter_Click);
            // 
            // btChange
            // 
            resources.ApplyResources(this.btChange, "btChange");
            this.btChange.Name = "btChange";
            this.btChange.UseVisualStyleBackColor = true;
            this.btChange.Click += new System.EventHandler(this.btChange_Click);
            this.btChange.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btChange_MouseDown);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Name = "label6";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Name = "label1";
            // 
            // FormPassword
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.btEsc);
            this.Controls.Add(this.btEnter);
            this.Controls.Add(this.btChange);
            this.Controls.Add(this.pnInput);
            this.Controls.Add(this.pnChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPassword";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormPassword_Shown);
            this.pnChange.ResumeLayout(false);
            this.pnChange.PerformLayout();
            this.pnInput.ResumeLayout(false);
            this.pnInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnChange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNewPass;
        private System.Windows.Forms.TextBox tbOldPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btChange;
        private System.Windows.Forms.Button btEnter;
        private System.Windows.Forms.Button btEsc;
        private System.Windows.Forms.Panel pnInput;
        private System.Windows.Forms.Label lbInputPass;
        private System.Windows.Forms.TextBox tbInputPass;
        private System.Windows.Forms.Label label6;
    }
}