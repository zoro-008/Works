namespace SML
{
    partial class FormLogOn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogOn));
            this.pnChange = new System.Windows.Forms.Panel();
            this.btEnter = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNewPass = new System.Windows.Forms.TextBox();
            this.tbOldPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnEdit = new System.Windows.Forms.Panel();
            this.cbEditLevel = new System.Windows.Forms.ComboBox();
            this.btDeleteAll = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbEditID = new System.Windows.Forms.TextBox();
            this.tbEditPW = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btEditNew = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.btEditChange = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.pnInput = new System.Windows.Forms.Panel();
            this.btLogIn = new System.Windows.Forms.Button();
            this.tbID = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lbLogOn = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.btEsc = new System.Windows.Forms.Button();
            this.btChangePW = new System.Windows.Forms.Button();
            this.btEdit = new System.Windows.Forms.Button();
            this.pnChange.SuspendLayout();
            this.pnEdit.SuspendLayout();
            this.pnInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnChange
            // 
            this.pnChange.BackColor = System.Drawing.SystemColors.Control;
            this.pnChange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnChange.Controls.Add(this.btEnter);
            this.pnChange.Controls.Add(this.label3);
            this.pnChange.Controls.Add(this.label2);
            this.pnChange.Controls.Add(this.tbNewPass);
            this.pnChange.Controls.Add(this.tbOldPass);
            this.pnChange.Controls.Add(this.label1);
            resources.ApplyResources(this.pnChange, "pnChange");
            this.pnChange.Name = "pnChange";
            // 
            // btEnter
            // 
            resources.ApplyResources(this.btEnter, "btEnter");
            this.btEnter.Name = "btEnter";
            this.btEnter.UseVisualStyleBackColor = true;
            this.btEnter.Click += new System.EventHandler(this.btEnter_Click);
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
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Name = "label1";
            // 
            // pnEdit
            // 
            this.pnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.pnEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnEdit.Controls.Add(this.cbEditLevel);
            this.pnEdit.Controls.Add(this.btDeleteAll);
            this.pnEdit.Controls.Add(this.label4);
            this.pnEdit.Controls.Add(this.tbEditID);
            this.pnEdit.Controls.Add(this.tbEditPW);
            this.pnEdit.Controls.Add(this.label5);
            this.pnEdit.Controls.Add(this.label7);
            this.pnEdit.Controls.Add(this.btEditNew);
            this.pnEdit.Controls.Add(this.label8);
            this.pnEdit.Controls.Add(this.btEditChange);
            this.pnEdit.Controls.Add(this.btDelete);
            resources.ApplyResources(this.pnEdit, "pnEdit");
            this.pnEdit.Name = "pnEdit";
            // 
            // cbEditLevel
            // 
            this.cbEditLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbEditLevel, "cbEditLevel");
            this.cbEditLevel.FormattingEnabled = true;
            this.cbEditLevel.Items.AddRange(new object[] {
            resources.GetString("cbEditLevel.Items"),
            resources.GetString("cbEditLevel.Items1"),
            resources.GetString("cbEditLevel.Items2")});
            this.cbEditLevel.Name = "cbEditLevel";
            // 
            // btDeleteAll
            // 
            resources.ApplyResources(this.btDeleteAll, "btDeleteAll");
            this.btDeleteAll.Name = "btDeleteAll";
            this.btDeleteAll.UseVisualStyleBackColor = true;
            this.btDeleteAll.Click += new System.EventHandler(this.btDeleteAll_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LightCoral;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Name = "label4";
            // 
            // tbEditID
            // 
            resources.ApplyResources(this.tbEditID, "tbEditID");
            this.tbEditID.Name = "tbEditID";
            // 
            // tbEditPW
            // 
            resources.ApplyResources(this.tbEditPW, "tbEditPW");
            this.tbEditPW.Name = "tbEditPW";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // btEditNew
            // 
            resources.ApplyResources(this.btEditNew, "btEditNew");
            this.btEditNew.Name = "btEditNew";
            this.btEditNew.UseVisualStyleBackColor = true;
            this.btEditNew.Click += new System.EventHandler(this.btEditNew_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // btEditChange
            // 
            resources.ApplyResources(this.btEditChange, "btEditChange");
            this.btEditChange.Name = "btEditChange";
            this.btEditChange.UseVisualStyleBackColor = true;
            this.btEditChange.Click += new System.EventHandler(this.btEditChange_Click);
            // 
            // btDelete
            // 
            resources.ApplyResources(this.btDelete, "btDelete");
            this.btDelete.Name = "btDelete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // pnInput
            // 
            this.pnInput.BackColor = System.Drawing.SystemColors.Control;
            this.pnInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnInput.Controls.Add(this.btLogIn);
            this.pnInput.Controls.Add(this.tbID);
            this.pnInput.Controls.Add(this.tbPassword);
            this.pnInput.Controls.Add(this.lbLogOn);
            resources.ApplyResources(this.pnInput, "pnInput");
            this.pnInput.Name = "pnInput";
            // 
            // btLogIn
            // 
            resources.ApplyResources(this.btLogIn, "btLogIn");
            this.btLogIn.Image = global::SML.Properties.Resources.Person;
            this.btLogIn.Name = "btLogIn";
            this.btLogIn.UseVisualStyleBackColor = true;
            this.btLogIn.Click += new System.EventHandler(this.btLogIn_Click);
            // 
            // tbID
            // 
            resources.ApplyResources(this.tbID, "tbID");
            this.tbID.Name = "tbID";
            this.tbID.Click += new System.EventHandler(this.tbID_Click);
            // 
            // tbPassword
            // 
            resources.ApplyResources(this.tbPassword, "tbPassword");
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Click += new System.EventHandler(this.tbPassword_Click);
            this.tbPassword.CursorChanged += new System.EventHandler(this.tbPassword_CursorChanged);
            this.tbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);
            // 
            // lbLogOn
            // 
            this.lbLogOn.BackColor = System.Drawing.Color.LightCoral;
            this.lbLogOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbLogOn, "lbLogOn");
            this.lbLogOn.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lbLogOn.Name = "lbLogOn";
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // btEsc
            // 
            resources.ApplyResources(this.btEsc, "btEsc");
            this.btEsc.Name = "btEsc";
            this.btEsc.UseVisualStyleBackColor = true;
            this.btEsc.Click += new System.EventHandler(this.btEsc_Click);
            // 
            // btChangePW
            // 
            resources.ApplyResources(this.btChangePW, "btChangePW");
            this.btChangePW.Image = global::SML.Properties.Resources.Diagram;
            this.btChangePW.Name = "btChangePW";
            this.btChangePW.UseVisualStyleBackColor = true;
            this.btChangePW.Click += new System.EventHandler(this.btChangePW_Click);
            // 
            // btEdit
            // 
            resources.ApplyResources(this.btEdit, "btEdit");
            this.btEdit.Name = "btEdit";
            this.btEdit.UseVisualStyleBackColor = true;
            this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
            // 
            // FormLogOn
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.btEsc);
            this.Controls.Add(this.btChangePW);
            this.Controls.Add(this.btEdit);
            this.Controls.Add(this.pnInput);
            this.Controls.Add(this.pnEdit);
            this.Controls.Add(this.pnChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogOn";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormLogOn_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormLogOn_VisibleChanged);
            this.pnChange.ResumeLayout(false);
            this.pnChange.PerformLayout();
            this.pnEdit.ResumeLayout(false);
            this.pnEdit.PerformLayout();
            this.pnInput.ResumeLayout(false);
            this.pnInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btEsc;
        private System.Windows.Forms.Button btChangePW;
        private System.Windows.Forms.Button btEdit;
        private System.Windows.Forms.Panel pnChange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNewPass;
        private System.Windows.Forms.TextBox tbOldPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btEnter;
        private System.Windows.Forms.Panel pnEdit;
        private System.Windows.Forms.ComboBox cbEditLevel;
        private System.Windows.Forms.Button btDeleteAll;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbEditID;
        private System.Windows.Forms.TextBox tbEditPW;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btEditNew;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btEditChange;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Panel pnInput;
        private System.Windows.Forms.Button btLogIn;
        private System.Windows.Forms.TextBox tbID;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lbLogOn;
        private System.Windows.Forms.Timer tmUpdate;
    }
}