namespace Machine
{
    partial class FormGrid
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
            this.btDeleteLine = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.gvPara = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPara)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.btDeleteLine);
            this.panel2.Controls.Add(this.btSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 319);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 27);
            this.panel2.TabIndex = 142;
            this.panel2.Visible = false;
            // 
            // btDeleteLine
            // 
            this.btDeleteLine.Dock = System.Windows.Forms.DockStyle.Right;
            this.btDeleteLine.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btDeleteLine.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDeleteLine.Location = new System.Drawing.Point(408, 0);
            this.btDeleteLine.Name = "btDeleteLine";
            this.btDeleteLine.Size = new System.Drawing.Size(114, 27);
            this.btDeleteLine.TabIndex = 142;
            this.btDeleteLine.Text = "DELETE LINE";
            this.btDeleteLine.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btDeleteLine.UseVisualStyleBackColor = true;
            this.btDeleteLine.Click += new System.EventHandler(this.btDeleteLine_Click);
            // 
            // btSave
            // 
            this.btSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btSave.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btSave.Location = new System.Drawing.Point(522, 0);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(114, 27);
            this.btSave.TabIndex = 138;
            this.btSave.Text = "DATA SAVE";
            this.btSave.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
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
            this.gvPara.Location = new System.Drawing.Point(0, 0);
            this.gvPara.Name = "gvPara";
            this.gvPara.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvPara.RowTemplate.Height = 23;
            this.gvPara.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gvPara.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvPara.Size = new System.Drawing.Size(636, 319);
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
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(636, 319);
            this.panel3.TabIndex = 144;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(252, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 24);
            this.button1.TabIndex = 143;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 346);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGrid";
            this.Text = "Col";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPara)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btDeleteLine;
        private System.Windows.Forms.DataGridView gvPara;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button button1;
    }
}