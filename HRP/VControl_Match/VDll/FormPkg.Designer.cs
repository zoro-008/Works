namespace VDll
{
    partial class FormPkg
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
            this.lvPkg = new System.Windows.Forms.ListView();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cbPkgType = new System.Windows.Forms.ComboBox();
            this.btInsert = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.btDel = new System.Windows.Forms.Button();
            this.btUp = new System.Windows.Forms.Button();
            this.btDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btRename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvPkg
            // 
            this.lvPkg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lvPkg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvPkg.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvPkg.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvPkg.FullRowSelect = true;
            this.lvPkg.Location = new System.Drawing.Point(0, 0);
            this.lvPkg.MultiSelect = false;
            this.lvPkg.Name = "lvPkg";
            this.lvPkg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lvPkg.Size = new System.Drawing.Size(325, 426);
            this.lvPkg.TabIndex = 5;
            this.lvPkg.UseCompatibleStateImageBehavior = false;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(413, 41);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(247, 21);
            this.tbName.TabIndex = 7;
            // 
            // cbPkgType
            // 
            this.cbPkgType.FormattingEnabled = true;
            this.cbPkgType.Location = new System.Drawing.Point(413, 68);
            this.cbPkgType.Name = "cbPkgType";
            this.cbPkgType.Size = new System.Drawing.Size(247, 20);
            this.cbPkgType.TabIndex = 8;
            // 
            // btInsert
            // 
            this.btInsert.Location = new System.Drawing.Point(412, 94);
            this.btInsert.Name = "btInsert";
            this.btInsert.Size = new System.Drawing.Size(61, 23);
            this.btInsert.TabIndex = 9;
            this.btInsert.Text = "Insert";
            this.btInsert.UseVisualStyleBackColor = true;
            this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(475, 94);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(61, 23);
            this.btAdd.TabIndex = 10;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btDel
            // 
            this.btDel.Location = new System.Drawing.Point(538, 94);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(61, 23);
            this.btDel.TabIndex = 11;
            this.btDel.Text = "Del";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // btUp
            // 
            this.btUp.Location = new System.Drawing.Point(413, 170);
            this.btUp.Name = "btUp";
            this.btUp.Size = new System.Drawing.Size(56, 40);
            this.btUp.TabIndex = 12;
            this.btUp.Text = "UP";
            this.btUp.UseVisualStyleBackColor = true;
            this.btUp.Click += new System.EventHandler(this.btUp_Click);
            // 
            // btDown
            // 
            this.btDown.Location = new System.Drawing.Point(413, 216);
            this.btDown.Name = "btDown";
            this.btDown.Size = new System.Drawing.Size(56, 40);
            this.btDown.TabIndex = 13;
            this.btDown.Text = "DOWN";
            this.btDown.UseVisualStyleBackColor = true;
            this.btDown.Click += new System.EventHandler(this.btDown_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(339, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "PKG Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(339, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "PKG Type";
            // 
            // btRename
            // 
            this.btRename.Location = new System.Drawing.Point(699, 331);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(61, 23);
            this.btRename.TabIndex = 16;
            this.btRename.Text = "Rename";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // FormPkg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 426);
            this.Controls.Add(this.btRename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvPkg);
            this.Controls.Add(this.btDown);
            this.Controls.Add(this.btUp);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btInsert);
            this.Controls.Add(this.cbPkgType);
            this.Controls.Add(this.tbName);
            this.Name = "FormPkg";
            this.Text = "FormPkg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvPkg;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ComboBox cbPkgType;
        private System.Windows.Forms.Button btInsert;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.Button btUp;
        private System.Windows.Forms.Button btDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btRename;
    }
}