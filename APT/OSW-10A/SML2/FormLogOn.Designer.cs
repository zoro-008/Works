namespace SML2
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
            this.btEsc = new System.Windows.Forms.Button();
            this.btChangePW = new System.Windows.Forms.Button();
            this.btEdit = new System.Windows.Forms.Button();
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
            this.label6 = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
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
            this.pnChange.Location = new System.Drawing.Point(2, 4);
            this.pnChange.Name = "pnChange";
            this.pnChange.Size = new System.Drawing.Size(330, 135);
            this.pnChange.TabIndex = 9;
            // 
            // btEnter
            // 
            this.btEnter.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEnter.Image = ((System.Drawing.Image)(resources.GetObject("btEnter.Image")));
            this.btEnter.Location = new System.Drawing.Point(213, 95);
            this.btEnter.Name = "btEnter";
            this.btEnter.Size = new System.Drawing.Size(105, 32);
            this.btEnter.TabIndex = 6;
            this.btEnter.Text = "Enter";
            this.btEnter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEnter.UseVisualStyleBackColor = true;
            this.btEnter.Click += new System.EventHandler(this.btEnter_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "New Password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Old Password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbNewPass
            // 
            this.tbNewPass.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbNewPass.Location = new System.Drawing.Point(122, 61);
            this.tbNewPass.Name = "tbNewPass";
            this.tbNewPass.Size = new System.Drawing.Size(200, 26);
            this.tbNewPass.TabIndex = 2;
            // 
            // tbOldPass
            // 
            this.tbOldPass.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbOldPass.Location = new System.Drawing.Point(122, 32);
            this.tbOldPass.Name = "tbOldPass";
            this.tbOldPass.Size = new System.Drawing.Size(200, 26);
            this.tbOldPass.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(-1, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(330, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "   PASSWORD Change";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btEsc
            // 
            this.btEsc.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEsc.Image = ((System.Drawing.Image)(resources.GetObject("btEsc.Image")));
            this.btEsc.Location = new System.Drawing.Point(223, 149);
            this.btEsc.Name = "btEsc";
            this.btEsc.Size = new System.Drawing.Size(111, 43);
            this.btEsc.TabIndex = 6;
            this.btEsc.Text = "Esc";
            this.btEsc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEsc.UseVisualStyleBackColor = true;
            this.btEsc.Click += new System.EventHandler(this.btEsc_Click);
            // 
            // btChangePW
            // 
            this.btChangePW.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btChangePW.Image = global::SML2.Properties.Resources.Diagram;
            this.btChangePW.Location = new System.Drawing.Point(112, 149);
            this.btChangePW.Name = "btChangePW";
            this.btChangePW.Size = new System.Drawing.Size(111, 43);
            this.btChangePW.TabIndex = 5;
            this.btChangePW.Text = "Change PW";
            this.btChangePW.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btChangePW.UseVisualStyleBackColor = true;
            this.btChangePW.Click += new System.EventHandler(this.btChangePW_Click);
            // 
            // btEdit
            // 
            this.btEdit.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEdit.Image = global::SML2.Properties.Resources.Edit112;
            this.btEdit.Location = new System.Drawing.Point(1, 149);
            this.btEdit.Name = "btEdit";
            this.btEdit.Size = new System.Drawing.Size(111, 43);
            this.btEdit.TabIndex = 4;
            this.btEdit.Text = "Edit ID";
            this.btEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEdit.UseVisualStyleBackColor = true;
            this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
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
            this.pnEdit.Location = new System.Drawing.Point(2, 4);
            this.pnEdit.Name = "pnEdit";
            this.pnEdit.Size = new System.Drawing.Size(330, 135);
            this.pnEdit.TabIndex = 16;
            // 
            // cbEditLevel
            // 
            this.cbEditLevel.FormattingEnabled = true;
            this.cbEditLevel.Items.AddRange(new object[] {
            "Operator",
            "Engineer",
            "Master"});
            this.cbEditLevel.Location = new System.Drawing.Point(94, 80);
            this.cbEditLevel.Name = "cbEditLevel";
            this.cbEditLevel.Size = new System.Drawing.Size(136, 20);
            this.cbEditLevel.TabIndex = 11;
            this.cbEditLevel.Text = "Operator";
            // 
            // btDeleteAll
            // 
            this.btDeleteAll.Location = new System.Drawing.Point(236, 104);
            this.btDeleteAll.Name = "btDeleteAll";
            this.btDeleteAll.Size = new System.Drawing.Size(87, 23);
            this.btDeleteAll.TabIndex = 10;
            this.btDeleteAll.Text = "Delete All ID";
            this.btDeleteAll.UseVisualStyleBackColor = true;
            this.btDeleteAll.Click += new System.EventHandler(this.btDeleteAll_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LightCoral;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(-1, -1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(330, 23);
            this.label4.TabIndex = 0;
            this.label4.Text = "   Edit";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbEditID
            // 
            this.tbEditID.Location = new System.Drawing.Point(94, 31);
            this.tbEditID.Name = "tbEditID";
            this.tbEditID.Size = new System.Drawing.Size(136, 21);
            this.tbEditID.TabIndex = 1;
            // 
            // tbEditPW
            // 
            this.tbEditPW.Location = new System.Drawing.Point(94, 55);
            this.tbEditPW.Name = "tbEditPW";
            this.tbEditPW.Size = new System.Drawing.Size(136, 21);
            this.tbEditPW.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Access Level";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "Password";
            // 
            // btEditNew
            // 
            this.btEditNew.Location = new System.Drawing.Point(236, 28);
            this.btEditNew.Name = "btEditNew";
            this.btEditNew.Size = new System.Drawing.Size(87, 23);
            this.btEditNew.TabIndex = 4;
            this.btEditNew.Text = "New";
            this.btEditNew.UseVisualStyleBackColor = true;
            this.btEditNew.Click += new System.EventHandler(this.btEditNew_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "ID";
            // 
            // btEditChange
            // 
            this.btEditChange.Location = new System.Drawing.Point(236, 54);
            this.btEditChange.Name = "btEditChange";
            this.btEditChange.Size = new System.Drawing.Size(87, 23);
            this.btEditChange.TabIndex = 5;
            this.btEditChange.Text = "Change";
            this.btEditChange.UseVisualStyleBackColor = true;
            this.btEditChange.Click += new System.EventHandler(this.btEditChange_Click);
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(236, 78);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(87, 23);
            this.btDelete.TabIndex = 6;
            this.btDelete.Text = "Delete";
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
            this.pnInput.Controls.Add(this.label6);
            this.pnInput.Location = new System.Drawing.Point(2, 4);
            this.pnInput.Name = "pnInput";
            this.pnInput.Size = new System.Drawing.Size(330, 135);
            this.pnInput.TabIndex = 17;
            // 
            // btLogIn
            // 
            this.btLogIn.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btLogIn.Image = global::SML2.Properties.Resources.onebit_18;
            this.btLogIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btLogIn.Location = new System.Drawing.Point(188, 42);
            this.btLogIn.Name = "btLogIn";
            this.btLogIn.Size = new System.Drawing.Size(135, 66);
            this.btLogIn.TabIndex = 3;
            this.btLogIn.Text = "LogIn";
            this.btLogIn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLogIn.UseVisualStyleBackColor = true;
            this.btLogIn.Click += new System.EventHandler(this.btLogIn_Click);
            // 
            // tbID
            // 
            this.tbID.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbID.Location = new System.Drawing.Point(9, 43);
            this.tbID.Name = "tbID";
            this.tbID.Size = new System.Drawing.Size(172, 29);
            this.tbID.TabIndex = 1;
            this.tbID.Text = "ID";
            this.tbID.Click += new System.EventHandler(this.tbID_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbPassword.Location = new System.Drawing.Point(9, 76);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(172, 29);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.Text = "Password";
            this.tbPassword.Click += new System.EventHandler(this.tbPassword_Click);
            this.tbPassword.CursorChanged += new System.EventHandler(this.tbPassword_CursorChanged);
            this.tbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.LightCoral;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Location = new System.Drawing.Point(-1, -1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(330, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "   LogOn";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FormLogOn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.ClientSize = new System.Drawing.Size(334, 200);
            this.Controls.Add(this.btEsc);
            this.Controls.Add(this.btChangePW);
            this.Controls.Add(this.btEdit);
            this.Controls.Add(this.pnInput);
            this.Controls.Add(this.pnEdit);
            this.Controls.Add(this.pnChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogOn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormLogOn";
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer tmUpdate;
    }
}