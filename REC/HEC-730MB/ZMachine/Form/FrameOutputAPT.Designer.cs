namespace Machine
{
    partial class FrameOutputAPT
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
            this.lbOutput = new System.Windows.Forms.Label();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.lbAdd = new System.Windows.Forms.Label();
            this.lbHexAdd = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbOutput
            // 
            this.lbOutput.BackColor = System.Drawing.Color.Pink;
            this.lbOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbOutput.Font = new System.Drawing.Font("맑은 고딕", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbOutput.Location = new System.Drawing.Point(0, 0);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(21, 28);
            this.lbOutput.TabIndex = 1;
            this.lbOutput.Text = "O";
            this.lbOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.Pink;
            this.lbTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.Blue;
            this.lbTitle.Location = new System.Drawing.Point(0, 29);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(143, 28);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Pusher Overload";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbState
            // 
            this.lbState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbState.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbState.Location = new System.Drawing.Point(144, 0);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(46, 57);
            this.lbState.TabIndex = 3;
            this.lbState.Text = "OFF";
            this.lbState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbState.DoubleClick += new System.EventHandler(this.lbState_DoubleClick);
            // 
            // lbAdd
            // 
            this.lbAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAdd.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAdd.Location = new System.Drawing.Point(22, 0);
            this.lbAdd.Name = "lbAdd";
            this.lbAdd.Size = new System.Drawing.Size(60, 28);
            this.lbAdd.TabIndex = 4;
            this.lbAdd.Text = "NO : 19";
            this.lbAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHexAdd
            // 
            this.lbHexAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbHexAdd.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbHexAdd.Location = new System.Drawing.Point(83, 0);
            this.lbHexAdd.Name = "lbHexAdd";
            this.lbHexAdd.Size = new System.Drawing.Size(60, 28);
            this.lbHexAdd.TabIndex = 5;
            this.lbHexAdd.Text = "Y13";
            this.lbHexAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FrameOutputAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(191, 58);
            this.Controls.Add(this.lbHexAdd);
            this.Controls.Add(this.lbAdd);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.lbOutput);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameOutputAPT";
            this.Text = "FrameInputAP";
            this.Shown += new System.EventHandler(this.FrameOutputAPT_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrameOutputAPT_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbOutput;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Label lbAdd;
        private System.Windows.Forms.Label lbHexAdd;
        private System.Windows.Forms.Timer tmUpdate;
    }
}