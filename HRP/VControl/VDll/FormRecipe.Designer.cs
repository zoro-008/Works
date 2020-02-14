namespace VDll
{
    partial class FormRecipe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecipe));
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCrntDevice = new System.Windows.Forms.Label();
            this.lbSelDevice = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btDownload = new System.Windows.Forms.Button();
            this.tbToName = new System.Windows.Forms.TextBox();
            this.tbFromName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btCopy = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btNew = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lvDevice = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(784, 354);
            this.panel4.TabIndex = 29;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lbCrntDevice);
            this.panel2.Controls.Add(this.lbSelDevice);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btDownload);
            this.panel2.Controls.Add(this.tbToName);
            this.panel2.Controls.Add(this.tbFromName);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.btCopy);
            this.panel2.Controls.Add(this.btRename);
            this.panel2.Controls.Add(this.btDelete);
            this.panel2.Controls.Add(this.btNew);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel2.Location = new System.Drawing.Point(416, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(368, 354);
            this.panel2.TabIndex = 26;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.DimGray;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label8.Image = ((System.Drawing.Image)(resources.GetObject("label8.Image")));
            this.label8.Location = new System.Drawing.Point(3, 200);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(360, 28);
            this.label8.TabIndex = 25;
            this.label8.Text = "RECIPE DOWNLOAD";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DimGray;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(360, 28);
            this.label2.TabIndex = 24;
            this.label2.Text = "RECIPE EDIT";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCrntDevice
            // 
            this.lbCrntDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCrntDevice.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbCrntDevice.Location = new System.Drawing.Point(127, 238);
            this.lbCrntDevice.Name = "lbCrntDevice";
            this.lbCrntDevice.Size = new System.Drawing.Size(236, 24);
            this.lbCrntDevice.TabIndex = 23;
            this.lbCrntDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSelDevice
            // 
            this.lbSelDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbSelDevice.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbSelDevice.Location = new System.Drawing.Point(127, 271);
            this.lbSelDevice.Name = "lbSelDevice";
            this.lbSelDevice.Size = new System.Drawing.Size(236, 24);
            this.lbSelDevice.TabIndex = 22;
            this.lbSelDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.label5.Location = new System.Drawing.Point(1, 274);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 19);
            this.label5.TabIndex = 21;
            this.label5.Text = "SELECTED RECIPE";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.label3.Location = new System.Drawing.Point(1, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 19);
            this.label3.TabIndex = 20;
            this.label3.Text = "CURRENT RECIPE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btDownload
            // 
            this.btDownload.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btDownload.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btDownload.Image = ((System.Drawing.Image)(resources.GetObject("btDownload.Image")));
            this.btDownload.Location = new System.Drawing.Point(3, 304);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(360, 45);
            this.btDownload.TabIndex = 19;
            this.btDownload.Text = "DOWNLOAD";
            this.btDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // tbToName
            // 
            this.tbToName.BackColor = System.Drawing.SystemColors.Window;
            this.tbToName.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.tbToName.Location = new System.Drawing.Point(127, 75);
            this.tbToName.Multiline = true;
            this.tbToName.Name = "tbToName";
            this.tbToName.Size = new System.Drawing.Size(235, 24);
            this.tbToName.TabIndex = 16;
            // 
            // tbFromName
            // 
            this.tbFromName.BackColor = System.Drawing.Color.Silver;
            this.tbFromName.Enabled = false;
            this.tbFromName.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.tbFromName.Location = new System.Drawing.Point(128, 40);
            this.tbFromName.Multiline = true;
            this.tbFromName.Name = "tbFromName";
            this.tbFromName.Size = new System.Drawing.Size(235, 24);
            this.tbFromName.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.label7.Location = new System.Drawing.Point(3, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 24);
            this.label7.TabIndex = 14;
            this.label7.Text = "TO NAME";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.label9.Location = new System.Drawing.Point(3, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 24);
            this.label9.TabIndex = 13;
            this.label9.Text = "FROM NAME";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btCopy
            // 
            this.btCopy.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btCopy.Image = ((System.Drawing.Image)(resources.GetObject("btCopy.Image")));
            this.btCopy.Location = new System.Drawing.Point(188, 116);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(175, 35);
            this.btCopy.TabIndex = 12;
            this.btCopy.Tag = "";
            this.btCopy.Text = "COPY";
            this.btCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btRename
            // 
            this.btRename.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btRename.Image = ((System.Drawing.Image)(resources.GetObject("btRename.Image")));
            this.btRename.Location = new System.Drawing.Point(7, 155);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(175, 35);
            this.btRename.TabIndex = 11;
            this.btRename.Tag = "";
            this.btRename.Text = "RENAME";
            this.btRename.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btRename.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            this.btDelete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btDelete.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.Image")));
            this.btDelete.Location = new System.Drawing.Point(188, 155);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(175, 35);
            this.btDelete.TabIndex = 10;
            this.btDelete.Tag = "";
            this.btDelete.Text = "DELETE";
            this.btDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btNew
            // 
            this.btNew.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btNew.Image = ((System.Drawing.Image)(resources.GetObject("btNew.Image")));
            this.btNew.Location = new System.Drawing.Point(7, 116);
            this.btNew.Name = "btNew";
            this.btNew.Size = new System.Drawing.Size(175, 35);
            this.btNew.TabIndex = 5;
            this.btNew.Tag = "";
            this.btNew.Text = "NEW";
            this.btNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btNew.UseVisualStyleBackColor = true;
            this.btNew.Click += new System.EventHandler(this.btNew_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lvDevice);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3);
            this.panel3.Size = new System.Drawing.Size(416, 354);
            this.panel3.TabIndex = 25;
            // 
            // lvDevice
            // 
            this.lvDevice.BackColor = System.Drawing.SystemColors.Window;
            this.lvDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDevice.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lvDevice.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDevice.Location = new System.Drawing.Point(3, 31);
            this.lvDevice.Name = "lvDevice";
            this.lvDevice.Size = new System.Drawing.Size(408, 318);
            this.lvDevice.TabIndex = 1;
            this.lvDevice.UseCompatibleStateImageBehavior = false;
            this.lvDevice.SelectedIndexChanged += new System.EventHandler(this.lvDevice_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DimGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(408, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "RECIPE LIST";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 354);
            this.Controls.Add(this.panel4);
            this.MaximumSize = new System.Drawing.Size(1280, 1024);
            this.MinimumSize = new System.Drawing.Size(800, 0);
            this.Name = "FormRecipe";
            this.Text = "FormRecipe";
            this.VisibleChanged += new System.EventHandler(this.FormRecipe_VisibleChanged);
            this.Resize += new System.EventHandler(this.FormRecipe_Resize);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCrntDevice;
        private System.Windows.Forms.Label lbSelDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.TextBox tbToName;
        private System.Windows.Forms.TextBox tbFromName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btNew;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lvDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
    }
}