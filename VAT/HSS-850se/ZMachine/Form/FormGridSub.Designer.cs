namespace Machine
{
    partial class FormGridSub
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btDel = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.btDeleteLine = new System.Windows.Forms.Button();
            this.btSaveAs = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.gvPara = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPara)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btDel);
            this.panel2.Controls.Add(this.btAdd);
            this.panel2.Controls.Add(this.btClear);
            this.panel2.Controls.Add(this.btDeleteLine);
            this.panel2.Controls.Add(this.btSaveAs);
            this.panel2.Controls.Add(this.btSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 551);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1256, 27);
            this.panel2.TabIndex = 142;
            // 
            // btDel
            // 
            this.btDel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btDel.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btDel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDel.Location = new System.Drawing.Point(446, 0);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(135, 27);
            this.btDel.TabIndex = 147;
            this.btDel.Text = "삭제";
            this.btDel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // btAdd
            // 
            this.btAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btAdd.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAdd.Location = new System.Drawing.Point(581, 0);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(135, 27);
            this.btAdd.TabIndex = 146;
            this.btAdd.Text = "추가";
            this.btAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btClear
            // 
            this.btClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.btClear.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btClear.Location = new System.Drawing.Point(716, 0);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(135, 27);
            this.btClear.TabIndex = 144;
            this.btClear.Text = "전체 클리어";
            this.btClear.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btDeleteLine
            // 
            this.btDeleteLine.Dock = System.Windows.Forms.DockStyle.Right;
            this.btDeleteLine.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btDeleteLine.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDeleteLine.Location = new System.Drawing.Point(851, 0);
            this.btDeleteLine.Name = "btDeleteLine";
            this.btDeleteLine.Size = new System.Drawing.Size(135, 27);
            this.btDeleteLine.TabIndex = 142;
            this.btDeleteLine.Text = "마지막 라인 삭제";
            this.btDeleteLine.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btDeleteLine.UseVisualStyleBackColor = true;
            this.btDeleteLine.Click += new System.EventHandler(this.btDeleteLine_Click);
            // 
            // btSaveAs
            // 
            this.btSaveAs.Dock = System.Windows.Forms.DockStyle.Right;
            this.btSaveAs.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btSaveAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btSaveAs.Location = new System.Drawing.Point(986, 0);
            this.btSaveAs.Name = "btSaveAs";
            this.btSaveAs.Size = new System.Drawing.Size(135, 27);
            this.btSaveAs.TabIndex = 143;
            this.btSaveAs.Text = "다른이름으로 저장";
            this.btSaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btSaveAs.UseVisualStyleBackColor = true;
            this.btSaveAs.Click += new System.EventHandler(this.btSaveAs_Click);
            // 
            // btSave
            // 
            this.btSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btSave.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btSave.Location = new System.Drawing.Point(1121, 0);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(135, 27);
            this.btSave.TabIndex = 138;
            this.btSave.Text = "저장";
            this.btSave.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.Dock = System.Windows.Forms.DockStyle.Right;
            this.btLoad.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btLoad.Location = new System.Drawing.Point(1140, 0);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(114, 23);
            this.btLoad.TabIndex = 143;
            this.btLoad.Text = "열기";
            this.btLoad.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // gvPara
            // 
            this.gvPara.AllowUserToResizeColumns = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gvPara.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gvPara.BackgroundColor = System.Drawing.Color.White;
            this.gvPara.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvPara.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gvPara.ColumnHeadersHeight = 60;
            this.gvPara.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvPara.DefaultCellStyle = dataGridViewCellStyle3;
            this.gvPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvPara.EnableHeadersVisualStyles = false;
            this.gvPara.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.gvPara.Location = new System.Drawing.Point(0, 25);
            this.gvPara.Name = "gvPara";
            this.gvPara.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvPara.RowTemplate.Height = 23;
            this.gvPara.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gvPara.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvPara.Size = new System.Drawing.Size(1256, 526);
            this.gvPara.TabIndex = 143;
            this.gvPara.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvPara_CellValueChanged);
            this.gvPara.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.gvPara_ColumnWidthChanged);
            this.gvPara.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gvPara_RowPostPaint);
            this.gvPara.Scroll += new System.Windows.Forms.ScrollEventHandler(this.gvPara_Scroll);
            this.gvPara.Paint += new System.Windows.Forms.PaintEventHandler(this.gvPara_Paint);
            this.gvPara.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvPara_KeyDown);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gvPara);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1256, 551);
            this.panel3.TabIndex = 144;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbName);
            this.panel1.Controls.Add(this.label49);
            this.panel1.Controls.Add(this.btLoad);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1256, 25);
            this.panel1.TabIndex = 144;
            // 
            // tbName
            // 
            this.tbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbName.Enabled = false;
            this.tbName.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.tbName.Location = new System.Drawing.Point(194, 0);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(946, 29);
            this.tbName.TabIndex = 44;
            // 
            // label49
            // 
            this.label49.BackColor = System.Drawing.Color.LightCoral;
            this.label49.Dock = System.Windows.Forms.DockStyle.Left;
            this.label49.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label49.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label49.Location = new System.Drawing.Point(0, 0);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(194, 23);
            this.label49.TabIndex = 45;
            this.label49.Text = "RECIPE SELECTION";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormGridSub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 578);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGridSub";
            this.Text = "Col";
            this.VisibleChanged += new System.EventHandler(this.FormGridSub_VisibleChanged);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPara)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btDeleteLine;
        private System.Windows.Forms.DataGridView gvPara;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Button btSaveAs;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.Button btAdd;
    }
}