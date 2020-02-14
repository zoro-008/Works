namespace Machine
{
    partial class FrameInputAPT
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
            this.lbInput = new System.Windows.Forms.Label();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.lbAdd = new System.Windows.Forms.Label();
            this.lbHexAdd = new System.Windows.Forms.Label();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbInput
            // 
            this.lbInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbInput.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbInput.Location = new System.Drawing.Point(0, 0);
            this.lbInput.Name = "lbInput";
            this.lbInput.Size = new System.Drawing.Size(21, 28);
            this.lbInput.TabIndex = 1;
            this.lbInput.Text = "I";
            this.lbInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTitle
            // 
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
            this.lbHexAdd.Text = "X13";
            this.lbHexAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // FrameInputAPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(191, 58);
            this.Controls.Add(this.lbHexAdd);
            this.Controls.Add(this.lbAdd);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.lbInput);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrameInputAPT";
            this.Text = "FrameInputAP";
            this.Shown += new System.EventHandler(this.FrameInputAPT_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrameInputAPT_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbInput;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Label lbAdd;
        private System.Windows.Forms.Label lbHexAdd;
        private System.Windows.Forms.Timer tmUpdate;
    }
}